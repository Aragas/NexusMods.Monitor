using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NexusMods.Monitor.Shared.Infrastructure.Npgsql
{
    [SuppressMessage("Usage", "EF1001", Justification = "Bootleg")]
    public class NpgsqlMigrationsSqlGenerator2 : NpgsqlMigrationsSqlGenerator
    {
        public NpgsqlMigrationsSqlGenerator2(MigrationsSqlGeneratorDependencies dependencies, INpgsqlOptions npgsqlOptions) : base(dependencies, npgsqlOptions) { }

        protected override void Generate(InsertDataOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            var sqlBuilder = new StringBuilder();
            foreach (var modificationCommand in GenerateModificationCommands(operation, model))
            {
                var overridingSystemValue = modificationCommand.ColumnModifications.Any(m => m.Property?.GetValueGenerationStrategy() == NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);
                var subSqlBuilder = new StringBuilder();
                ((NpgsqlUpdateSqlGenerator) Dependencies.UpdateSqlGenerator).AppendInsertOperation(subSqlBuilder, modificationCommand, 0, overridingSystemValue);
                subSqlBuilder.Replace(";", " ON CONFLICT DO NOTHING;");
                sqlBuilder.Append(subSqlBuilder);
            }

            builder.Append(sqlBuilder.ToString());

            if (terminate)
            {
                builder.EndCommand();
            }
        }
    }
}