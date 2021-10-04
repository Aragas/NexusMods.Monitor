using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task EnsureTablesCreatedAsync(this DbContext context, CancellationToken cancellationToken = default)
        {
            if (!await context.Database.EnsureCreatedAsync(cancellationToken))
            {
                var dependencies = context.Database.GetService<RelationalDatabaseCreatorDependencies>();

                var commands = dependencies.MigrationsSqlGenerator
                    .Generate(dependencies.ModelDiffer.GetDifferences(null, dependencies.Model.GetRelationalModel()), dependencies.Model)
                    .Select(c =>
                    {
                        var relCommandField = c.GetType().GetField("_relationalCommand", BindingFlags.NonPublic | BindingFlags.Instance);
                        var relCommand = (IRelationalCommand) relCommandField!.GetValue(c)!;
                        var transformed = TransformSQL(relCommand.CommandText);

                        var depsField = relCommand.GetType().GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                        var deps = (RelationalCommandBuilderDependencies) depsField!.GetValue(relCommand)!;

                        return new MigrationCommand(new RelationalCommand(deps, transformed, relCommand.Parameters), context, c.CommandLogger, c.TransactionSuppressed);
                    })
                    .ToArray();

                if (commands.Length > 0)
                {
                    await dependencies.MigrationCommandExecutor.ExecuteNonQueryAsync(commands, dependencies.Connection, cancellationToken);
                }
            }
        }

        private static string TransformSQL(string command)
        {
            if (command.Contains("insert", StringComparison.InvariantCultureIgnoreCase))
            {
                var splittedCommands = command.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var sb = new StringBuilder();
                foreach (var subCommand in splittedCommands)
                {
                    sb.Append(subCommand).Append(" ON CONFLICT DO NOTHING; ");
                }
                return sb.ToString();
            }
            if (command.Contains("create table", StringComparison.InvariantCultureIgnoreCase))
            {
                return command.Replace("create table", "create table if not exists", StringComparison.InvariantCultureIgnoreCase);
            }
            if (command.Contains("create index", StringComparison.InvariantCultureIgnoreCase))
            {
                return command.Replace("create index", "create index if not exists", StringComparison.InvariantCultureIgnoreCase);
            }
            if (command.Contains("create schema", StringComparison.InvariantCultureIgnoreCase))
            {
                return command;
            }

            return command;
        }
    }
}