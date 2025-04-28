// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace DiscordIntegration.Events
{
    using API.Commands;
    using Dependency;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Respawning;
    using static DiscordIntegration;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal sealed class ServerHandler
    {
#pragma warning disable SA1600 // Elements should be documented

        public async void OnReportingCheater(Exiled.Events.EventArgs.Server.ReportingCheaterEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ReportingCheater)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Reports, string.Format(Language.CheaterReportFilled, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role, ev.Reason))).ConfigureAwait(false);
        }

        public async void OnLocalReporting(Exiled.Events.EventArgs.Server.LocalReportingEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ReportingCheater)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Reports, string.Format(Language.CheaterReportFilled, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role, ev.Reason))).ConfigureAwait(false);
        }

        public async void OnWaitingForPlayers()
        {
            if (Instance.Config.EventsToLog.WaitingForPlayers)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, Language.WaitingForPlayers)).ConfigureAwait(false);
            if (Instance.Config.StaffOnlyEventsToLog.WaitingForPlayers)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.StaffCopy, Language.WaitingForPlayers)).ConfigureAwait(false);
        }

        public async void OnRoundStarted()
        {
            if (Instance.Config.EventsToLog.RoundStarted)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.RoundStarting, Player.Dictionary.Count))).ConfigureAwait(false);
        }

        public async void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.RoundEnded)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.RoundEnded, ev.LeadingTeam, Player.Dictionary.Count, Instance.Slots))).ConfigureAwait(false);
        }

        public async void OnRespawningTeam(Exiled.Events.EventArgs.Server.RespawningTeamEventArgs ev)
        {
            if (Instance.Config.EventsToLog.RespawningTeam)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(ev.NextKnownTeam == PlayerRoles.Faction.FoundationEnemy ? Language.ChaosInsurgencyHaveSpawned : Language.NineTailedFoxHaveSpawned, ev.Players.Count))).ConfigureAwait(false);
        }

        public async void Command(Exiled.Events.EventArgs.Player.SendingValidCommandEventArgs ev)
        {
            string message = string.Format(
                DiscordIntegration.Language.UsedCommand,
                ev.Player.Nickname,
                ev.Player.UserId ?? DiscordIntegration.Language.DedicatedServer,
                ev.Player.Role,
                ev.Command.Command,
                ev.Command.Aliases);

            if (DiscordIntegration.Instance.Config.EventsToLog.SendingRemoteAdminCommands)
                _ = DiscordIntegration.Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Command, message));

            if (DiscordIntegration.Instance.Config.StaffOnlyEventsToLog.SendingRemoteAdminCommands)
                _ = DiscordIntegration.Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.StaffCopy, message));
        }
    }
}