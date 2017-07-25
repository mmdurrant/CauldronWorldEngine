using System;
using System.Runtime.Serialization;

namespace CauldronWorldEngine.Database
{
    public class DbResponse<T>
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public T Result { get; set; }
    }
}