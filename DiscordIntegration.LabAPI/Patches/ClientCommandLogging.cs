using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using global::DiscordIntegration.Dependency;
using NorthwoodLib.Pools;
using RemoteAdmin;
using LabApi.Features.Wrappers;
using System.Linq;

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
            if (!PluginStart.Instance.Config.EventsToLog.SendingConsoleCommands)
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
                PluginStart.Instance.Config.TrustedAdmins.Contains(player.UserId))
                return;

            string baseCommand = args[0];
            string fullCommand = string.Join(" ", args.Skip(1));

            string message = string.Format(
                PluginStart.Language.UsedCommand,
                sender.Nickname,
                sender.SenderId ?? PluginStart.Language.DedicatedServer,
                player.Role,
                baseCommand,
                fullCommand
            );

            if (PluginStart.Instance.Config.EventsToLog.SendingConsoleCommands)
                _ = PluginStart.Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Command, message));
        }
    }
}