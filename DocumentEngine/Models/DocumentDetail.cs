using System.Collections.Generic;

namespace DocumentEngine.Models
{
    public class DocumentDetail
    {
        public DocumentDetail()
        {
            Errors = new List<ErrorModel>();
        }

        public string Name { get; set; }
        public byte[] Bytes { get; set; }

        public string Data { get; set; }
        public string PartitionName { get; set; }
        public Dictionary<string, string> MetaData { get; set; }

        public string BlobUri { get; set; }

        public bool IsValid => Errors == null || Errors.Count == 0;

        public List<ErrorModel> Errors { get; set; }
    }
}
