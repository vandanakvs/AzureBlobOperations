namespace DocumentEngine.Resources
{
    public static class Constants
    {
        #region Error
        public static string DocumentNotFoundCode = "E1001";
        public static string DocumentNotFoundMessage = "Document not found.";

        public static string DocumentContentNotFoundCode = "E1002";
        public static string DocumentContentNotFoundMessage = "Document is empty. Please check its content.";

        public static string SomethingWentWrongCode = "E1003";
        public static string SomethingWentWrongMessage = "Something went wrong. Please try again later.";

        public static string BlobDoesNotExistCode = "E1004";
        public static string BlobDoesNotExistMessage = "{0} blob doesn't exist in {1} storage account";        
        #endregion Error
    }
}