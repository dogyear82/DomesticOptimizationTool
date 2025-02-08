#enable role based auth
vault auth enable approle

#create app role for fetching secrets from vault
vault write auth/approle/role/dot token_policies="dot-vault-policy" token_ttl="1h" token_max_ttl="4h"

#create policy for the role
vault policy write dot-vault-policy dot-vault-policy.hcl

#retrieve role id
vault read auth/approle/role/dot/role-id

#generate secret_id for role
vault write -f auth/approle/role/dot/secret-id

#create secret path for dot
vault secrets enable -path=dot -version=2 kv

#overwrite all secret in path
vault kv put dot/{{sub-directory}} SECRET_KEY="SECRET_VALUE"

#add secret to path
vault kv patch dot{{}}

#view all secrets in path
vault kv get dot/{{sub-directory}}