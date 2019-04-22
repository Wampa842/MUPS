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

    [Serializable]
    public class PmxImportException : PmxException
    {
        public long StreamPosition { get; private set; }

        public PmxImportException() { }
        public PmxImportException(string message) : base(message) { }
        public PmxImportException(string message, long position) : this(message)
        {
            StreamPosition = position;
        }
        public PmxImportException(string message, Exception inner) : base(message, inner) { }
        protected PmxImportException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
