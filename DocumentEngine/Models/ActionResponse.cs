using System;
using System.Collections.Generic;

namespace DocumentEngine.Models
{
    public class ActionResponse
    {
        private ActionResponse(string blobName, bool success, List<ErrorModel> errors)
        {
            BlobName = blobName;
            Success = success;
            Errors = errors;
        }

        private ActionResponse(string blobName, bool success, List<ErrorModel> errors, Byte[] bytes)
        {
            BlobName = blobName;
            Success = success;
            Errors = errors;
            BlobByte = bytes;
        }

        private ActionResponse(string blobName, bool success, List<ErrorModel> errors, string blobContent)
        {
            BlobName = blobName;
            Success = success;
            Errors = errors;
            BlobContent = blobContent;
        }

        private ActionResponse(string blobName, bool success, string uri)
        {
            BlobName = blobName;
            Success = success;
            BlobUri = uri;
        }

        public string BlobName { get; }

        public string BlobContent { get; }

        public string BlobUri { get; }
        public bool Success { get; }

        public byte[] BlobByte { get; }
        public List<ErrorModel> Errors { get; set; }
        public static ActionResponse FailResponse(string blobName, ErrorCodes error) => new ActionResponse(blobName, false, new List<ErrorModel> { new ErrorModel(error) });

        public static ActionResponse FailResponse(string blobName, Exception exception) => new ActionResponse(blobName, false, new List<ErrorModel> { new ErrorModel(exception) });
        public static ActionResponse SuccessResponse(string blobName) => new ActionResponse(blobName, true, errors: null);
        public static ActionResponse UploadSuccessResponse(string blobName, string uri) => new ActionResponse(blobName, true, uri);
        public static ActionResponse DownloadSuccessResponse(string blobName, byte[] bytes) => new ActionResponse(blobName, true, null, bytes);
        public static ActionResponse DownloadSuccessResponse(string blobName, string content) => new ActionResponse(blobName, true, null, content);
    }
}
