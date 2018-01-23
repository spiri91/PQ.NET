using System;

namespace PQ.NET
{
    public class EventArgsContainer<T> : EventArgs
    {
        public PqEvent<T> Obj { get; }

        public EventArgsContainer(PqEvent<T> obj)
        {
            Obj = obj;
        }
    }
}
