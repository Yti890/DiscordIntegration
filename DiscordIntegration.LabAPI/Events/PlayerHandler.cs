namespace DiscordIntegration.Events
{
    using Dependency;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Arguments.Scp079Events;
    using LabApi.Events.Arguments.Scp106Events;
    using LabApi.Events.Arguments.Scp914Events;
    using LabApi.Events.CustomHandlers;
    using PlayerRoles;
    using System;
    using static PluginStart;

    internal sealed class PlayerHandler : CustomEventsHandler
    {
        public override async void OnPlayerActivatedGenerator(PlayerActivatedGeneratorEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerInsertingGeneratorTablet && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GeneratorInserted, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerActivatedGenerator(ev);
        }
        public override async void OnPlayerOpeningGenerator(PlayerOpeningGeneratorEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerOpeningGenerator && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GeneratorOpened, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerOpeningGenerator(ev);
        }
        public override async void OnPlayerUnlockedGenerator(PlayerUnlockedGeneratorEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerUnlockingGenerator && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GeneratorUnlocked, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerUnlockedGenerator(ev);
        }
        public async override void OnScp106TeleportingPlayer(Scp106TeleportingPlayerEvent ev)
        {
            if (Instance.Config.EventsToLog.CreatingScp106Portal && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.Scp106CreatedPortal, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnScp106TeleportingPlayer(ev);
        }
        public override async void OnPlayerChangedItem(PlayerChangedItemEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ChangingPlayerItem && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.ItemChanged, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.CurrentItem.Type, ev.OldItem.Type))).ConfigureAwait(false);
            base.OnPlayerChangedItem(ev);
        }
        public override async void OnScp079GainedExperience(Scp079GainedExperienceEventArgs ev)
        {
            if (Instance.Config.EventsToLog.GainingScp079Experience && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GainedExperience, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.Amount, ev.Reason))).ConfigureAwait(false);
            base.OnScp079GainedExperience(ev);
        }
        public override async void OnScp079LeveledUp(Scp079LeveledUpEventArgs ev)
        {
            if (Instance.Config.EventsToLog.GainingScp079Level && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GainedLevel, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.Tier - 1, ev.Tier))).ConfigureAwait(false);
            base.OnScp079LeveledUp(ev);
        }
        public override async void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerLeft && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.LeftServer, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerLeft(ev);
        }
        public override async void OnPlayerReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ReloadingPlayerWeapon && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.Reloaded, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.CurrentItem.Type, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerReloadingWeapon(ev);
        }
        public override async void OnPlayerUnlockedWarheadButton(PlayerUnlockedWarheadButtonEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerActivatingWarheadPanel && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.AccessedWarhead, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerUnlockedWarheadButton(ev);
        }
        public override async void OnPlayerInteractedElevator(PlayerInteractedElevatorEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerInteractingElevator && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.CalledElevator, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerInteractedElevator(ev);
        }
        public override async void OnPlayerInteractedLocker(PlayerInteractedLockerEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerInteractingLocker && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.UsedLocker, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerInteractedLocker(ev);
        }
        public override async void OnPlayerTriggeredTesla(PlayerTriggeredTeslaEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerTriggeringTesla && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasTriggeredATeslaGate, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerTriggeredTesla(ev);
        }
        public override async void OnPlayerClosingGenerator(PlayerClosingGeneratorEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerClosingGenerator && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GeneratorClosed, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerClosingGenerator(ev);
        }
        public override async void OnPlayerDeactivatedGenerator(PlayerDeactivatedGeneratorEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerEjectingGeneratorTablet && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GeneratorEjected, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerDeactivatedGenerator(ev);
        }
        public override async void OnPlayerInteractedDoor(PlayerInteractedDoorEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerInteractingDoor && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(ev.Door.IsOpened ? Language.HasClosedADoor : Language.HasOpenedADoor, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.Door.NameTag))).ConfigureAwait(false);
            base.OnPlayerInteractedDoor(ev);
        }
        public override async void OnScp914Activated(Scp914ActivatedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ActivatingScp914 && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.Scp914HasBeenActivated, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.KnobSetting))).ConfigureAwait(false);
            base.OnScp914Activated(ev);
        }
        public override async void OnScp914KnobChanged(Scp914KnobChangedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.ChangingScp914KnobSetting && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.Scp914KnobSettingChanged, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.KnobSetting))).ConfigureAwait(false);
            base.OnScp914KnobChanged(ev);
        }
        public override async void OnPlayerEnteredPocketDimension(PlayerEnteredPocketDimensionEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerEnteringPocketDimension && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasEnteredPocketDimension, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerEnteredPocketDimension(ev);
        }
        public override async void OnPlayerEscaped(PlayerEscapedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerEscapingPocketDimension && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasEscapedPocketDimension, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerEscaped(ev);
        }
        public override async void OnScp079UsedTesla(Scp079UsedTeslaEventArgs ev)
        {
            if (Instance.Config.EventsToLog.Scp079InteractingTesla && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasTriggeredATeslaGate, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnScp079UsedTesla(ev);
        }
        public override async void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (Instance.Config.EventsToLog.HurtingPlayer && ev.Player != null && (ev.Attacker == null || !Instance.Config.ShouldLogFriendlyFireDamageOnly || ev.Attacker.Role == ev.Player.Role) && (!Instance.Config.ShouldRespectDoNotTrack || (ev.Attacker == null || (!ev.Attacker.DoNotTrack && !ev.Player.DoNotTrack))) && (!Instance.Config.OnlyLogPlayerDamage || ev.Attacker != null))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasDamagedForWith, ev.Attacker != null ? ev.Attacker.Nickname : "Server", Instance.Config.ShouldLogUserIds ? (ev.Attacker != null ? ev.Attacker.UserId : "") : Language.Redacted, ev.Attacker?.Role ?? RoleTypeId.None, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);

            base.OnPlayerHurting(ev);
        }
        public override async void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerDying && ev.Player != null && (ev.Attacker == null || !Instance.Config.ShouldLogFriendlyFireKillsOnly || ev.Attacker.Role == ev.Player.Role) && (!Instance.Config.ShouldRespectDoNotTrack || (ev.Attacker == null || (!ev.Attacker.DoNotTrack && !ev.Player.DoNotTrack))))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasKilledWith, ev.Attacker != null ? ev.Attacker.Nickname : "Server", Instance.Config.ShouldLogUserIds ? ev.Attacker != null ? ev.Attacker.UserId : string.Empty : Language.Redacted, ev.Player?.Role ?? RoleTypeId.None, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerDeath(ev);
        }
        public override async void OnPlayerThrowingItem(PlayerThrowingItemEventArgs ev)
        {
            if (ev.Player != null && Instance.Config.EventsToLog.PlayerThrowingGrenade && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.ThrewAGrenade, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.Pickup.Type))).ConfigureAwait(false);
            base.OnPlayerThrowingItem(ev);
        }
        public override async void OnPlayerChangingRole(PlayerChangingRoleEventArgs ev)
        {
            if (ev.Player != null && Instance.Config.EventsToLog.ChangingPlayerRole && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.ChangedRole, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.NewRole))).ConfigureAwait(false);
            base.OnPlayerChangingRole(ev);
        }

        public override async void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerJoined && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasJoinedTheGame, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, Instance.Config.ShouldLogIPAddresses ? ev.Player.IpAddress : Language.Redacted))).ConfigureAwait(false);
            base.OnPlayerJoined(ev);
        }
        public override async void OnPlayerUncuffed(PlayerUncuffedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerRemovingHandcuffs && ((!ev.Player.DoNotTrack && !ev.Target.DoNotTrack) || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasBeenFreedBy, ev.Target.Nickname, Instance.Config.ShouldLogUserIds ? ev.Target.UserId : Language.Redacted, ev.Target.Role, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerUncuffed(ev);
        }
        public override async void OnPlayerCuffed(PlayerCuffedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.HandcuffingPlayer && ((!ev.Player.DoNotTrack && !ev.Target.DoNotTrack) || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasBeenHandcuffedBy, ev.Target.Nickname, Instance.Config.ShouldLogUserIds ? ev.Target.UserId : Language.Redacted, ev.Target.Role, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerCuffed(ev);
        }
        public override async void OnPlayerKicked(PlayerKickedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerBanned)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, "kicks", string.Format(Language.WasKicked, ev.Player?.Nickname ?? Language.NotAuthenticated, ev.Player?.UserId ?? Language.NotAuthenticated, ev.Reason))).ConfigureAwait(false);
            base.OnPlayerKicked(ev);
        }
        public override async void OnPlayerBanned(PlayerBannedEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerBanned)
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Bans, string.Format(Language.WasBannedBy, ev.Player.Nickname, ev.Player.PlayerId, ev.Issuer, ev.Reason, new DateTime(ev.Duration).ToString(Instance.Config.DateFormat)))).ConfigureAwait(false);
            base.OnPlayerBanned(ev);
        }
        public override async void OnPlayerUsedIntercom(PlayerUsedIntercomEventArgs ev)
        {
            if (ev.Player != null && Instance.Config.EventsToLog.PlayerIntercomSpeaking && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasStartedUsingTheIntercom, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role))).ConfigureAwait(false);
            base.OnPlayerUsedIntercom(ev);
        }
        public override async void OnPlayerPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerPickingupItem && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasPickedUp, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.Item.Type))).ConfigureAwait(false);
            base.OnPlayerPickedUpItem(ev);
        }
        public override async void OnPlayerDroppedItem(PlayerDroppedItemEventArgs ev)
        {
            if (Instance.Config.EventsToLog.PlayerItemDropped && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.HasDropped, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.Pickup.Type))).ConfigureAwait(false);
            base.OnPlayerDroppedItem(ev);
        }
        public override async void OnPlayerGroupChanged(PlayerGroupChangedEventArgs ev)
        {
            if (ev.Player != null && Instance.Config.EventsToLog.ChangingPlayerGroup && (!ev.Player.DoNotTrack || !Instance.Config.ShouldRespectDoNotTrack))
                await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, string.Format(Language.GroupSet, ev.Player.Nickname, Instance.Config.ShouldLogUserIds ? ev.Player.UserId : Language.Redacted, ev.Player.Role, ev.Group?.BadgeText ?? Language.None, ev.Group?.BadgeColor ?? Language.None))).ConfigureAwait(false);
            base.OnPlayerGroupChanged(ev);
        }
    }
}