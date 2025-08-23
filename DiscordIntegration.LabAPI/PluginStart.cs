// -----------------------------------------------------------------------
// <copyright file="DiscordIntegration.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace DiscordIntegration
{
    using API;
    using Events;
    using global::DiscordIntegration.Dependency;
    using HarmonyLib;
    using LabApi.Events.CustomHandlers;
    using LabApi.Features.Wrappers;
    using LabApi.Loader.Features.Plugins;
    using System;
    using System.Reflection;
    using System.Threading;
    using UnityEngine;
    using Logger = LabApi.Features.Console.Logger;

    /// <summary>
    /// Link a Discord server with an SCP: SL server.
    /// </summary>
    public class PluginStart : Plugin<Config>
    {
        private MapHandler mapHandler;

        private ServerHandler serverHandler;

        private PlayerHandler playerHandler;

        private NetworkHandler networkHandler;

        private Harmony harmony;

        private int slots;

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
        /// Gets the <see cref="PluginStart"/> instance.
        /// </summary>
        public static PluginStart Instance;

        /// <summary>
        /// Gets the server slots.
        /// </summary>
        public int Slots
        {
            get
            {
                if (Server.MaxPlayers > 0)
                    slots = Server.MaxPlayers;
                return slots;
            }
        }

        public override string Name => "DiscordIntegration";

        public override string Description => "Discord Bot";

        public override string Author => "Exiled-Team. Forked by Yti890";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public override Version RequiredApiVersion => new Version(1, 1, 0, 0);

        public override void Enable()
        {
            Instance = this;

            try
            {
                harmony = new Harmony($"com.joker.DI-{DateTime.Now.Ticks}");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            if (Config?.Bot == null)
            {
                Logger.Error("PluginStart.Config.Bot is null. Check the config file.");
                return;
            }

            Language = new Language();
            Network = new Network(Config.Bot.IPAddress, Config.Bot.Port, TimeSpan.FromSeconds(Config.Bot.ReconnectionInterval));

            NetworkCancellationTokenSource = new CancellationTokenSource();

            Language.Save(true);
            Language.Load();

            RegisterEvents();

            API.Configs.Bot.UpdateActivityCancellationTokenSource = new CancellationTokenSource();
            API.Configs.Bot.UpdateChannelsTopicCancellationTokenSource = new CancellationTokenSource();

            _ = Network.Start(NetworkCancellationTokenSource);

            _ = API.Configs.Bot.UpdateActivity(API.Configs.Bot.UpdateActivityCancellationTokenSource.Token);
            _ = API.Configs.Bot.UpdateChannelsTopic(API.Configs.Bot.UpdateChannelsTopicCancellationTokenSource.Token);
        }

        public override void Disable()
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
        }

        private void RegisterEvents()
        {
            mapHandler = new MapHandler();
            serverHandler = new ServerHandler();
            playerHandler = new PlayerHandler();
            networkHandler = new NetworkHandler();

            CustomHandlersManager.RegisterEventsHandler(mapHandler);
            CustomHandlersManager.RegisterEventsHandler(serverHandler);
            CustomHandlersManager.RegisterEventsHandler(playerHandler);

            Network.SendingError += networkHandler.OnSendingError;
            Network.ReceivingError += networkHandler.OnReceivingError;
            Network.UpdatingConnectionError += networkHandler.OnUpdatingConnectionError;
            Network.ConnectingError += networkHandler.OnConnectingError;
            Network.Connected += networkHandler.OnConnected;
            Network.Connecting += networkHandler.OnConnecting;
            Network.ReceivedFull += networkHandler.OnReceivedFull;
            Network.Sent += networkHandler.OnSent;
            Network.Terminated += networkHandler.OnTerminated;

            Application.logMessageReceived += HandleLog;
        }

        private void UnregisterEvents()
        {
            CustomHandlersManager.UnregisterEventsHandler(mapHandler);
            CustomHandlersManager.UnregisterEventsHandler(playerHandler);
            CustomHandlersManager.UnregisterEventsHandler(playerHandler);

            Network.SendingError -= networkHandler.OnSendingError;
            Network.ReceivingError -= networkHandler.OnReceivingError;
            Network.UpdatingConnectionError -= networkHandler.OnUpdatingConnectionError;
            Network.ConnectingError -= networkHandler.OnConnectingError;
            Network.Connected -= networkHandler.OnConnected;
            Network.Connecting -= networkHandler.OnConnecting;
            Network.ReceivedFull -= networkHandler.OnReceivedFull;
            Network.Sent -= networkHandler.OnSent;
            Network.Terminated -= networkHandler.OnTerminated;

            Application.logMessageReceived -= HandleLog;

            playerHandler = null;
            mapHandler = null;
            serverHandler = null;
            networkHandler = null;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                LogError($"[LogCatcher] Error: {logString}\nStack: {stackTrace}");
            }
        }

        private static void LogError(string message)
        {
            if (PluginStart.Instance.Config.LogErrors)
                _ = PluginStart.Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.Errors, message));
        }
    }
}
