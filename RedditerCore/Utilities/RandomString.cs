using System;

namespace RedditerCore.Utilities
{
    public static class RandomString
    {
        public static string Symbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Create(uint length)
        {
            Random rand = new Random();

            string str = "";
            for (uint i = 0; i < length; ++i)
                str += Symbols[rand.Next(Symbols.Length - 1)];

            return str;
        }
    }
}
