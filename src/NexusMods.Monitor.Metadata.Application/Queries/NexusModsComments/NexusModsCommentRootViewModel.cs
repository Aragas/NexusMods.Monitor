namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsComments
{
    public sealed record NexusModsCommentRootViewModel(string NexusModsGameDomain, uint NexusModsGameId, uint NexusModsModId, string GameName, string ModName, NexusModsCommentViewModel NexusModsComment);
}