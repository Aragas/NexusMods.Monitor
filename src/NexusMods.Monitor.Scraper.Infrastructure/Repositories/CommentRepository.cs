using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Infrastructure.Contexts;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Repositories
{
    public sealed class CommentRepository : ICommentRepository
    {
        private readonly NexusModsDb _context;

        public IUnitOfWork UnitOfWork => _context;

        public CommentRepository(NexusModsDb context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public CommentEntity Add(CommentEntity commentEntity)
        {
            return  _context.CommentEntities.Add(commentEntity).Entity;
        }

        public async Task<CommentEntity?> GetAsync(uint commentEntityId)
        {
            var commentEntity = await _context
                .CommentEntities
                .Include(x => x.Replies)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == commentEntityId);

            if (commentEntity is { })
                return commentEntity;

            commentEntity = _context
                .CommentEntities
                .Local
                .FirstOrDefault(o => o.Id == commentEntityId);

            if (commentEntity is { })
            {
                await _context.Entry(commentEntity)
                    .Collection(i => i.Replies).LoadAsync();
            }

            return commentEntity;
        }
        public IQueryable<CommentEntity> GetAll() => _context.CommentEntities
            .Include(x => x.Replies);

        public CommentEntity Update(CommentEntity commentEntity)
        {
            return  _context.CommentEntities.Update(commentEntity).Entity;
        }
    }
}