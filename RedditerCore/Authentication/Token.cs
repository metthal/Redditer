using System;

namespace RedditerCore.Authentication
{
    public class Token
    {
        public Token(string type, string value, uint durationSecs)
        {
            Type = type;
            Value = value;
            ValidUntil = DateTime.Now.AddSeconds(durationSecs);
        }

        public override string ToString()
        {
            return Type + ' ' + Value;
        }

        public string Type { get; }
        public string Value { get; }
        public DateTime ValidUntil { get; }
        public bool Expired => DateTime.Now >= ValidUntil;
    }
}