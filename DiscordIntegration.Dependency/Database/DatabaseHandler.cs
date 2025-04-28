using System;
using System.IO;
using LiteDB;

namespace DiscordIntegration.Dependency.Database
{
    public class DatabaseHandler
    {
        private static readonly string DatabasePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DiscordIntegration.db");

        private static LiteDatabase Database => new($"Filename={DatabasePath};Connection=shared");

        private static ILiteCollection<WatchlistEntry> WatchlistCollection => Database.GetCollection<WatchlistEntry>("watchlist");

        public static void Init()
        {
            WatchlistCollection.EnsureIndex(x => x.UserId);
        }

        public static void AddEntry(string userId, string reason)
        {
            var entry = new WatchlistEntry
            {
                UserId = userId,
                Reason = reason
            };
            WatchlistCollection.Upsert(entry);
        }

        public static void RemoveEntry(string userId)
        {
            WatchlistCollection.DeleteMany(x => x.UserId == userId);
        }

        public static bool CheckWatchlist(string userId, out string reason)
        {
            var entry = WatchlistCollection.FindOne(x => x.UserId == userId);
            if (entry != null)
            {
                reason = entry.Reason;
                return true;
            }

            reason = "Not in watchlist";
            return false;
        }

        private class WatchlistEntry
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string Reason { get; set; }
        }
    }
}