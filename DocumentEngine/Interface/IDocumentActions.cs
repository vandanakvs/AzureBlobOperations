using DocumentEngine.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DocumentEngine.Interface
{
    public interface IDocumentActions : ICommonBase
    {
        Task<ActionResponse> UploadTextAsync(string fileName, string data, string partitionName , Dictionary<string, string> metaData = null);
        Task<List<ActionResponse>> BulkUploadTextAsync(List<DocumentDetail> documentDetails);
        Task<ActionResponse> UploadBytesAsync(string fileName, byte[] fileBytes, string partitionName, Dictionary<string, string> metaData = null);
        Task<ActionResponse> DownloadByteAsync(string fileName, string partitionName);
        Task<ActionResponse> DownloadTextAsync(string fileName, string partitionName);
        Task<ActionResponse> DeleteAsync(string fileName, string partitionName);
        Task<List<ActionResponse>> BulkUploadBytesAsync(List<DocumentDetail> documentDetails);
    }
}