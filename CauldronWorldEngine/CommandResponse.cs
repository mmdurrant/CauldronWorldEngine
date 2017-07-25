using System;

namespace CauldronWorldEngine
{
    public class CommandResponse<T>
    {
        public bool Success { get; set; }
        public bool IsException => Exception != null;
        public string Message { get; set; }
        public T Result { get; set; }
        public Exception Exception { get; set; }
    }
}