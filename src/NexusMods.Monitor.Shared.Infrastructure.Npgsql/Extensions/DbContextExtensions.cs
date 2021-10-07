using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

using Npgsql;

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Infrastructure.Npgsql.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// We assume that some tables or data seeds might or might not exist. We assume that the the table schemas didn't change or are changed manually.
        /// </summary>
        public static async Task<bool> UpsertDatabaseSchemaAsync(this DbContext context, CancellationToken ct = default)
        {
            var ensureCreated = await context.Database.EnsureCreatedAsync(ct);

            if (!ensureCreated)
            {
                var staticModel = context.GetService<IModel>();
                var staticModelRelationalModel = staticModel.GetRelationalModel();

                var modelDiffer = context.GetService<IMigrationsModelDiffer>();
                var migrationsSqlGenerator = context.GetService<IMigrationsSqlGenerator>();
                var migrationCommandExecutor = context.GetService<IMigrationCommandExecutor>();
                var connection = context.GetService<IRelationalConnection>();

                var migrationOperations = modelDiffer.GetDifferences(null, staticModelRelationalModel);
                var migrationSqlCommands = migrationsSqlGenerator.Generate(migrationOperations, staticModel).ToImmutableArray();
                if (migrationSqlCommands.Length > 0)
                {
                    try
                    {
                        await migrationCommandExecutor.ExecuteNonQueryAsync(migrationSqlCommands, connection, ct);
                    }
                    catch (Exception e) when (e is PostgresException) { } // TODO: Patching on ARM is not working
                    return true;
                }
            }

            return ensureCreated;
        }
    }
}