using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Attributes;

namespace Redditer.Providers
{
    public class SettingsData
    {
        public SettingsData()
        {
            LastLoggedUser = null;
            Favorites = new List<string>();
        }

        public string LastLoggedUser { get; set; }

        [TextBlob("favoritesBlobbed")]
        public List<string> Favorites { get; set; }
    }
}
