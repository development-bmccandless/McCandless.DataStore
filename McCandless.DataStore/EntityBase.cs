namespace McCandless.DataStore
{
    using System;

    public abstract class EntityBase<TIdentity>
    {
        public string? Id { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

        public abstract TIdentity GetIdentity();

        public abstract void SetIdentity(TIdentity identity);
    }
}
