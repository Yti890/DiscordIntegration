using DiscordIntegration.API.Configs;
using System.Collections.Generic;
using System.ComponentModel;

namespace DiscordIntegration
{
    public class Config
    {
        public bool Debug { get; set; } = false;
        [Description("Bot-related configs")]
        public Bot Bot { get; private set; } = new Bot();
        [Description("Indicates events that should be logged or not")]
        public EventsToLog EventsToLog { get; private set; } = new EventsToLog();
        [Description("Indicates events that should be logged to the staff-only channel. These logs always ignore DNT, and will always show the player's UserID.")]
        public EventsToLog StaffOnlyEventsToLog { get; private set; } = new EventsToLog();
        [Description("Indicates whether players' IP Addresses should be logged or not")]
        public bool ShouldLogIPAddresses { get; private set; } = true;
        [Description("Indicates whether players' user ids should be logged or not")]
        public bool ShouldLogUserIds { get; private set; } = true;
        [Description("Indicates whether server errors should be logged or not.")]
        public bool LogErrors { get; private set; } = true;
        [Description("Indicates whether players' with the \"Do not track\" enabled, should be logged or not")]
        public bool ShouldRespectDoNotTrack { get; private set; } = true;
        [Description("Indicates whether only friendly fire for damage should be logged or not")]
        public bool ShouldLogFriendlyFireDamageOnly { get; private set; }
        [Description("Indicates whether only friendly fire for kills should be logged or not")]
        public bool ShouldLogFriendlyFireKillsOnly { get; private set; }

        [Description("Indicates whether or not only player-dealt damage should be logged in hurting events.")]
        public bool OnlyLogPlayerDamage { get; private set; }
        [Description("The date format that will be used throughout the plugin (es. dd/MM/yy HH:mm:ss or MM/dd/yy HH:mm:ss)")]
        public string DateFormat { get; private set; } = "dd/MM/yy HH:mm:ss";
        [Description("The list of trusted admins, whos command usage will not be logged.")]
        public List<string> TrustedAdmins { get; private set; } = new List<string>();
        [Description("The plugin language")]
        public string Language { get; private set; } = "english";
        [Description("Indicates whether the player watchlist feature can be used.")]
        public bool UseWatchlist { get; set; } = true;
        [Description("Ranks for Admin Chat.")]
        public List<string> Ranks { get; set; } = new List<string>()
        {
            "owner",
            "admin",
            "moderator"
        };
    }
}
