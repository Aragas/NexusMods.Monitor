using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;

using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NexusMods.Monitor.Shared.Infrastructure.Npgsql
{
    [SuppressMessage("Usage", "EF1001", Justification = "Bootleg")]
    public class NpgsqlMigrationsSqlGenerator2 : NpgsqlMigrationsSqlGenerator
    {
        private delegate string DelimitIdentifierDelegate(NpgsqlMigrationsSqlGenerator instance, string identifier);
        private static readonly DelimitIdentifierDelegate DelimitIdentifier =
            AccessTools2.GetDelegate<DelimitIdentifierDelegate>(typeof(NpgsqlMigrationsSqlGenerator), "DelimitIdentifier", new[] { typeof(string) });
        private delegate string DelimitIdentifier2Delegate(NpgsqlMigrationsSqlGenerator instance, string identifier, string? schema);
        private static readonly DelimitIdentifier2Delegate DelimitIdentifier2 =
            AccessTools2.GetDelegate<DelimitIdentifier2Delegate>(typeof(NpgsqlMigrationsSqlGenerator), "DelimitIdentifier", new[] { typeof(string), typeof(string) });

        private delegate Dictionary<string, string> GetStorageParametersDelegate(NpgsqlMigrationsSqlGenerator instance, Annotatable annotatable);
        private static readonly GetStorageParametersDelegate GetStorageParameters =
            AccessTools2.GetDelegate<GetStorageParametersDelegate>(typeof(NpgsqlMigrationsSqlGenerator), "GetStorageParameters");

        private delegate bool IsSystemColumnDelegate(NpgsqlMigrationsSqlGenerator instance, string name);
        private static readonly IsSystemColumnDelegate IsSystemColumn =
            AccessTools2.GetDelegate<IsSystemColumnDelegate>(typeof(NpgsqlMigrationsSqlGenerator), "IsSystemColumn");

        private delegate Array GetIndexColumnsDelegate(CreateIndexOperation operation);
        private static readonly GetIndexColumnsDelegate GetIndexColumns =
            AccessTools2.GetDelegate<GetIndexColumnsDelegate>(typeof(NpgsqlMigrationsSqlGenerator), "GetIndexColumns");

        private delegate string IndexColumnListDelegate(NpgsqlMigrationsSqlGenerator instance, Array columns, string? method);
        private static readonly IndexColumnListDelegate IndexColumnList =
            AccessTools2.GetDelegate<IndexColumnListDelegate>(typeof(NpgsqlMigrationsSqlGenerator), "IndexColumnList");

        private delegate string ColumnsToTsVectorDelegate(NpgsqlMigrationsSqlGenerator instance, string columnOrIndexName, IEnumerable<string> columnNames, string tsVectorConfig, IModel? model, string? schema, string table);
        private static readonly ColumnsToTsVectorDelegate ColumnsToTsVector =
            AccessTools2.GetDelegate<ColumnsToTsVectorDelegate>(typeof(NpgsqlMigrationsSqlGenerator), "ColumnsToTsVector");

        private static readonly AccessTools.FieldRef<NpgsqlMigrationsSqlGenerator, RelationalTypeMapping> _stringTypeMapping =
            AccessTools2.FieldRefAccess<NpgsqlMigrationsSqlGenerator, RelationalTypeMapping>("_stringTypeMapping");

        public NpgsqlMigrationsSqlGenerator2(MigrationsSqlGeneratorDependencies dependencies, INpgsqlSingletonOptions npgsqlOptions) : base(dependencies, npgsqlOptions) { }

        protected override void Generate(CreateTableOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            if (!terminate && operation.Comment is not null)
            {
                throw new ArgumentException($"When generating migrations SQL for {nameof(CreateTableOperation)}, can't produce unterminated SQL with comments");
            }

            operation.Columns.RemoveAll(c => IsSystemColumn(this, c.Name));

            builder.Append("CREATE ");

            if (operation[NpgsqlAnnotationNames.UnloggedTable] is bool unlogged && unlogged)
            {
                builder.Append("UNLOGGED ");
            }

            builder
                .Append("TABLE IF NOT EXISTS ")
                .Append(DelimitIdentifier2(this, operation.Name, operation.Schema))
                .AppendLine(" (");

            using (builder.Indent())
            {
                base.CreateTableColumns(operation, model, builder);
                base.CreateTableConstraints(operation, model, builder);
                builder.AppendLine();
            }

            builder.Append(")");

            // CockroachDB "interleave in parent" (https://www.cockroachlabs.com/docs/stable/interleave-in-parent.html)
            if (operation[CockroachDbAnnotationNames.InterleaveInParent] is string)
            {
                var interleaveInParent = new CockroachDbInterleaveInParent(operation);
                var parentTableSchema = interleaveInParent.ParentTableSchema;
                var parentTableName = interleaveInParent.ParentTableName;
                var interleavePrefix = interleaveInParent.InterleavePrefix;

                builder
                    .AppendLine()
                    .Append("INTERLEAVE IN PARENT ")
                    .Append(DelimitIdentifier2(this, parentTableName, parentTableSchema))
                    .Append(" (")
                    .Append(string.Join(", ", interleavePrefix.Select(c => DelimitIdentifier(this, c))))
                    .Append(")");
            }

            var storageParameters = GetStorageParameters(this, operation);
            if (storageParameters.Count > 0)
            {
                builder
                    .AppendLine()
                    .Append("WITH (")
                    .Append(string.Join(", ", storageParameters.Select(p => $"{p.Key}={p.Value}")))
                    .Append(")");
            }

            // Comment on the table
            if (operation.Comment is not null)
            {
                builder.AppendLine(";");

                builder
                    .Append("COMMENT ON TABLE ")
                    .Append(DelimitIdentifier2(this, operation.Name, operation.Schema))
                    .Append(" IS ")
                    .Append(_stringTypeMapping(this).GenerateSqlLiteral(operation.Comment));
            }

            // Comments on the columns
            foreach (var columnOp in operation.Columns.Where(c => c.Comment is not null))
            {
                var columnComment = columnOp.Comment;
                builder.AppendLine(";");

                builder
                    .Append("COMMENT ON COLUMN ")
                    .Append(DelimitIdentifier2(this, operation.Name, operation.Schema))
                    .Append(".")
                    .Append(DelimitIdentifier(this, columnOp.Name))
                    .Append(" IS ")
                    .Append(_stringTypeMapping(this).GenerateSqlLiteral(columnComment));
            }

            if (terminate)
            {
                builder.AppendLine(";");
                EndStatement(builder);
            }
        }

        protected override void Generate(CreateIndexOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            builder.Append("CREATE ");

            if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }

            builder.Append("INDEX IF NOT EXISTS ");

            var concurrently = operation[NpgsqlAnnotationNames.CreatedConcurrently] as bool? == true;
            if (concurrently)
            {
                builder.Append("CONCURRENTLY ");
            }

            builder
                .Append(DelimitIdentifier(this, operation.Name))
                .Append(" ON ")
                .Append(DelimitIdentifier2(this, operation.Table, operation.Schema));

            var method = operation[NpgsqlAnnotationNames.IndexMethod] as string;
            if (method?.Length > 0)
            {
                builder.Append(" USING ").Append(method);
            }

            var indexColumns = GetIndexColumns(operation);
            var indexColumns2 = Unsafe.As<IndexColumn[]>(indexColumns);

            var columnsExpression = operation[NpgsqlAnnotationNames.TsVectorConfig] is string tsVectorConfig
                ? ColumnsToTsVector(this, operation.Name, indexColumns2.Select(i => i.Name), tsVectorConfig, model, operation.Schema, operation.Table)
                : IndexColumnList(this, indexColumns, method);

            builder
                .Append(" (")
                .Append(columnsExpression)
                .Append(")");

            IndexOptions(operation, model, builder);

            if (terminate)
            {
                builder.AppendLine(";");
                // Concurrent indexes cannot be created within a transaction
                EndStatement(builder, suppressTransaction: concurrently);
            }
        }

        protected override void Generate(InsertDataOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            var sqlBuilder = new StringBuilder();
            foreach (var modificationCommand in GenerateModificationCommands(operation, model))
            {
                var overridingSystemValue = modificationCommand.ColumnModifications.Any(m => m.Property?.GetValueGenerationStrategy() == NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);
                var subSqlBuilder = new StringBuilder();
                ((NpgsqlUpdateSqlGenerator) Dependencies.UpdateSqlGenerator).AppendInsertOperation(subSqlBuilder, modificationCommand, 0, overridingSystemValue, out _);
                subSqlBuilder.Replace(";", " ON CONFLICT DO NOTHING;");
                sqlBuilder.Append(subSqlBuilder);
            }

            builder.Append(sqlBuilder.ToString());

            if (terminate)
            {
                builder.EndCommand();
            }
        }

        private readonly struct IndexColumn
        {
            public IndexColumn(string name, string? @operator, string? collation, bool isDescending, NullSortOrder nullSortOrder)
            {
                Name = name;
                Operator = @operator;
                Collation = collation;
                IsDescending = isDescending;
                NullSortOrder = nullSortOrder;
            }

            public string Name { get; }
            public string? Operator { get; }
            public string? Collation { get; }
            public bool IsDescending { get; }
            public NullSortOrder NullSortOrder { get; }
        }
    }
}