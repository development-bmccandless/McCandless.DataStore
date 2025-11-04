namespace McCandless.DataStore.Exceptions
{
    using System;

    public class DataStoreException : Exception
    {
        private readonly DataStoreErrorCode errorCode;

        public DataStoreException(DataStoreErrorCode errorCode)
            : this(errorCode, message: null, innerException: null)
        {
        }

        public DataStoreException(DataStoreErrorCode errorCode, string? message)
            : this(errorCode, message, innerException: null)
        {
        }

        public DataStoreException(DataStoreErrorCode errorCode, string? message, Exception? innerException)
            : base(message, innerException)
        {
            this.errorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        }

        public int ErrorCode { get => errorCode.ErrorCode; }

        public string ErrorMessage { get => errorCode.ErrorMessage; }

        // TODO: Implement proper exception serialization
    }
}
