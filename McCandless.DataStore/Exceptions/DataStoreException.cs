namespace McCandless.DataStore.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
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

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected DataStoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            errorCode = (DataStoreErrorCode)info.GetValue("errorCode", typeof(DataStoreErrorCode));
        }

        public int ErrorCode { get => errorCode.ErrorCode; }

        public string ErrorMessage { get => errorCode.ErrorMessage; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");

            info.AddValue("errorCode", errorCode);
            base.GetObjectData(info, context);
        }
    }
}
