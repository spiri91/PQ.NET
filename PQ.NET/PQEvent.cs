using System;

namespace PQ.NET
{
    public class PqEvent<T>
    {
        public Actions Action { get; }
        public T Value { get; }
        public DateTimeOffset TimeItHappened { get; }
        public uint Priority { get; }

        public PqEvent(Actions action, T value, uint priority)
        {
            Action = action;
            Value = value;
            TimeItHappened = DateTimeOffset.UtcNow;
            Priority = priority;
        }
    }
}