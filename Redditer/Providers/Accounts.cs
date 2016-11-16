using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace Redditer.Providers
{
    public static class Accounts
    {
        private static readonly string Resource = "Redditer";

        public static void Save(string username, string password)
        {
            var vault = new PasswordVault();
            var credentials = new PasswordCredential(Resource, username, password);
            vault.Add(credentials);
        }

        public static PasswordCredential Get(string username)
        {
            var vault = new PasswordVault();
            return vault.Retrieve(Resource, username);
        }

        public static void Remove(string username)
        {
            var vault = new PasswordVault();
            vault.Remove(vault.Retrieve(Resource, username));
        }
    }
}
