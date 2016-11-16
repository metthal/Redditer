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

            try
            {
                vault.Add(credentials);
            }
            catch (Exception)
            {
            }
        }

        public static PasswordCredential Get(string username)
        {
            var vault = new PasswordVault();
            try
            {
                var credentials = vault.Retrieve(Resource, username);
                return credentials;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Remove(string username)
        {
            var vault = new PasswordVault();
            try
            {
                var credentials = vault.Retrieve(Resource, username);
                if (credentials == null)
                    return;

                vault.Remove(credentials);
            }
            catch (Exception)
            {
            }
        }
    }
}
