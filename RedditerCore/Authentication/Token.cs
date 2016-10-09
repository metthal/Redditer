using System;

namespace RedditerCore.Authentication
{
    public class Token
    {
        public Token(string type, string value, uint durationSecs)
        {
            Type = type;
            Renew(value, durationSecs);
        }

        public void Renew(string newValue, uint durationSecs)
        {
            Value = newValue;
            ValidUntil = DateTime.Now.AddSeconds(durationSecs);
        }

        public override string ToString()
        {
            return Type + ' ' + Value;
        }

        public string Type { get; }
        public string Value { get; private set; }
        public DateTime ValidUntil { get; private set; }
        public bool Expired => DateTime.Now >= ValidUntil;
    }
}