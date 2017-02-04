using System;

namespace System
{
    public class NotFiniteNumberException : Exception
    {
        public NotFiniteNumberException()
        {
        }

        public NotFiniteNumberException(string message)
            : base(message)
        {
        }
    }
}
