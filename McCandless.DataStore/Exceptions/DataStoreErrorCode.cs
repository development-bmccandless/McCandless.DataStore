namespace McCandless.DataStore.Exceptions
{
    using System;

    public class DataStoreErrorCode
    {
        public static DataStoreErrorCode NotFound = new DataStoreErrorCode(404, "NotFound");
        public static DataStoreErrorCode Conflict = new DataStoreErrorCode(409, "Conflict");

        public DataStoreErrorCode(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        public int ErrorCode { get; }

        public string ErrorMessage { get; }
    }
}
