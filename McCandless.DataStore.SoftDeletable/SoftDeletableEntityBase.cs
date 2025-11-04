namespace McCandless.DataStore.SoftDeletable
{
    using System;
    using System.Text.Json.Serialization;

    public abstract class SoftDeletableEntityBase<TIdentity> : EntityBase<TIdentity>
    {
        [JsonPropertyName("deletedBy")]
        [JsonPropertyOrder(994)]
        public string? DeletedBy { get; set; }

        [JsonPropertyName("deletedDateTime")]
        [JsonPropertyOrder(995)]
        public DateTime? DeletedDateTime { get; set; }

        [JsonPropertyName("isDeleted")]
        [JsonPropertyOrder(996)]
        public bool? IsDeleted { get; set; }
    }
}
