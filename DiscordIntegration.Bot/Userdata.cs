using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordIntegration.Bot;

public class UsersData
{
    public Dictionary<string, string> SteamToDiscord { get; set; } = new();

    public static UsersData Default => new UsersData
    {
        SteamToDiscord = new Dictionary<string, string>()
    };
}