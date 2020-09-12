namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames
{
    public sealed class NexusModsGameViewModel
    {
        public uint Id { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string ForumUrl { get; private set; } = default!;
        public string NexusModsUrl { get; private set; } = default!;
        public string DomainName { get; private set; } = default!;

        private NexusModsGameViewModel() { }
        public NexusModsGameViewModel(uint id, string name, string forumUrl, string nexusModsUrl, string domainName) : this()
        {
            Id = id;
            Name = name;
            ForumUrl = forumUrl;
            NexusModsUrl = nexusModsUrl;
            DomainName = domainName;
        }
    }
}