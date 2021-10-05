using HarmonyLib;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

using System;

namespace NexusMods.Monitor.Shared.Infrastructure.Npgsql.Extensions
{
    public static class IServiceCollectionExtensions
    {
        private static Harmony? Harmony;

        /// <summary>
        /// Don't forget to add <![CDATA[<EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>]]>
        /// </summary>
        public static DbContextOptionsBuilder UseNpgsql2(this DbContextOptionsBuilder builder, string connectionString)
        {
            if (Harmony is null)
            {
                Harmony = new Harmony("NexusMods.Monitor.Shared.Infrastructure.Npgsql.Extensions");
                NpgsqlMigrationsSqlGeneratorPath.Patch(Harmony);
            }

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