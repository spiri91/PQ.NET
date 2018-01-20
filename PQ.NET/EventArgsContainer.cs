using System;

namespace PQ.NET
{
    public class EventArgsContainer<T> : EventArgs
    {
        public T Value { get; }
        public UInt32 Priority { get; }

        public EventArgsContainer(T obj, UInt32 priority)
        {
            Value = obj;
            Priority = priority;
        }
    }
}
