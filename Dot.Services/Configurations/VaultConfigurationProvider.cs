using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp;
using Microsoft.Extensions.Configuration;
using VaultSharp.V1.Commons;

namespace Dot.Services.Configurations
{
    public class VaultConfigurationProvider : ConfigurationProvider
    {
        private readonly VaultOptions _config;
        private readonly IVaultClient _client;

        public VaultConfigurationProvider(VaultOptions config)
        {
            _config = config;

            var vaultClientSettings = new VaultClientSettings(
                _config.Address,
                new AppRoleAuthMethodInfo(_config.RoleId,
                                          _config.SecretId)
            );

            _client = new VaultClient(vaultClientSettings);
        }

        public override void Load()
        {
            LoadAsync().Wait();
        }

        public async Task LoadAsync()
        {
            await LoadSecrets();
        }

        public async Task LoadSecrets()
        {
            foreach (var path in _config.Secrets)
            {
                Secret<SecretData> secrets = await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                  path, null, _config.MountPath);
                foreach (var kvp in secrets.Data.Data)
                {
                    Data.Add(kvp.Key, kvp.Value.ToString());
                }
            }      
        }
    }

    public class VaultConfigurationSource : IConfigurationSource
    {
        private VaultOptions _config;

        public VaultConfigurationSource(Action<VaultOptions> config)
        {
            _config = new VaultOptions();
            config.Invoke(_config);
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new VaultConfigurationProvider(_config);
        }
    }

    public class VaultOptions
    {
        public string Address { get; set; }
        public string RoleId { get; set; }
        public string SecretId { get; set; }
        public string MountPath { get; set; }
        public string SecretType { get; set; }
        public List<string> Secrets { get; set; }
    }

    public static class VaultExtensions
    {
        public static IConfigurationBuilder AddVault(this IConfigurationBuilder configuration, Action<VaultOptions> options)
        {
            var vaultOptions = new VaultConfigurationSource(options);
            configuration.Add(vaultOptions);
            return configuration;
        }
    }
}
