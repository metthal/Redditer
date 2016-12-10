using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;

namespace Redditer.Providers
{
    public class TableLastLoggedUser
    {
        [PrimaryKey]
        public string Username { get; set; }
    }

    public class TableFavorites
    {
        [PrimaryKey]
        public string Subreddit { get; set; }
    }

    public class Settings
    {
        public static Settings Instance => _instance ?? (_instance = new Settings());
        private static Settings _instance;

        public Settings()
        {
            LastLoggedUser = null;
            Favorites = new List<string>();
        }

        public void Load()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "settings.sqlite");
            _connection = new SQLiteConnection(new SQLitePlatformWinRT(), path);

            var lastLoggedUserTableInfo = _connection.GetTableInfo("TableLastLoggedUser");
            if (!lastLoggedUserTableInfo.Any())
                _connection.CreateTable<TableLastLoggedUser>();
            else
                LastLoggedUser = QueryLastLoggedUser();

            var favoritesTableInfo = _connection.GetTableInfo("TableFavorites");
            if (!favoritesTableInfo.Any())
                _connection.CreateTable<TableFavorites>();
            else
                Favorites = QueryFavorites();
        }

        public void Save()
        {
            if (_connection == null)
                return;

            _connection.Table<TableLastLoggedUser>().Delete(user => true);
            _connection.Table<TableFavorites>().Delete(favorites => true);

            if (LastLoggedUser != null)
                _connection.Insert(new TableLastLoggedUser {Username = LastLoggedUser});

            foreach (var favorite in Favorites)
                _connection.Insert(new TableFavorites {Subreddit = favorite});
        }

        public void Reset()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "settings.sqlite");
            File.Delete(path);
        }

        private string QueryLastLoggedUser()
        {
            var list = _connection.Table<TableLastLoggedUser>().ToList();
            return list.Count > 0 ? list[0].Username : null;
        }

        private List<string> QueryFavorites()
        {
            var list = _connection.Table<TableFavorites>().ToList();
            var favorites = new List<string>();
            foreach (var favorite in list)
                favorites.Add(favorite.Subreddit);
            return favorites;
        }

        public string LastLoggedUser { get; set; }
        public List<string> Favorites { get; set; }

        private SQLiteConnection _connection;
    }
}
