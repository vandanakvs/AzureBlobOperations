using Microsoft.Extensions.Logging;
using DocumentEngine.Interface;
using DocumentEngine.Models;
using DocumentEngine.Resources;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentEngine
{
    public class BlobActions : CommonBase, IDocumentActions
    {       
        public BlobActions(string accountName, string storageKey, string containerName, int maxRetryValueForBlobAction, ILogger logger)
        {
            _accountName = accountName;
            _storageKey = storageKey;
            _containerName = containerName;
            _maxRetryValueForBlobAction = maxRetryValueForBlobAction;
            _logger = logger;
        }

        private ILogger _logger { get; set; }
        private string _accountName { get; set; }

        private string _storageKey { get; set; }

        private string _containerName { get; set; }

        private int _maxRetryValueForBlobAction { get; set; }
        public async Task<ActionResponse> DeleteAsync(string fileName, string partitionName)
        {
            CloudBlockBlob blockBlob = null;
            bool blobExist = false;
            bool deleteResult = false;
            ActionResponse result = null;
            var blockName = GetBlockName(partitionName, fileName);
            try
            {
                var policyResult = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        _maxRetryValueForBlobAction,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogError($"Retey {retryCount} after {timeSpan.TotalSeconds} of Blob {exception.Message}");
                        }
                    );

                await policyResult.ExecuteAsync(
                    async () =>
                    {
                        if (blockBlob == null)
                        {
                            blockBlob = await GetBlockBlobAsync(_containerName, blockName);
                            blobExist = await blockBlob.ExistsAsync();
                        }

                        if (blockBlob != null && blobExist)
                        {
                            deleteResult = await blockBlob.DeleteIfExistsAsync();
                            result = deleteResult ? ActionResponse.SuccessResponse(blockName) : ActionResponse.FailResponse(blockName, new ErrorCodes { Message = Constants.SomethingWentWrongMessage, Code = Constants.SomethingWentWrongCode });
                        }
                        else if (!blobExist)
                        {
                            result = ActionResponse.FailResponse(blockName, new ErrorCodes { Message = string.Format(Constants.BlobDoesNotExistMessage, blockName, _accountName), Code = Constants.BlobDoesNotExistCode });                           
                        }
                    });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to delete {blockName}");
                throw ex;
            }

            return result;
        }

        public async Task<ActionResponse> DownloadByteAsync(string fileName, string partitionName)
        {
            CloudBlockBlob blockBlob = null;
            byte[] fileContent = null;
            var blockName = GetBlockName(partitionName, fileName);
            try
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        _maxRetryValueForBlobAction,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogError($"Retey {retryCount} after {timeSpan.TotalSeconds} of Blob {exception.Message}");
                        }
                    );

                await retryPolicy.ExecuteAsync(async () =>
                {
                    if (blockBlob == null)
                    {
                        blockBlob = await GetBlockBlobAsync(_containerName, blockName);
                        await blockBlob.FetchAttributesAsync();
                        long fileByteLength = blockBlob.Properties.Length;
                        fileContent = new byte[fileByteLength];
                    }

                    if (blockBlob != null)
                    {
                        await blockBlob.DownloadToByteArrayAsync(fileContent, 0);
                    }

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to download bytes for {blockName}");
                throw ex;
            }

            return ActionResponse.DownloadSuccessResponse(blockName, fileContent);
        }

        public async Task<ActionResponse> DownloadTextAsync(string fileName, string partitionName)
        {
            CloudBlockBlob blockBlob = null;
            string documentText = null;
            var blockName = GetBlockName(partitionName, fileName);
            try
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        _maxRetryValueForBlobAction,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogError($"Retey {retryCount} after {timeSpan.TotalSeconds} of Blob {exception.Message}");
                        }
                    );

                await retryPolicy.ExecuteAsync(async () =>
                {
                    if (blockBlob == null)
                    {
                        blockBlob = await GetBlockBlobAsync(_containerName, blockName);                        
                    }

                    if (blockBlob != null)
                    {
                        documentText = await blockBlob.DownloadTextAsync();
                    }

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to download text for {blockName}");
                throw ex;
            }

            return ActionResponse.DownloadSuccessResponse(blockName, documentText);
        }        

        public async Task<ActionResponse> UploadBytesAsync(string fileName, byte[] fileBytes, string partitionName, Dictionary<string, string> metaData = null)
        {
            return await UploadBlockAsync(fileName, fileBytes, partitionName, metaData, null,DocumentContentType.Byte);
        }

        public async Task<ActionResponse> UploadTextAsync(string fileName, string data, string partitionName, Dictionary<string, string> metaData = null)
        {
            return await UploadBlockAsync(fileName, null, partitionName, metaData, data, DocumentContentType.Text);
        }

        public async Task<List<ActionResponse>> BulkUploadTextAsync(List<DocumentDetail> documentDetails)
        {
            return await Task.Run(() =>
            {
                List<ActionResponse> actionResponses = new List<ActionResponse>();
                //parallel
                Parallel.ForEach(documentDetails, (a) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(a.Data))
                            actionResponses.Add(Task.Run(async () => await UploadBlockAsync(a.Name, null, a.PartitionName, a.MetaData, a.Data, DocumentContentType.Text)).Result);
                        else
                            actionResponses.Add(ActionResponse.FailResponse(a.Name, new ErrorCodes { Message = Constants.DocumentContentNotFoundMessage, Code = Constants.DocumentContentNotFoundCode }));
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Unable to upload {a.Name} document");
                        actionResponses.Add(ActionResponse.FailResponse(a.Name, exception));
                    }
                });
                return actionResponses;
            });
        }

        public async Task<List<ActionResponse>> BulkUploadBytesAsync(List<DocumentDetail> documentDetails)
        {
            return await Task.Run( () => 
            {
                List<ActionResponse> actionResponses = new List<ActionResponse>();
                //parallel
                Parallel.ForEach(documentDetails, (a) =>
                {
                    try
                    {
                        if (a.Bytes != null && a.Bytes.Length > 0)
                            actionResponses.Add(Task.Run(async () => await UploadBlockAsync(a.Name, a.Bytes, a.PartitionName, a.MetaData, null, DocumentContentType.Byte)).Result);
                        else
                            actionResponses.Add(ActionResponse.FailResponse(a.Name, new ErrorCodes { Message = Constants.DocumentContentNotFoundMessage, Code = Constants.DocumentContentNotFoundCode }));
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Unable to upload {a.Name} document");
                        actionResponses.Add(ActionResponse.FailResponse(a.Name, exception));
                    }
                });

                return actionResponses;
            });
        }

        private async Task<CloudBlockBlob> GetBlockBlobAsync(string containerName, string blockName)
        {
            var blobContainer = new CloudStorageAccount(new StorageCredentials(_accountName, _storageKey), true).CreateCloudBlobClient().GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync();
            return blobContainer.GetBlockBlobReference(blockName);
        }

        private async Task<ActionResponse> UploadBlockAsync(string fileName, byte[] fileBytes, string partitionName, Dictionary<string, string> metaData, string data, DocumentContentType documentContentType)
        {
            CloudBlockBlob blockBlob = null;
            var blobUri = string.Empty;
            var blockName = GetBlockName(partitionName, fileName);

            var result = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    _maxRetryValueForBlobAction,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogError($"Retey {retryCount} after {timeSpan.TotalSeconds} of Blob {exception.Message}");
                    }
                );

            await result.ExecuteAsync(
                async () =>
                {
                    if (blockBlob == null)
                    {
                        blockBlob = await GetBlockBlobAsync(_containerName, blockName);
                    }

                    if (blockBlob != null)
                    {
                        await blockBlob.DeleteIfExistsAsync();
                        switch (documentContentType)
                        {
                            case DocumentContentType.Byte:
                                await blockBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);
                                break;
                            case DocumentContentType.Text:
                                await blockBlob.UploadTextAsync(data);
                                break;
                            default:
                                break;
                        }

                        if (metaData != null)
                        {
                            foreach (var keyValue in metaData)
                            {
                                if (blockBlob.Metadata.ContainsKey(keyValue.Key))
                                {
                                    blockBlob.Metadata.Remove(keyValue);
                                }

                                blockBlob.Metadata.Add(keyValue);
                            }

                            await blockBlob.SetMetadataAsync();
                        }

                        blobUri = blockBlob.Uri.ToString();
                    }
                });

            return !string.IsNullOrEmpty(blobUri) ? ActionResponse.UploadSuccessResponse(fileName, blobUri) : ActionResponse.FailResponse(fileName, new ErrorCodes { Message = Constants.SomethingWentWrongMessage, Code = Constants.SomethingWentWrongCode });
        }

        private string GetBlockName(string partitionName, string fileName)
        {
            if (string.IsNullOrEmpty(partitionName))
                return fileName;
            else
                return $"{partitionName}/{fileName}";
        }
    }
}
