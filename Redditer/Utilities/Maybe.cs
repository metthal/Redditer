using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redditer.Utilities
{
    public class Maybe<T>
    {
        public Maybe()
        {
            Undefine();
        }

        public Maybe(T value)
        {
            Value = value;
        }

        public static Maybe<T> Nothing()
        {
            return new Maybe<T>();
        }

        public static Maybe<T> Just(T value)
        {
            return new Maybe<T>(value);
        }

        public void Undefine()
        {
            Value = default(T);
            Defined = false;
        }

        public bool Defined { get; private set; }

        public T Value
        {
            get { return _value; }
            set
            {
                Defined = true;
                _value = value;
            }
        }

        private T _value;
    }
}
