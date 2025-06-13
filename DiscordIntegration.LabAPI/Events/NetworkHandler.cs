// -----------------------------------------------------------------------
// <copyright file="NetworkHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace DiscordIntegration.Events
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using API.EventArgs.Network;
    using API.User;
    using Dependency;
    using global::DiscordIntegration.API.Commands;
    using LabApi.Features.Console;
    using LabApi.Features.Wrappers;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Threading;
    using static PluginStart;

    /// <summary>
    /// Handles network-related events.
    /// </summary>
    internal sealed class NetworkHandler
    {
        /// <inheritdoc cref="API.Network.OnReceivedFull(object, ReceivedFullEventArgs)"/>
        public void OnReceivedFull(object _, ReceivedFullEventArgs ev)
        {
            try
            {
                Logger.Debug($"[NET] {string.Format(Language.ReceivedData, ev.Data, ev.Length)}", Instance.Config.Debug);
                if (ev.Data.Contains("heartbeat"))
                    return;

                RemoteCommand remoteCommand = JsonConvert.DeserializeObject<RemoteCommand>(ev.Data, Network.JsonSerializerSettings);

                Logger.Debug($"[NET] {string.Format(Language.HandlingRemoteCommand, remoteCommand.Action, remoteCommand.Parameters[0], Network.TcpClient?.Client?.RemoteEndPoint)}", Instance.Config.Debug);

                switch (remoteCommand.Action)
                {
                    case ActionType.ExecuteCommand:
                        GameCommand command = new GameCommand(remoteCommand.Parameters[0].ToString(), remoteCommand.Parameters[1].ToString(), new DiscordUser(remoteCommand.Parameters[2].ToString(), remoteCommand.Parameters[3].ToString(), remoteCommand.Parameters[1].ToString()));
                        command?.Execute();
                        break;
                    case ActionType.CommandReply:
                        JsonConvert.DeserializeObject<CommandReply>(remoteCommand.Parameters[0].ToString(), Network.JsonSerializerSettings)?.Answer();
                        break;
                    case ActionType.AdminMessage:
                        foreach (var plr in Player.List)
                        {
                            foreach (var group in Instance.Config.Ranks)
                            {
                                if (plr.UserGroup.Name == group)
                                {
                                    plr.SendConsoleMessage(remoteCommand.Parameters[0].ToString(), UnityEngine.Color.green.ToString());
                                    plr.SendBroadcast(remoteCommand.Parameters[0].ToString(), 10, global::Broadcast.BroadcastFlags.Normal, false);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error($"[NET] {string.Format(Language.HandlingRemoteCommandError, Instance.Config.Debug ? exception.ToString() : exception.Message)}");
            }
        }

        /// <inheritdoc cref="API.Network.OnSendingError(object, SendingErrorEventArgs)"/>
        public void OnSendingError(object _, SendingErrorEventArgs ev)
        {
            Logger.Error($"[NET] {string.Format(Language.SendingDataError, Instance.Config.Debug ? ev.Exception.ToString() : ev.Exception.Message)}");
        }

        /// <inheritdoc cref="API.Network.OnReceivingError(object, ReceivingErrorEventArgs)"/>
        public void OnReceivingError(object _, ReceivingErrorEventArgs ev)
        {
            Logger.Error($"[NET] {string.Format(Language.ReceivingDataError, Instance.Config.Debug ? ev.Exception.ToString() : ev.Exception.Message)}");
        }

        /// <inheritdoc cref="API.Network.OnSent(object, SentEventArgs)"/>
        public void OnSent(object _, SentEventArgs ev) => Logger.Debug(string.Format(Language.SentData, ev.Data, ev.Length), Instance.Config.Debug);

        /// <inheritdoc cref="API.Network.OnConnecting(object, ConnectingEventArgs)"/>
        public void OnConnecting(object _, ConnectingEventArgs ev)
        {
            if (!IPAddress.TryParse(Instance.Config?.Bot?.IPAddress, out IPAddress ipAddress))
            {
                Logger.Error($"[NET] {string.Format(Language.InvalidIPAddress, Instance.Config?.Bot?.IPAddress)}");
                return;
            }

            ev.IPAddress = ipAddress;
            ev.Port = Instance.Config.Bot.Port;
            ev.ReconnectionInterval = TimeSpan.FromSeconds(Instance.Config.Bot.ReconnectionInterval);

            Logger.Warn($"[NET] {string.Format(Language.ConnectingTo, ev.IPAddress, ev.Port)}");
        }

        /// <inheritdoc cref="API.Network.OnConnected(object, System.EventArgs)"/>
        public async void OnConnected(object _, System.EventArgs ev)
        {
            Logger.Info($"[NET] {string.Format(Language.SuccessfullyConnected, Network.IPEndPoint?.Address, Network.IPEndPoint?.Port)}");

            await Network.SendAsync(new RemoteCommand(ActionType.Log, ChannelType.GameEvents, Language.ServerConnected));
        }

        /// <inheritdoc cref="API.Network.OnConnectingError(object, ConnectingErrorEventArgs)"/>
        public void OnConnectingError(object _, ConnectingErrorEventArgs ev)
        {
            Logger.Error($"[NET] {string.Format(Language.ConnectingError, Instance.Config.Debug ? ev.Exception.ToString() : ev.Exception.Message)}");
        }

        /// <inheritdoc cref="API.Network.OnConnectingError(object, ConnectingErrorEventArgs)"/>
        public void OnUpdatingConnectionError(object _, UpdatingConnectionErrorEventArgs ev)
        {
            Logger.Error($"[NET] {string.Format(Language.UpdatingConnectionError, Instance.Config.Debug ? ev.Exception.ToString() : ev.Exception.Message)}");
        }

        /// <inheritdoc cref="API.Network.OnTerminated(object, TerminatedEventArgs)"/>
        public void OnTerminated(object _, TerminatedEventArgs ev)
        {
            if (ev.Task.IsFaulted)
                Logger.Error($"[NET] {string.Format(Language.ServerHasBeenTerminatedWithErrors, Instance.Config.Debug ? ev.Task.Exception.ToString() : ev.Task.Exception.Message)}");
            else
                Logger.Warn($"[NET] {Language.ServerHasBeenTerminated}");

            NetworkCancellationTokenSource.Cancel();
            NetworkCancellationTokenSource.Dispose();

            NetworkCancellationTokenSource = new CancellationTokenSource();
            _ = Network.Start(NetworkCancellationTokenSource);
        }
    }
}
