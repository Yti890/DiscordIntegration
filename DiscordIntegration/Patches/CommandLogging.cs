using DiscordIntegration.Dependency;
using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DiscordIntegration.Patches
{
    [HarmonyPatch]
    internal static class CommandLogging
    {
        private static readonly char[] SpaceArray = { ' ' };

        [HarmonyPatch(typeof(CommandProcessor))]
        [HarmonyPatch("ProcessQuery")]
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(CommandProcessor), "ProcessQuery");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = new List<CodeInstruction>(instructions);

            var logCommandMethod = typeof(CommandLogging).GetMethod(nameof(LogCommand), BindingFlags.NonPublic | BindingFlags.Static);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, logCommandMethod),
            });

            foreach (var instruction in newInstructions)
                yield return instruction;
        }

        private static void LogCommand(string query, CommandSender sender)
        {
            if (!DiscordIntegration.Instance.Config.EventsToLog.SendingRemoteAdminCommands &&
                !DiscordIntegration.Instance.Config.StaffOnlyEventsToLog.SendingRemoteAdminCommands)
                return;

            if (string.IsNullOrWhiteSpace(query))
                return;

            string[] args = query.Trim().Split(SpaceArray, 512, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length == 0 || args[0].StartsWith("$", StringComparison.Ordinal))
                return;

            Player player = sender is PlayerCommandSender playerCommandSender
                ? Player.Get(playerCommandSender)
                : Server.Host;

            if (player == null)
                return;

            if (!string.IsNullOrEmpty(player.UserId) && DiscordIntegration.Instance.Config.TrustedAdmins.Contains(player.UserId))
                return;

            string commandName = args[0];
            string commandArgs = string.Join(" ", args.Skip(1));

            string message = string.Format(
                DiscordIntegration.Language.UsedCommand,
                sender.Nickname,
                sender.SenderId ?? DiscordIntegration.Language.DedicatedServer,
                player.Role,
                commandName,
                commandArgs);

            if (DiscordIntegration.Instance.Config.EventsToLog.SendingRemoteAdminCommands)
                _ = DiscordIntegration.Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Command, message));

            if (DiscordIntegration.Instance.Config.StaffOnlyEventsToLog.SendingRemoteAdminCommands)
                _ = DiscordIntegration.Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.StaffCopy, message));
        }
    }
}