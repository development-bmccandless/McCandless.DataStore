namespace McCandless.DataStore.Exceptions
{
    public static class DataStoreExceptions
    {
        public static DataStoreException Conflict { get => new DataStoreException(DataStoreErrorCode.Conflict); }

        public static DataStoreException NotFound { get => new DataStoreException(DataStoreErrorCode.NotFound); }
    }
}
