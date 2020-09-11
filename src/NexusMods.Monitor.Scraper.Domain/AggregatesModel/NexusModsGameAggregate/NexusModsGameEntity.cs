using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate
{
    public sealed class NexusModsGameEntity : IAggregateRoot
    {
        public uint Id { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string ForumUrl { get; private set; } = default!;
        public string NexusModsUrl { get; private set; } = default!;
        public string DomainName { get; private set; } = default!;

        private NexusModsGameEntity() { }
        public NexusModsGameEntity(uint id, string name, string forumUrl, string nexusModsUrl, string domainName)
        {
            Id = id;
            Name = name;
            ForumUrl = forumUrl;
            NexusModsUrl = nexusModsUrl;
            DomainName = domainName;
        }
    }
}