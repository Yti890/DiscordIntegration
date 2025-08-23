namespace DiscordIntegration.Events
{
    using Dependency;
    using LabApi.Events.Arguments.Scp914Events;
    using LabApi.Events.Arguments.ServerEvents;
    using LabApi.Events.Arguments.WarheadEvents;
    using LabApi.Events.CustomHandlers;
    using LabApi.Features.Wrappers;
    using System;
    using static PluginStart;

    internal sealed class MapHandler : CustomEventsHandler
    {
        public static int GeneratorCount;
        public override async void OnWarheadDetonated(WarheadDetonatedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.WarheadDetonated)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, Language.WarheadHasDetonated)).ConfigureAwait(false);
            base.OnWarheadDetonated(ev);
        }
        public override async void OnServerGeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.GeneratorActivated)
                GeneratorCount++;
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GeneratorFinished, ev.Generator.Room, GeneratorCount))).ConfigureAwait(false);
            base.OnServerGeneratorActivated(ev);
        }
        public override async void OnServerLczDecontaminationStarting(LczDecontaminationStartingEventArgs ev)
        {
            if (Instance.Config.EventsToLog.Decontaminating)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, Language.DecontaminationHasBegun)).ConfigureAwait(false);
            base.OnServerLczDecontaminationStarting(ev);
        }
        public override async void OnWarheadStarting(WarheadStartingEventArgs ev)
        {
            if (Instance.Config.EventsToLog.StartingWarhead && (ev.Player == null || (ev.Player != null && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))))
            {
                object[] vars = ev.Player == null ?
                    new object[] { Warhead.DetonationTime } :
                    new object[] { ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, Warhead.DetonationTime };

                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(ev.Player == null ? Language.WarheadStarted : Language.PlayerWarheadStarted, vars))).ConfigureAwait(false);
            }
            base.OnWarheadStarting(ev);
        }
        public override async void OnWarheadStopping(WarheadStoppingEventArgs ev)
        {
            if (Instance.Config.EventsToLog.StoppingWarhead && (ev.Player == null || (ev.Player != null && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))))
            {
                object[] vars = ev.Player == null ?
                    Array.Empty<object>() :
                    new object[] { ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role };

                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(ev.Player == null ? Language.CanceledWarhead : Language.PlayerCanceledWarhead, vars))).ConfigureAwait(false);
            }
            base.OnWarheadStopping(ev);
        }
        public override async void OnScp914ProcessedPickup(Scp914ProcessedPickupEventArgs ev)
        {
            if (Instance.Config.EventsToLog.UpgradingScp914Items)
            {
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.Scp914ProcessedItem, ev.Pickup.Type)));
            }
            base.OnScp914ProcessedPickup(ev);
        }
    }
}