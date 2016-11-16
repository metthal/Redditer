using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace Redditer.Providers
{
    public class Settings
    {
        public static Settings Instance => _instance ?? (_instance = new Settings());
        private static Settings _instance;

        public void Load()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "settings.sqlite");
            _connection = new SQLiteConnection(new SQLitePlatformWinRT(), path);

            var tableInfo = _connection.GetTableInfo("SettingsData");
            if (!tableInfo.Any())
            {
                _connection.CreateTable<SettingsData>();
                Data = new SettingsData();
            }
            else
            {
                var firstOrDefault = _connection.Table<SettingsData>().FirstOrDefault();
                Data = firstOrDefault ?? new SettingsData();
            }
        }

        public void Save()
        {
            if (_connection == null)
                return;

            _connection.InsertOrReplace(Data);
        }

        public void Reset()
        {
            if (_connection == null)
                return;

            _connection.DropTable<SettingsData>();
        }

        public SettingsData Data { get; set; }

        private SQLiteConnection _connection;
    }
}
