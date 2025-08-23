namespace DiscordIntegration.Bot;

using DiscordIntegration.Bot.Services;
using Newtonsoft.Json;

public static class Program
{
    private static Config? _config;
    private static UsersData? _users;

    private static readonly string KCfgFile = "DiscordIntegration-config.json";
    private static readonly string KUsersFile = "DiscordIntegration-users.json";

    private static readonly List<Bot> _bots = new();
    public static Config Config => _config ??= LoadConfig();
    public static UsersData Users => _users ??= LoadUsers();

    public static Random Rng { get; } = new();

    public static void Main(string[] args)
    {
        Log.PrintBanner();
        Log.IsDebug = args.Contains("--debug");

        _ = Config;
        _ = Users;

        foreach (var (id, token) in Config.BotTokens)
            _bots.Add(new Bot(id, token));

        AppDomain.CurrentDomain.ProcessExit += (_, _) => OnExit();

        KeepAlive().GetAwaiter().GetResult();
    }

    private static Config LoadConfig()
    {
        if (File.Exists(KCfgFile))
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(KCfgFile))!;

        var defaultConfig = Config.Default;
        File.WriteAllText(KCfgFile, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
        return defaultConfig;
    }

    private static UsersData LoadUsers()
    {
        if (File.Exists(KUsersFile))
            return JsonConvert.DeserializeObject<UsersData>(File.ReadAllText(KUsersFile))!;

        var defaultUsers = UsersData.Default;
        File.WriteAllText(KUsersFile, JsonConvert.SerializeObject(defaultUsers, Formatting.Indented));
        return defaultUsers;
    }

    private static void OnExit()
    {
        foreach (var bot in _bots)
            bot.Destroy();

        if (Config.Debug)
        {
            Log.Warn(0, nameof(Main), "Shutting down..");
            Thread.Sleep(10000);
        }
    }

    private static async Task KeepAlive()
    {
        Log.Debug(0, nameof(KeepAlive), "Keeping alive bots.");
        await Task.Delay(-1);
    }
}