using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace NexusMods.Monitor.Shared.Infrastructure.Npgsql.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Don't forget to add <![CDATA[<EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>]]>
        /// </summary>
        public static DbContextOptionsBuilder UseNpgsql2(this DbContextOptionsBuilder builder, string connectionString)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder
                .UseNpgsql(connectionString, o => o.UseNodaTime())
                .ReplaceService<IMigrationsSqlGenerator, NpgsqlMigrationsSqlGenerator2>();

            return builder;
        }
    }
}