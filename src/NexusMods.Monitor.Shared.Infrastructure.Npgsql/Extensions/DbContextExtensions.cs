﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Infrastructure.Npgsql.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// We assume that some tables or data seeds might or might not exist. We assume that the the table schemas didn't change or are changed manually.
        /// </summary>
        public static async Task<bool> EnsureTablesCreatedAsync(this DbContext context, CancellationToken cancellationToken = default)
        {
            var ensureCreated = await context.Database.EnsureCreatedAsync(cancellationToken);

            if (!ensureCreated)
            {
                var dependencies = context.Database.GetService<RelationalDatabaseCreatorDependencies>();

                var commands = dependencies.MigrationsSqlGenerator.Generate(dependencies.ModelDiffer.GetDifferences(null, dependencies.Model.GetRelationalModel()), dependencies.Model).ToArray();
                if (commands.Length > 0)
                {
                    await dependencies.MigrationCommandExecutor.ExecuteNonQueryAsync(commands, dependencies.Connection, cancellationToken);
                    return true;
                }
            }

            return ensureCreated;
        }
    }
}