namespace Vergil.Models
{
    public class Catalog
    {
        public string? Id { get; set; }

        public string[]? Type { get; set; }

        public string? CommitId { get; set; }

        public DateTime CommitTimeStamp { get; set; }

        public int Count { get; set; }

        public DateTime NugetlastCreated { get; set; }

        public DateTime NugetlastDeleted { get; set; }

        public DateTime NugetlastEdited { get; set; }

        public List<Entity>? Items { get; set; }

        public Context? Context { get; set; }
    }
}
