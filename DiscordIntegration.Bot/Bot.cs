using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordIntegration.API.EventArgs.Network;
using DiscordIntegration.Bot.Commands;
using DiscordIntegration.Bot.ConfigObjects;
using DiscordIntegration.Bot.Services;
using DiscordIntegration.Dependency;
using Newtonsoft.Json;

using ActionType = DiscordIntegration.Dependency.ActionType;
using ChannelType = DiscordIntegration.Dependency.ChannelType;

namespace DiscordIntegration.Bot;

public class Bot
{
    private DiscordSocketClient? client;
    private SocketGuild? guild;
    private readonly string token;
    private int lastCount = -1, lastTotal;

    public ushort ServerNumber { get; }
    public TcpServer Server { get; private set; } = null!;
    public InteractionService InteractionService { get; private set; } = null!;
    public SlashCommandHandler CommandHandler { get; private set; } = null!;
    public Dictionary<LogChannel, string> Messages { get; } = new();
    public DiscordSocketClient Client => client ??= new DiscordSocketClient();
    public SocketGuild Guild => guild ??= Client.GetGuild(Program.Config.DiscordServerIds[ServerNumber]);

    public Bot(ushort port, string token)
    {
        ServerNumber = port;
        this.token = token;
        Task.Run(Init);
    }

    public void Destroy() => Client.LogoutAsync();

    private async Task Init()
    {
        if (!ValidateToken()) return;

        SetupDiscord();
        await RegisterCommands();

        Server = new TcpServer(Program.Config.TcpServers[ServerNumber].IpAddress, Program.Config.TcpServers[ServerNumber].Port, this);
        _ = Server.Start();
        Server.ReceivedFull += OnReceived;

        await DequeueMessages();
        await Task.Delay(-1);
    }

    private bool ValidateToken()
    {
        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, token);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(ServerNumber, nameof(ValidateToken), e);
            return false;
        }
    }

    private void SetupDiscord()
    {
        InteractionService = new(Client, new InteractionServiceConfig { AutoServiceScopes = false });
        CommandHandler = new(InteractionService, Client, this);
        InteractionService.Log += SendLog;
        Client.Log += SendLog;
    }

    private async Task RegisterCommands()
    {
        await CommandHandler.InstallCommandsAsync();
        Client.Ready += async () =>
        {
            int count = (await InteractionService.RegisterCommandsToGuildAsync(Guild.Id)).Count;
            Log.Debug(ServerNumber, nameof(RegisterCommands), $"{count} slash commands registered.");
        };

        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
    }

    private Task SendLog(LogMessage arg) => Log.Send(ServerNumber, arg);

    private async void OnReceived(object? sender, ReceivedFullEventArgs ev)
    {
        try
        {
            var command = JsonConvert.DeserializeObject<RemoteCommand>(ev.Data)!;
            switch (command.Action)
            {
                case ActionType.Log:
                    HandleLogCommand(command);
                    break;
                case ActionType.SendMessage:
                    await HandleSendMessage(command);
                    break;
                case ActionType.UpdateActivity:
                    await HandleUpdateActivity(command);
                    break;
                case ActionType.UpdateChannelActivity:
                    await HandleChannelTopic(command);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error(ServerNumber, nameof(OnReceived), e);
        }
    }

    private void HandleLogCommand(RemoteCommand command)
    {
        if (!Enum.TryParse(command.Parameters[0].ToString(), true, out ChannelType type)) return;

        foreach (var channel in Program.Config.Channels[ServerNumber].Logs[type])
        {
            if (!Messages.ContainsKey(channel))
                Messages[channel] = string.Empty;

            Messages[channel] += $"[{DateTime.Now}] {command.Parameters[1]}\n";
        }
    }

    private async Task HandleSendMessage(RemoteCommand command)
    {
        if (!ulong.TryParse(command.Parameters[0].ToString(), out var chanId)) return;

        var split = command.Parameters[1].ToString()!.Split('|');
        bool isSuccess = (bool)command.Parameters[2];
        var embed = await EmbedBuilderService.CreateBasicEmbed(ServerNumber + split[0], split[1], isSuccess ? Color.Green : Color.Red);

        await Guild.GetTextChannel(chanId).SendMessageAsync(embed: embed);
    }

    private async Task HandleUpdateActivity(RemoteCommand command)
    {
        try
        {
            var split = command.Parameters[0].ToString()!.Split('/');
            int count = int.Parse(split[0]), total = int.Parse(split[1]);

            if (count > 0 && Client.Status != UserStatus.Online)
                await Client.SetStatusAsync(UserStatus.Online);
            else if (count == 0 && Client.Status != UserStatus.AFK)
                await Client.SetStatusAsync(UserStatus.AFK);

            if (count != lastCount || total != lastTotal)
            {
                lastCount = count;
                if (total > 0) lastTotal = total;
                await Client.SetActivityAsync(new Game($"{lastCount}/{lastTotal}"));
            }
        }
        catch (Exception e)
        {
            Log.Error(ServerNumber, nameof(HandleUpdateActivity), "Error updating bot status");
            Log.Debug(ServerNumber, nameof(HandleUpdateActivity), e);
        }
    }

    private async Task HandleChannelTopic(RemoteCommand command)
    {
        foreach (var channelId in Program.Config.Channels[ServerNumber].TopicInfo)
        {
            var channel = Guild.GetTextChannel(channelId);
            if (channel is not null)
                await channel.ModifyAsync(x => x.Topic = (string)command.Parameters[0]);
        }
    }

    private async Task DequeueMessages()
    {
        while (true)
        {
            List<KeyValuePair<LogChannel, string>> toSend;

            lock (Messages)
            {
                toSend = Messages.ToList();
                Messages.Clear();
            }

            foreach (var (channel, value) in toSend)
            {
                try
                {
                    if (value.Length > 1900)
                    {
                        var parts = value.Split('\n');
                        string chunk = string.Empty;
                        int i = 0;
                        while (chunk.Length < 1900 && i < parts.Length)
                            chunk += parts[i++] + '\n';

                        await SendLogMessage(channel, chunk);
                        Messages[channel] = value.Substring(chunk.Length);
                    }
                    else await SendLogMessage(channel, value);
                }
                catch (Exception e)
                {
                    Log.Error(ServerNumber, nameof(DequeueMessages), $"{e.Message}\nLikely invalid ChannelId {channel.Id} or GuildId {Program.Config.DiscordServerIds[ServerNumber]}");
                    Log.Debug(ServerNumber, nameof(DequeueMessages), value);
                }
            }

            await Task.Delay(Program.Config.MessageDelay);
        }
    }

    private Task SendLogMessage(LogChannel channel, string content)
    {
        return channel.LogType switch
        {
            LogType.Embed => Guild.GetTextChannel(channel.Id)
                .SendMessageAsync(embed: EmbedBuilderService.CreateBasicEmbed($"Server {ServerNumber} Logs", content, Color.Green).Result),

            LogType.Text => Guild.GetTextChannel(channel.Id)
                .SendMessageAsync($"[{ServerNumber}]: {content}"),

            _ => throw new ArgumentOutOfRangeException()
        };
    }
}