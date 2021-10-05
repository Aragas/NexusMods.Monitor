using HarmonyLib;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NexusMods.Monitor.Shared.Infrastructure.Npgsql;

public static class NpgsqlMigrationsSqlGeneratorPath
{
    public static void Patch(Harmony harmony)
    {
        var generateTableMethod = AccessTools.Method(
            typeof(NpgsqlMigrationsSqlGenerator),
            nameof(NpgsqlMigrationsSqlGenerator.Generate),
            new[] { typeof(CreateTableOperation), typeof(IModel), typeof(MigrationCommandListBuilder), typeof(bool) });

        var generateIndexMethod = AccessTools.Method(
            typeof(NpgsqlMigrationsSqlGenerator),
            nameof(NpgsqlMigrationsSqlGenerator.Generate),
            new[] { typeof(CreateIndexOperation), typeof(IModel), typeof(MigrationCommandListBuilder), typeof(bool) });

        if (generateTableMethod is null || generateIndexMethod is null)
            return;

        harmony.Patch(
            original: generateTableMethod,
            transpiler: new HarmonyMethod(AccessTools.Method(typeof(NpgsqlMigrationsSqlGeneratorPath), nameof(GenerateTableTranspiler))));

        harmony.Patch(
            original: generateIndexMethod,
            transpiler: new HarmonyMethod(AccessTools.Method(typeof(NpgsqlMigrationsSqlGeneratorPath), nameof(GenerateIndexTranspiler))));
    }

    private static IEnumerable<CodeInstruction> GenerateTableTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var codeInstruction in instructions)
        {
            if (codeInstruction.opcode == OpCodes.Ldstr && codeInstruction.operand is string str && str.Trim().Equals("table", StringComparison.OrdinalIgnoreCase))
                codeInstruction.operand = "TABLE IF NOT EXISTS ";

            yield return codeInstruction;
        }
    }

    private static IEnumerable<CodeInstruction> GenerateIndexTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var codeInstruction in instructions)
        {
            if (codeInstruction.opcode == OpCodes.Ldstr && codeInstruction.operand is string str && str.Trim().Equals("index", StringComparison.OrdinalIgnoreCase))
                codeInstruction.operand = "INDEX IF NOT EXISTS ";

            yield return codeInstruction;
        }
    }
}