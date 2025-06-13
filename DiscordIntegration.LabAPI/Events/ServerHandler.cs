namespace DiscordIntegration.Events
{
    using Dependency;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Arguments.ServerEvents;
    using LabApi.Events.CustomHandlers;
    using LabApi.Features.Wrappers;
    using static PluginStart;

    internal sealed class ServerHandler : CustomEventsHandler
    {
        public override async void OnPlayerReportedCheater(PlayerReportedCheaterEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ReportingCheater)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Reports, string.Format(Language.CheaterReportFilled, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role, ev.Reason))).ConfigureAwait(false);
            base.OnPlayerReportedCheater(ev);
        }
        public override async void OnPlayerReportedPlayer(PlayerReportedPlayerEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ReportingCheater)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Reports, string.Format(Language.CheaterReportFilled, ev.Player.Nickname, ev.Player.UserId, ev.Player.Role, ev.Target.Nickname, ev.Target.UserId, ev.Target.Role, ev.Reason))).ConfigureAwait(false);
            base.OnPlayerReportedPlayer(ev);
        }
        public override async void OnServerWaitingForPlayers()
        {
            if (Instance.Config.EventsToLog.WaitingForPlayers)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, Language.WaitingForPlayers)).ConfigureAwait(false);
            base.OnServerWaitingForPlayers();
        }
        public override async void OnServerRoundStarted()
        {
            if (Instance.Config.EventsToLog.RoundStarted)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.RoundStarting, Player.Count))).ConfigureAwait(false);
            base.OnServerRoundStarted();
        }
        public override async void OnServerRoundEnded(RoundEndedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.RoundEnded)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.RoundEnded, ev.LeadingTeam, Player.Count, Instance.Slots))).ConfigureAwait(false);
            base.OnServerRoundEnded(ev);
        }
        public override async void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.RespawningTeam)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(ev.Wave.Faction == PlayerRoles.Faction.FoundationEnemy ? Language.ChaosInsurgencyHaveSpawned : Language.NineTailedFoxHaveSpawned, ev.Players.Count))).ConfigureAwait(false);
            base.OnServerWaveRespawned(ev);
        }
    }
}