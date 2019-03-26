using System;

namespace PmxSharp
{
    [Serializable]
    public class PmxException : Exception
    {
        public PmxException() { }
        public PmxException(string message) : base(message) { }
        public PmxException(string message, Exception inner) : base(message, inner) { }
        protected PmxException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
