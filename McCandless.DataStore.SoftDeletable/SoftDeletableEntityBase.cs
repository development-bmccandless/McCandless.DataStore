namespace McCandless.DataStore.SoftDeletable
{
    using System;

    public abstract class SoftDeletableEntityBase<TIdentity> : EntityBase<TIdentity>
    {
        public string? DeletedBy { get; set; }

        public DateTime? DeletedDateTime { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
