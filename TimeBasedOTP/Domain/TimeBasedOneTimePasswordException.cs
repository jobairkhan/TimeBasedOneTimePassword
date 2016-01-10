using System;
using System.Runtime.Serialization;

namespace TimeBasedOTP
{
    public sealed class TimeBasedOneTimePasswordException : Exception
    {
        public TimeBasedOneTimePasswordException()
            : base()
        {
        }

        public TimeBasedOneTimePasswordException(string message)
            : base(message)
        {
        }

        public TimeBasedOneTimePasswordException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TimeBasedOneTimePasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
