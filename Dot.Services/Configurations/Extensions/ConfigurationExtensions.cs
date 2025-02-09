using Microsoft.Extensions.Configuration;

namespace Dot.Services.Configurations.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void AddApplicationSecrets(this IConfigurationBuilder configBuilder)
        {
            configBuilder.AddEnvironmentVariables(prefix: "VAULT_");
            var config = (IConfiguration)configBuilder;
            var vaultOptions = config.GetSection("Vault").Get<VaultOptions>();
            config.EnsureSecretsOptionsArePresent(vaultOptions);

            configBuilder.AddVault(options =>
            {
                options.Address = config.GetEnvironmentVariable("VAULT_ADDR");
                options.RoleId = config.GetEnvironmentVariable("VAULT_ROLE_ID");
                options.MountPath = vaultOptions.MountPath;
                options.SecretType = vaultOptions.SecretType;
                options.SecretId = config.GetEnvironmentVariable("VAULT_SECRET_ID");
                options.Secrets = vaultOptions.Secrets;
            });
        }

        private static void EnsureSecretsOptionsArePresent(this IConfiguration config, VaultOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            if (string.IsNullOrWhiteSpace(options.MountPath))
                throw new ArgumentNullException(nameof(options.MountPath));

            if (string.IsNullOrWhiteSpace(options.SecretType))
                throw new ArgumentNullException(nameof(options.SecretType));

            if (options.Secrets == null || !options.Secrets.Where(x => !string.IsNullOrEmpty(x)).Any())
                throw new ArgumentNullException(nameof(options.Secrets));
        }

        private static string GetEnvironmentVariable(this IConfiguration config, string envVarName)
        {
            var value = config.GetSection(envVarName).Value;
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException($"Environment Variable {envVarName} not set.");

            return value;
        }
    }
}
