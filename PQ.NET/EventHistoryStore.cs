using System;
using System.Collections.Generic;
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
        internal ConcurrentBag<PQEvent<T>> history;

        public EventHistoryStore()
        {
            history = new ConcurrentBag<PQEvent<T>>();
        }

        internal void Add(PQEvent<T> pQEvent) => history.Add(pQEvent);
    }
}