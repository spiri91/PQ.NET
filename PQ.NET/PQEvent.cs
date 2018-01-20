using System;

namespace PQ.NET
{
    public class PQEvent<T>
    {
        public Actions Action { get; private set; }
        public T Value { get; private set; }
        public DateTimeOffset TimeItHappened { get; private set; }
        public uint Priority { get; private set; }

        public PQEvent(Actions action, T value, uint priority)
        {
            this.Action = action;
            this.Value = value;
            this.TimeItHappened = DateTimeOffset.UtcNow;
            this.Priority = priority;
        }
    }
}