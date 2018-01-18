using System;
using System.Collections.Generic;
using System.Text;

namespace PQ.NET
{
    class CoreStore<T>
    {
        internal HashSet<UInt32> _priorities { get; private set; }

        public CoreStore(IEnumerable<uint> priorities)
        {

        }

        internal void Append<T>(T obj, uint priority) where T : new()
        {
            throw new NotImplementedException();
        }

        internal void Append<T>(T obj) where T : new()
        {
            throw new NotImplementedException();
        }

        internal T Pop(uint priority)
        {
            throw new NotImplementedException();
        }

        internal Tuple<T, uint> Pop()
        {
            throw new NotImplementedException();
        }

        internal void AddPriority(uint priority)
        {
            throw new NotImplementedException();
        }

        internal T Peek(uint priority)
        {
            throw new NotImplementedException();
        }

        internal T Peek()
        {
            throw new NotImplementedException();
        }
    }
}
