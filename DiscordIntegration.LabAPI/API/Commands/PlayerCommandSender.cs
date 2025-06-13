// -----------------------------------------------------------------------
// <copyright file="PlayerCommandSender.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace DiscordIntegration.API.Commands
{
    using LabApi.Features.Console;
    using LabApi.Features.Wrappers;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a compatible and JSON serializable <see cref="RemoteAdmin.PlayerCommandSender"/>.
    /// </summary>
    public class PlayerCommandSender : CommandSender
    {
        private readonly Player player;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCommandSender"/> class.
        /// </summary>
        /// <param name="senderId"><inheritdoc cref="SenderId"/></param>
        /// <param name="nickname"><inheritdoc cref="Nickname"/></param>
        /// <param name="permissions"><inheritdoc cref="Permissions"/></param>
        /// <param name="kickPower"><inheritdoc cref="KickPower"/></param>
        /// <param name="fullPermissions"><inheritdoc cref="FullPermissions"/></param>
        [JsonConstructor]
        public PlayerCommandSender(string senderId, string nickname, ulong permissions, byte kickPower, bool fullPermissions)
        {
            SenderId = senderId;
            Nickname = nickname;
            Permissions = permissions;
            KickPower = kickPower;
            FullPermissions = fullPermissions;

            player = Player.Get(SenderId);
        }

        /// <inheritdoc/>
        public override string SenderId { get; }

        /// <inheritdoc/>
        public override string Nickname { get; }

        /// <inheritdoc/>
        public override ulong Permissions { get; }

        /// <inheritdoc/>
        public override byte KickPower { get; }

        /// <inheritdoc/>
        public override bool FullPermissions { get; }

        /// <inheritdoc/>
        public override void Print(string text) => Logger.Info(text);

        /// <inheritdoc/>
        public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay) => player.SendConsoleMessage($"DISCORDINTEGRATION#{text}, {success}, {logToConsole}, {overrideDisplay}");

        public override bool Available()
        {
            return true;
        }
    }
}