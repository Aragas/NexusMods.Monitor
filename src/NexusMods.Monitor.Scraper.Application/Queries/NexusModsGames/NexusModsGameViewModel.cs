using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames
{
    [DataContract]
    public sealed class NexusModsGameViewModel
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public string Name { get; private set; } = default!;
        [DataMember]
        public string ForumUrl { get; private set; } = default!;
        [DataMember]
        public string NexusModsUrl { get; private set; } = default!;
        [DataMember]
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