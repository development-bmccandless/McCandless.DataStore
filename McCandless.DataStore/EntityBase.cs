namespace McCandless.DataStore
{
    using System;
    using System.Text.Json.Serialization;

    public abstract class EntityBase<TIdentity>
    {
        [JsonPropertyName("id")]
        [JsonPropertyOrder(0)]
        public string? Id { get; set; }

        [JsonPropertyName("createdBy")]
        [JsonPropertyOrder(990)]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("createdDateTime")]
        [JsonPropertyOrder(991)]
        public DateTime? CreatedDateTime { get; set; }

        [JsonPropertyName("updatedBy")]
        [JsonPropertyOrder(992)]
        public string? UpdatedBy { get; set; }

        [JsonPropertyName("updatedDateTime")]
        [JsonPropertyOrder(993)]
        public DateTime? UpdatedDateTime { get; set; }

        public abstract TIdentity GetIdentity();

        public abstract void SetIdentity(TIdentity identity);
    }
}
