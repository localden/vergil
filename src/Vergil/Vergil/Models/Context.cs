namespace Vergil.Models
{
    public class Context
    {
        public string? Vocab { get; set; }

        public string? NuGet { get; set; }

        public Entity? Items { get; set; }

        public Entity? Parent { get; set; }

        public Entity? CommitTimeStamp { get; set; }

        public Entity? NuGetLastCreated { get; set; }

        public Entity? NuGetLastEdited { get; set; }

        public Entity? NuGetLastDeleted { get; set; }
    }
}
