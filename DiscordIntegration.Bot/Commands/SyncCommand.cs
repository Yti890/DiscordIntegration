using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordIntegration.Bot.Services;
using DiscordIntegration.Dependency;
using Newtonsoft.Json;
using ActionType = DiscordIntegration.Dependency.ActionType;

namespace DiscordIntegration.Bot.Commands
{
    public class SyncCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Bot bot;

        public SyncCommand(Bot bot) => this.bot = bot;

        [SlashCommand("sync", "Sync your SteamID64@steam for automatic role synchronization.")]
        public async Task SyncAsync([Summary("steamid", "Your SteamID64@steam")] string steamId)
        {
            if (!steamId.EndsWith("@steam"))
            {
                await RespondAsync("❌ Invalid SteamID format. Make sure it ends with @steam.", ephemeral: true);
                return;
            }

            var discordId = Context.User.Id.ToString();
            Program.Users.SteamToDiscord[steamId] = discordId;
            File.WriteAllText("DiscordIntegration-users.json",
                JsonConvert.SerializeObject(Program.Users, Formatting.Indented));

            if (!Program.Config.DiscordServerIds.TryGetValue(1, out var guildId))
            {
                await RespondAsync("⚠️ Discord server is not configured properly.", ephemeral: true);
                return;
            }

            var client = Context.Client as DiscordSocketClient;
            var guild = client?.GetGuild(guildId);
            if (guild == null)
            {
                await RespondAsync("❌ Unable to fetch the configured Discord server.", ephemeral: true);
                return;
            }

            var member = guild.GetUser(Context.User.Id);
            if (member == null)
            {
                await RespondAsync("❌ Could not find you in the configured Discord server.", ephemeral: true);
                return;
            }

            var matchedRoles = new List<string>();
            foreach (var role in member.Roles)
            {
                if (Program.Config.DiscordAutomaticRoles.TryGetValue(role.Id, out var roleName))
                {
                    matchedRoles.Add(roleName);
                }
            }

            var userRoles = member.Roles.Where(r => Program.Config.DiscordAutomaticRoles.ContainsKey(r.Id)).Select(r => Program.Config.DiscordAutomaticRoles[r.Id]).ToList();
            string? highestRole = null;
            if (userRoles.Count > 0)
            {
                foreach (var kv in Program.Config.DiscordAutomaticRoles)
                {
                    if (userRoles.Contains(kv.Value))
                    {
                        highestRole = kv.Value;
                        break;
                    }
                }
            }

            if (highestRole != null)
            {
                await bot.Server.SendAsync(new RemoteCommand(ActionType.AutomaticRoles, $"/pm setgroup {steamId} {highestRole}"));
                await RespondAsync($"✅ Sync complete. Assigned role: {highestRole}", ephemeral: true);
            }
            else
            {
                await RespondAsync("⚠️ Sync complete, but no matching roles were found.", ephemeral: true);
            }
        }
    }
}