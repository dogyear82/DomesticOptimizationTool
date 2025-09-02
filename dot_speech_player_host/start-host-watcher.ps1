param(
  [string]$RabbitHost = "localhost",
  [string]$Queue = "dot.speech.tts.response",
  [string]$RabbitUser,
  [string]$RabbitPass
)

$ErrorActionPreference = "Stop"

# Repo root (one level above this script folder)
$root = Split-Path -Parent $PSScriptRoot
$envPath = Join-Path $root ".env"
$venvDir = Join-Path $root ".host-venv"
$venvPython = Join-Path $venvDir "Scripts\python.exe"
$venvPip = Join-Path $venvDir "Scripts\pip.exe"
$hostAudio = Join-Path $root "shared_audio"

function Load-DotEnv($path) {
  $map = @{}
  if (Test-Path $path) {
    Get-Content $path | ForEach-Object {
      $line = $_.Trim()
      if ($line -and -not $line.StartsWith('#') -and $line.Contains('=')) {
        $k,$v = $line -split '=',2
        $map[$k.Trim()] = $v.Trim()
      }
    }
  }
  return $map
}

Write-Host "[HOST] Starting host audio watcher..." -ForegroundColor Cyan
$envMap = Load-DotEnv $envPath

$rabbitUser = if ($RabbitUser) { $RabbitUser } elseif ($env:RABBITMQ_USER) { $env:RABBITMQ_USER } elseif ($envMap.ContainsKey('ADMIN_USER_NAME')) { $envMap['ADMIN_USER_NAME'] } else { '' }
$rabbitPass = if ($RabbitPass) { $RabbitPass } elseif ($env:RABBITMQ_PASS) { $env:RABBITMQ_PASS } elseif ($envMap.ContainsKey('ADMIN_PASS')) { $envMap['ADMIN_PASS'] } else { '' }

# Try auto-discovering credentials from the running TTS container if not explicitly provided
if (-not $RabbitUser -and -not $RabbitPass) {
  try {
    $cid = (docker ps --filter "name=dot-speech-tts" --format "{{.ID}}" | Select-Object -First 1)
    if ($cid) {
      $envOut = docker exec $cid sh -lc 'printf "%s\n" "$RABBITMQ_USER" "$RABBITMQ_PASS"'
      if ($LASTEXITCODE -eq 0 -and $envOut) {
        $parts = $envOut -split "`r?`n"
        if ($parts.Length -ge 2) {
          if (-not $rabbitUser) { $rabbitUser = $parts[0].Trim() }
          if (-not $rabbitPass) { $rabbitPass = $parts[1].Trim() }
          if ($rabbitUser -and $rabbitPass) {
            Write-Host "[HOST] Discovered RabbitMQ creds from TTS container (user=$rabbitUser)" -ForegroundColor Cyan
          }
        }
      }
    }
  } catch {
    Write-Host "[HOST] Skipping auto-credential discovery: $($_.Exception.Message)" -ForegroundColor Yellow
  }
}

if (-not $rabbitUser) { $rabbitUser = 'root' }
if (-not $rabbitPass) {
  $rabbitPass = Read-Host -Prompt "Enter RabbitMQ admin password"
}

if (-not (Test-Path $hostAudio)) {
  New-Item -ItemType Directory -Path $hostAudio | Out-Null
}

# Ensure venv
if (-not (Test-Path $venvPython)) {
  Write-Host "[HOST] Creating venv at $venvDir"
  python -m venv $venvDir
  & $venvPip install -r (Join-Path $PSScriptRoot "requirements.txt")
}

# Export env for this process (inherited by child python process)
$env:RABBITMQ_HOST = $RabbitHost
$env:RABBITMQ_USER = $rabbitUser
$env:RABBITMQ_PASS = $rabbitPass
$env:TTS_OUTPUT_QUEUE = $Queue
$env:HOST_AUDIO_DIR = (Resolve-Path $hostAudio)

Write-Host "[HOST] RabbitMQ: host=$RabbitHost user=$rabbitUser"
Write-Host "[HOST] Queue: $Queue"
Write-Host "[HOST] Host audio dir: $env:HOST_AUDIO_DIR"

& $venvPython (Join-Path $PSScriptRoot "player.py")
