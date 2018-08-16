using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Sourcing.Common
{
    public interface IConfig
    {
        string BlobStorageAccount { get; }
        string BlobStorageKey { get; }
        string BlobContainerName { get; }

        int MaxRetryValueForBlobAction { get; }
    }
}
