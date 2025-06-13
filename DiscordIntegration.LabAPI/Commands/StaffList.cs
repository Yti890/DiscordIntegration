// -----------------------------------------------------------------------
// <copyright file="StaffList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace DiscordIntegration.Commands
{
    using System;
    using System.Text;
    using CommandSystem;
    using LabApi.Features.Wrappers;
    using NorthwoodLib.Pools;
    using static PluginStart;

    /// <summary>
    /// Gets the list of staffers in the server.
    /// </summary>
    internal sealed class StaffList : ICommand
    {
#pragma warning disable SA1600 // Elements should be documented
        private StaffList()
        {
        }

        public static StaffList Instance { get; } = new StaffList();

        public string Command { get; } = "stafflist";

        public string[] Aliases { get; } = new[] { "sl" };

        public string Description { get; } = Language.StaffListCommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GameplayData))
            {
                response = string.Format(Language.NotEnoughPermissions, PlayerPermissions.GameplayData);
                return false;
            }

            StringBuilder message = StringBuilderPool.Shared.Rent();

            foreach (Player player in Player.List)
            {
                if (player.RemoteAdminAccess)
                    message.Append(player.Nickname).Append(" - ").Append(player?.GroupName).AppendLine();
            }

            if (message.Length == 0)
                message.Append(Language.NoStaffOnline);

            response = message.ToString();

            StringBuilderPool.Shared.Return(message);

            return true;
        }
    }
}
