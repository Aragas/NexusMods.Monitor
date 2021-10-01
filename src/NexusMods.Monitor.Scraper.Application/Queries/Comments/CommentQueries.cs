using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;

using System;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public sealed class CommentQueries : ICommentQueries
    {
        private readonly ICommentRepository _commentRepository;

        public CommentQueries(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        }

        public IQueryable<CommentViewModel> GetAll() => _commentRepository.GetAll()
            .Select(x => new CommentViewModel(x.Id, x.NexusModsGameId, x.NexusModsModId, x.IsLocked, x.IsSticky, x.Replies.Select(y => new CommentReplyViewModel(y.Id, y.OwnerId)).ToImmutableArray()));
    }
}