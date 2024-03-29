﻿using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    public sealed class IssueQueries : IIssueQueries
    {
        private readonly IIssueRepository _issueRepository;

        public IssueQueries(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }

        public IAsyncEnumerable<IssueViewModel> GetAllAsync(uint nexusModsGameId, uint nexusModsModId, CancellationToken ct) => _issueRepository.GetAll()
            .Where(x => x.NexusModsGameId == nexusModsGameId && x.NexusModsModId == nexusModsModId)
            .Select(x => new IssueViewModel(x.Id, x.NexusModsGameId, x.NexusModsModId, x.Status.Id, x.Priority.Id, x.IsClosed, x.IsPrivate, x.TimeOfLastPost, x.Replies.Select(y => new IssueReplyViewModel(y.Id, y.OwnerId)).ToImmutableArray()))
            .ToAsyncEnumerable();

        public IAsyncEnumerable<IssueViewModel> GetAllAsync(CancellationToken ct) => _issueRepository.GetAll()
            .Select(x => new IssueViewModel(x.Id, x.NexusModsGameId, x.NexusModsModId, x.Status.Id, x.Priority.Id, x.IsClosed, x.IsPrivate, x.TimeOfLastPost, x.Replies.Select(y => new IssueReplyViewModel(y.Id, y.OwnerId)).ToImmutableArray()))
            .ToAsyncEnumerable();

        public async Task<IssueStatusEnumeration> GetStatusAsync(uint id, CancellationToken ct = default) => await _issueRepository.GetStatusAsync(id);

        public async Task<IssuePriorityEnumeration> GetPriorityAsync(uint id, CancellationToken ct = default) => await _issueRepository.GetPriorityAsync(id);
    }
}