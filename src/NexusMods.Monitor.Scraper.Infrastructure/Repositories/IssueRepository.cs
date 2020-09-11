using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Scraper.Infrastructure.Contexts;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Repositories
{
    public class IssueRepository : IIssueRepository
    {
        private readonly NexusModsDb _context;

        public IUnitOfWork UnitOfWork => _context;

        public IssueRepository(NexusModsDb context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IssueEntity Add(IssueEntity issueEntity)
        {
            _context.Entry(issueEntity).State = EntityState.Added;

            if (issueEntity.Content is { })
                _context.Entry(issueEntity.Content).State = EntityState.Added;

            foreach (var issueReplyEntity in issueEntity.Replies)
                _context.Entry(issueReplyEntity).State = EntityState.Added;

            return issueEntity;
        }

        public async Task<IssueEntity?> GetAsync(uint issueEntityId)
        {
            var issueEntity = await _context
                .IssueEntities
                .Include(x => x.Replies)
                .Include(x => x.Content)
                .Include(x => x.Priority)
                .Include(x => x.Status)
                .FirstOrDefaultAsync(o => o.Id == issueEntityId);

            if (issueEntity is null)
            {
                issueEntity = _context
                    .IssueEntities
                    .Local
                    .FirstOrDefault(o => o.Id == issueEntityId);
            }

            if (issueEntity is { })
            {
                await _context.Entry(issueEntity)
                    .Collection(i => i.Replies).LoadAsync();
                await _context.Entry(issueEntity)
                    .Reference(i => i.Content).LoadAsync();
                await _context.Entry(issueEntity)
                    .Reference(i => i.Priority).LoadAsync();
                await _context.Entry(issueEntity)
                    .Reference(i => i.Status).LoadAsync();
            }

            return issueEntity;
        }
        public IQueryable<IssueEntity> GetAll() => _context.IssueEntities
            .Include(x => x.Replies)
            .Include(x => x.Content)
            .Include(x => x.Priority)
            .Include(x => x.Status);

        public IssueEntity Update(IssueEntity issueEntity)
        {
            _context.Entry(issueEntity).State = EntityState.Modified;
            return issueEntity;
        }
    }
}