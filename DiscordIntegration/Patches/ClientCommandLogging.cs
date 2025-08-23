using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.API.Features;
using global::DiscordIntegration.Dependency;
using NorthwoodLib.Pools;
using RemoteAdmin;

namespace DiscordIntegration.Patches
{
    [HarmonyPatch]
    internal class ClientCommandLogging
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(QueryProcessor), "ProcessGameConsoleQuery", new Type[] { typeof(string) });
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var logCommandMethod = typeof(ClientCommandLogging).GetMethod(nameof(LogCommand), BindingFlags.NonPublic | BindingFlags.Static);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, logCommandMethod),
            });

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void LogCommand(string query, QueryProcessor processor)
        {
            if (!DiscordIntegration.Instance.Config.EventsToLog.SendingConsoleCommands)
                return;

            if (string.IsNullOrWhiteSpace(query))
                return;

            var spaceArrayField = typeof(QueryProcessor).GetField("SpaceArray", BindingFlags.NonPublic | BindingFlags.Static);
            char[] spaceArray = spaceArrayField?.GetValue(null) as char[] ?? new[] { ' ' };

            string[] args = query.Trim().Split(spaceArray, 512, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length == 0 || args[0].StartsWith("$"))
                return;

            Player player = null;
            if (processor.TryGetSender(out var sender))
            {
                if (sender is PlayerCommandSender playerCommandSender)
                    player = Player.Get(playerCommandSender);
            }

            player ??= Server.Host;

            if (player == null)
                return;

            if (!string.IsNullOrEmpty(player.UserId) &&
                DiscordIntegration.Instance.Config.TrustedAdmins.Contains(player.UserId))
                return;

            string baseCommand = args[0];
            string fullCommand = string.Join(" ", args.Skip(1));

            string message = string.Format(
                DiscordIntegration.Language.UsedCommand,
                sender.Nickname,
                sender.SenderId ?? DiscordIntegration.Language.DedicatedServer,
                player.Role,
                baseCommand,
                fullCommand
            );

            if (DiscordIntegration.Instance.Config.EventsToLog.SendingConsoleCommands)
                _ = DiscordIntegration.Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Command, message));
        }
    }
}