using System.Text.Json.Serialization;

namespace Vergil.Models
{
    public class Entity
    {
        [JsonPropertyName("@id")]
        public string? QualifiedId { get; set; }

        public string? Container { get; set; }

        public string? Type { get; set; }

        public string? CommitId { get; set; }

        public DateTime CommitTimeStamp { get; set; }

        public int Count { get; set; }

        public string? Id { get; set; }

        public string? Range { get; set; }

        [JsonPropertyName("nuget:id")]
        public string? NuGetId { get; set; }

        [JsonPropertyName("nuget:version")]
        public string? NuGetVersion { get; set; }
    }
}
