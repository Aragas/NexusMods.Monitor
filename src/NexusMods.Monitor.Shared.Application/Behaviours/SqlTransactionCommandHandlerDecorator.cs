/*
using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Shared.Domain.Exceptions;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Application.Behaviours
{
    public interface ISqlCommand
    {

    }

    internal class SqlTransactionCommandHandlerDecorator<TCommand, TEntity> : IPipelineBehavior<TCommand, bool> where TCommand : ISqlCommand
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SqlTransactionCommandHandlerDecorator(ILogger<SqlTransactionCommandHandlerDecorator<TCommand, TEntity>> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(TCommand command, CancellationToken ct, RequestHandlerDelegate<bool> next)
        {
            bool result;
            if (_unitOfWork.CurrentTransaction == null)
            {
                await using var transaction = await _unitOfWork.GetOrCreateTransactionAsync(ct);
                _unitOfWork.CurrentTransaction = transaction;
                try
                {
                    _logger.LogInformation(JsonSerializer.Serialize(command));

                    result = await next();
                    await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                    await transaction.CommitAsync(ct);
                    _logger.LogInformation($"Execute: {typeof(TCommand).Name} SUCCESSFULLY");
                }
                catch (DomainException ex)
                {
                    await transaction.RollbackAsync(ct);
                    _logger.LogError(ex, ex.Message);
                    result = false;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(ct);
                    var message = $"Attempted: {typeof(TCommand).Name} and FAILED with message: {ex.Message}";
                    _logger.LogError(ex, message);
                    throw new Exception("Unknown error \n" + message);
                }
            }
            else
            {
                result = await next();
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                return result;
            }

            return result;
        }
    }
}
*/