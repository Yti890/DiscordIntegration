// -----------------------------------------------------------------------
// <copyright file="DiscordIntegration.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace DiscordIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using API;
    using API.Configs;
    using API.User;
    using Events;
    using Exiled.API.Features;
    using HarmonyLib;
    using MEC;
    using Handlers = Exiled.Events.Handlers;
    using Version = System.Version;

    /// <summary>
    /// Link a Discord server with an SCP: SL server.
    /// </summary>
    public class DiscordIntegration : Plugin<Config>
    {
        private static readonly DiscordIntegration InstanceValue = new();

        private MapHandler mapHandler;

        private ServerHandler serverHandler;

        private PlayerHandler playerHandler;

        private NetworkHandler networkHandler;

        private Harmony harmony;

        private int slots;

        private DiscordIntegration()
        {
        }

        /// <summary>
        /// Gets the plugin <see cref="Language"/> instance.
        /// </summary>
        public static Language Language { get; private set; }

        /// <summary>
        /// Gets the <see cref="API.Network"/> instance.
        /// </summary>
        public static Network Network { get; private set; }

        /// <summary>
        /// Gets or sets the network <see cref="CancellationTokenSource"/> instance.
        /// </summary>
        public static CancellationTokenSource NetworkCancellationTokenSource { get; internal set; }

        /// <summary>
        /// Gets the <see cref="DiscordIntegration"/> instance.
        /// </summary>
        public static DiscordIntegration Instance => InstanceValue;

        /// <summary>
        /// Gets the server slots.
        /// </summary>
        public int Slots
        {
            get
            {
                if (Server.MaxPlayerCount > 0)
                    slots = Server.MaxPlayerCount;
                return slots;
            }
        }

        /// <summary>
        /// Fired when the plugin is enabled.
        /// </summary>
        public override void OnEnabled()
        {
            try
            {
                harmony = new Harmony($"com.joker.DI-{DateTime.Now.Ticks}");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            Language = new Language();
            Network = new Network(Instance.Config.Bot.IPAddress, Instance.Config.Bot.Port, TimeSpan.FromSeconds(Instance.Config.Bot.ReconnectionInterval));

            NetworkCancellationTokenSource = new CancellationTokenSource();

            Language.Save();
            Language.Load();

            RegisterEvents();

            API.Configs.Bot.UpdateActivityCancellationTokenSource = new CancellationTokenSource();
            API.Configs.Bot.UpdateChannelsTopicCancellationTokenSource = new CancellationTokenSource();

            _ = Network.Start(NetworkCancellationTokenSource);

            _ = API.Configs.Bot.UpdateActivity(API.Configs.Bot.UpdateActivityCancellationTokenSource.Token);
            _ = API.Configs.Bot.UpdateChannelsTopic(API.Configs.Bot.UpdateChannelsTopicCancellationTokenSource.Token);

            base.OnEnabled();
        }

        /// <summary>
        /// Fired when the plugin is disabled.
        /// </summary>
        public override void OnDisabled()
        {
            harmony?.UnpatchAll(harmony.Id);
            harmony = null;

            NetworkCancellationTokenSource.Cancel();
            NetworkCancellationTokenSource.Dispose();

            Network.Close();

            API.Configs.Bot.UpdateActivityCancellationTokenSource.Cancel();
            API.Configs.Bot.UpdateActivityCancellationTokenSource.Dispose();

            API.Configs.Bot.UpdateChannelsTopicCancellationTokenSource.Cancel();
            API.Configs.Bot.UpdateChannelsTopicCancellationTokenSource.Dispose();

            UnregisterEvents();

            Language = null;
            Network = null;

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            mapHandler = new MapHandler();
            serverHandler = new ServerHandler();
            playerHandler = new PlayerHandler();
            networkHandler = new NetworkHandler();

            Handlers.Map.Decontaminating += mapHandler.OnDecontaminating;
            Handlers.Map.GeneratorActivating += mapHandler.OnGeneratorActivated;
            Handlers.Warhead.Starting += mapHandler.OnStartingWarhead;
            Handlers.Warhead.Stopping += mapHandler.OnStoppingWarhead;
            Handlers.Warhead.Detonated += mapHandler.OnWarheadDetonated;
            Handlers.Scp914.UpgradingInventoryItem += mapHandler.OnUpgradingItems;

            Handlers.Server.WaitingForPlayers += serverHandler.OnWaitingForPlayers;
            Handlers.Server.RoundStarted += serverHandler.OnRoundStarted;
            Handlers.Server.RoundEnded += serverHandler.OnRoundEnded;
            Handlers.Server.RespawningTeam += serverHandler.OnRespawningTeam;
            Handlers.Server.ReportingCheater += serverHandler.OnReportingCheater;
            Handlers.Server.LocalReporting += serverHandler.OnLocalReporting;

            Handlers.Scp914.ChangingKnobSetting += playerHandler.OnChangingScp914KnobSetting;
            Handlers.Player.UsedItem += playerHandler.OnUsedMedicalItem;
            Handlers.Scp079.InteractingTesla += playerHandler.OnInteractingTesla;
            Handlers.Player.PickingUpItem += playerHandler.OnPickingUpItem;
            Handlers.Player.ActivatingGenerator += playerHandler.OnInsertingGeneratorTablet;
            Handlers.Player.StoppingGenerator += playerHandler.OnEjectingGeneratorTablet;
            Handlers.Player.UnlockingGenerator += playerHandler.OnUnlockingGenerator;
            Handlers.Player.OpeningGenerator += playerHandler.OnOpeningGenerator;
            Handlers.Player.ClosingGenerator += playerHandler.OnClosingGenerator;
            Handlers.Scp079.GainingLevel += playerHandler.OnGainingLevel;
            Handlers.Scp079.GainingExperience += playerHandler.OnGainingExperience;
            Handlers.Player.EscapingPocketDimension += playerHandler.OnEscapingPocketDimension;
            Handlers.Player.EnteringPocketDimension += playerHandler.OnEnteringPocketDimension;
            Handlers.Scp106.Teleporting += playerHandler.OnCreatingPortal;
            Handlers.Player.ActivatingWarheadPanel += playerHandler.OnActivatingWarheadPanel;
            Handlers.Player.TriggeringTesla += playerHandler.OnTriggeringTesla;
            Handlers.Player.ThrowingRequest += playerHandler.OnThrowingGrenade;
            Handlers.Player.Hurting += playerHandler.OnHurting;
            Handlers.Player.Dying += playerHandler.OnDying;
            Handlers.Player.Kicked += playerHandler.OnKicked;
            Handlers.Player.Banned += playerHandler.OnBanned;
            Handlers.Player.InteractingDoor += playerHandler.OnInteractingDoor;
            Handlers.Player.InteractingElevator += playerHandler.OnInteractingElevator;
            Handlers.Player.InteractingLocker += playerHandler.OnInteractingLocker;
            Handlers.Player.IntercomSpeaking += playerHandler.OnIntercomSpeaking;
            Handlers.Player.Handcuffing += playerHandler.OnHandcuffing;
            Handlers.Player.RemovingHandcuffs += playerHandler.OnRemovingHandcuffs;
            Handlers.Scp106.Teleporting += playerHandler.OnTeleporting;
            Handlers.Player.ReloadingWeapon += playerHandler.OnReloadingWeapon;
            Handlers.Player.DroppingItem += playerHandler.OnItemDropped;
            Handlers.Player.Verified += playerHandler.OnVerified;
            Handlers.Player.Destroying += playerHandler.OnDestroying;
            Handlers.Player.ChangingRole += playerHandler.OnChangingRole;
            Handlers.Player.ChangingGroup += playerHandler.OnChangingGroup;
            Handlers.Player.ChangingItem += playerHandler.OnChangingItem;
            Handlers.Scp914.Activating += playerHandler.OnActivatingScp914;
            Handlers.Map.AnnouncingChaosEntrance += mapHandler.OnAnnounceCI;
            Handlers.Map.AnnouncingNtfEntrance += mapHandler.OnAnnounceMTF;
            Handlers.Map.TurningOffLights += mapHandler.OnTurningOffLights;
            Handlers.Player.MicroHIDOpeningDoor += playerHandler.OnOpenMicroHID;

            Network.SendingError += networkHandler.OnSendingError;
            Network.ReceivingError += networkHandler.OnReceivingError;
            Network.UpdatingConnectionError += networkHandler.OnUpdatingConnectionError;
            Network.ConnectingError += networkHandler.OnConnectingError;
            Network.Connected += networkHandler.OnConnected;
            Network.Connecting += networkHandler.OnConnecting;
            Network.ReceivedFull += networkHandler.OnReceivedFull;
            Network.Sent += networkHandler.OnSent;
            Network.Terminated += networkHandler.OnTerminated;
        }

        private void UnregisterEvents()
        {
            Handlers.Map.Decontaminating -= mapHandler.OnDecontaminating;
            Handlers.Map.GeneratorActivating -= mapHandler.OnGeneratorActivated;
            Handlers.Warhead.Starting -= mapHandler.OnStartingWarhead;
            Handlers.Warhead.Stopping -= mapHandler.OnStoppingWarhead;
            Handlers.Warhead.Detonated -= mapHandler.OnWarheadDetonated;
            Handlers.Scp914.UpgradingInventoryItem -= mapHandler.OnUpgradingItems;

            Handlers.Server.WaitingForPlayers -= serverHandler.OnWaitingForPlayers;
            Handlers.Server.RoundStarted -= serverHandler.OnRoundStarted;
            Handlers.Server.RoundEnded -= serverHandler.OnRoundEnded;
            Handlers.Server.RespawningTeam -= serverHandler.OnRespawningTeam;
            Handlers.Server.ReportingCheater -= serverHandler.OnReportingCheater;
            Handlers.Server.LocalReporting -= serverHandler.OnLocalReporting;

            Handlers.Scp914.ChangingKnobSetting -= playerHandler.OnChangingScp914KnobSetting;
            Handlers.Player.UsedItem -= playerHandler.OnUsedMedicalItem;
            Handlers.Scp079.InteractingTesla -= playerHandler.OnInteractingTesla;
            Handlers.Player.PickingUpItem -= playerHandler.OnPickingUpItem;
            Handlers.Player.ActivatingGenerator -= playerHandler.OnInsertingGeneratorTablet;
            Handlers.Player.StoppingGenerator -= playerHandler.OnEjectingGeneratorTablet;
            Handlers.Player.UnlockingGenerator -= playerHandler.OnUnlockingGenerator;
            Handlers.Player.OpeningGenerator -= playerHandler.OnOpeningGenerator;
            Handlers.Player.ClosingGenerator -= playerHandler.OnClosingGenerator;
            Handlers.Scp079.GainingLevel -= playerHandler.OnGainingLevel;
            Handlers.Scp079.GainingExperience -= playerHandler.OnGainingExperience;
            Handlers.Player.EscapingPocketDimension -= playerHandler.OnEscapingPocketDimension;
            Handlers.Player.EnteringPocketDimension -= playerHandler.OnEnteringPocketDimension;
            Handlers.Scp106.Teleporting -= playerHandler.OnCreatingPortal;
            Handlers.Player.ActivatingWarheadPanel -= playerHandler.OnActivatingWarheadPanel;
            Handlers.Player.TriggeringTesla -= playerHandler.OnTriggeringTesla;
            Handlers.Player.ThrowingRequest -= playerHandler.OnThrowingGrenade;
            Handlers.Player.Hurting -= playerHandler.OnHurting;
            Handlers.Player.Dying -= playerHandler.OnDying;
            Handlers.Player.Kicked -= playerHandler.OnKicked;
            Handlers.Player.Banned -= playerHandler.OnBanned;
            Handlers.Player.InteractingDoor -= playerHandler.OnInteractingDoor;
            Handlers.Player.InteractingElevator -= playerHandler.OnInteractingElevator;
            Handlers.Player.InteractingLocker -= playerHandler.OnInteractingLocker;
            Handlers.Player.IntercomSpeaking -= playerHandler.OnIntercomSpeaking;
            Handlers.Player.Handcuffing -= playerHandler.OnHandcuffing;
            Handlers.Player.RemovingHandcuffs -= playerHandler.OnRemovingHandcuffs;
            Handlers.Scp106.Teleporting -= playerHandler.OnTeleporting;
            Handlers.Player.ReloadingWeapon -= playerHandler.OnReloadingWeapon;
            Handlers.Player.DroppingItem -= playerHandler.OnItemDropped;
            Handlers.Player.Verified -= playerHandler.OnVerified;
            Handlers.Player.Destroying -= playerHandler.OnDestroying;
            Handlers.Player.ChangingRole -= playerHandler.OnChangingRole;
            Handlers.Player.ChangingGroup -= playerHandler.OnChangingGroup;
            Handlers.Player.ChangingItem -= playerHandler.OnChangingItem;
            Handlers.Scp914.Activating -= playerHandler.OnActivatingScp914;
            Handlers.Map.AnnouncingChaosEntrance -= mapHandler.OnAnnounceCI;
            Handlers.Map.AnnouncingNtfEntrance -= mapHandler.OnAnnounceMTF;
            Handlers.Map.TurningOffLights -= mapHandler.OnTurningOffLights;
            Handlers.Player.MicroHIDOpeningDoor -= playerHandler.OnOpenMicroHID;

            Network.SendingError -= networkHandler.OnSendingError;
            Network.ReceivingError -= networkHandler.OnReceivingError;
            Network.UpdatingConnectionError -= networkHandler.OnUpdatingConnectionError;
            Network.ConnectingError -= networkHandler.OnConnectingError;
            Network.Connected -= networkHandler.OnConnected;
            Network.Connecting -= networkHandler.OnConnecting;
            Network.ReceivedFull -= networkHandler.OnReceivedFull;
            Network.Sent -= networkHandler.OnSent;
            Network.Terminated -= networkHandler.OnTerminated;

            playerHandler = null;
            mapHandler = null;
            serverHandler = null;
            networkHandler = null;
        }
    }
}
