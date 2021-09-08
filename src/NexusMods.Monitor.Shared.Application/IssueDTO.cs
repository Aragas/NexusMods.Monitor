using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Shared.Application
{
    public sealed record IssueDTO(uint Id, uint NexusModsGameId, uint NexusModsModId, string GameName, string ModName, string Title, string Url, string ModVersion, IssueStatusDTO Status, IssuePriorityDTO Priority, bool IsPrivate, bool IsClosed, DateTimeOffset TimeOfLastPost, IssueContentDTO? Content, IReadOnlyCollection<IssueReplyDTO> Replies);

    public sealed record IssueStatusDTO(uint Id, string Name);

    public sealed record IssuePriorityDTO(uint Id, string Name);

    public sealed record IssueContentDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, DateTimeOffset TimeOfPost);

    public sealed record IssueReplyDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, DateTimeOffset TimeOfPost);
}