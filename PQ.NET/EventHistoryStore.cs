using System.Collections.Concurrent;

namespace PQ.NET
{
    public enum Actions
    {
        Enqueue = 0,
        Dequeue = 1
    }

    internal class EventHistoryStore<T>
    {
        internal readonly ConcurrentBag<PqEvent<T>> History;

        public EventHistoryStore()
        {
            History = new ConcurrentBag<PqEvent<T>>();
        }

        internal void Add(PqEvent<T> pQEvent) => History.Add(pQEvent);
    }
}