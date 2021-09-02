﻿using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Shared.Application
{
    public sealed record CommentDTO(uint Id, uint NexusModsGameId, uint NexusModsModId, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsSticky, bool IsLocked, DateTimeOffset TimeOfPost, IReadOnlyCollection<CommentReplyDTO> Replies);

    public sealed record CommentReplyDTO(uint Id, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsDeleted, DateTimeOffset TimeOfPost);
}