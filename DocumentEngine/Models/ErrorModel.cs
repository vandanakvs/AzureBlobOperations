using System;

namespace DocumentEngine.Models
{
    public class ErrorModel
    {
        public ErrorModel(ErrorCodes errorCodes)
        {
            ErrorCodes = errorCodes;
        }

        public ErrorModel(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>The error message</summary>
        public ErrorCodes ErrorCodes { get; set; }

        public Exception Exception { get; set; }       
    }

    public class ErrorCodes
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
