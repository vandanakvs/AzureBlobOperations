using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Sourcing.Common
{
    public class AppConfig : IConfig
    {
        private readonly IConfiguration _configuration;
        public AppConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BlobStorageAccount => _configuration["Storage:AccountName"];

        public string BlobStorageKey => _configuration["Storage:StorageKey"];

        public string BlobContainerName => _configuration["Storage:ContainerName"];

        public int MaxRetryValueForBlobAction => int.TryParse(_configuration["Storage:MaxRetryValueForBlobAction"], out var privacyResponseExpirySeconds) ? privacyResponseExpirySeconds : 3;
    }
}