using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    public sealed class IssueQueries : IIssueQueries
    {
        private readonly IIssueRepository _issueRepository;

        public IssueQueries(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }

        public IAsyncEnumerable<IssueViewModel> GetAllAsync() => _issueRepository.GetAll()
            .ToAsyncEnumerable()
            .Select(x => new IssueViewModel(x.Id, x.NexusModsGameId, x.NexusModsModId, x.Status.Id, x.Priority.Id, x.IsClosed, x.IsPrivate, x.TimeOfLastPost, x.Replies.Select(y => new IssueReplyViewModel(y.Id, y.OwnerId)).ToImmutableArray()));
    }
}