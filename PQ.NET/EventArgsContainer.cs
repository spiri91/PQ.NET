using System;
using System.Collections.Generic;
using System.Text;

namespace PQ.NET
{
    public class EventArgsContainer<T> : EventArgs
    {
        public T Value { get; private set; }
        public UInt32 Priority { get; private set; }

        public EventArgsContainer(T obj, UInt32 priority)
        {
            this.Value = obj;
            this.Priority = priority;
        }
    }
}
