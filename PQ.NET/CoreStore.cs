using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PQ.NET
{
    class CoreStore<T>
    {
        internal HashSet<uint> _priorities { get; private set; }

        private ConcurrentDictionary<uint, ConcurrentQueue<T>> store;
        private T defaultObj;
        private uint minPrio;
        private uint maxPrio;

        public CoreStore(IEnumerable<uint> priorities, T defaultObj)
        {
            store = new ConcurrentDictionary<uint, ConcurrentQueue<T>>();
            _priorities = new HashSet<uint>();
            this.defaultObj = defaultObj;

            foreach (var i in priorities) _priorities.Add(i);

            AddPrioritiesInStore();
            SetMinAndMaxPrio();
        }

        private void SetMinAndMaxPrio()
        {
            minPrio = _priorities.Min();
            maxPrio = _priorities.Max();
        }

        private void AddPrioritiesInStore()
        {
            foreach(var i in _priorities)
                store.AddOrUpdate(i, new ConcurrentQueue<T>(), (key, old) => old);
        }

        internal void Append(T obj, uint priority) => store[priority].Enqueue(obj);

        internal void Append(T obj) => store[minPrio].Enqueue(obj);
       
        internal T Pop(uint priority)
        {
            store[priority].TryDequeue(out T obj);

            return obj;
        }

        internal Tuple<T, uint> Pop()
        {
            foreach(var i in _priorities.OrderByDescending(x => x))
            {
                if(store[i].Count > 0)
                {
                    store[i].TryDequeue(out T obj);
                    return new Tuple<T, uint>(obj, i);
                }
            }

            return new Tuple<T, uint>(defaultObj, 0);
        }

        internal void AddPriority(uint priority)
        {
            _priorities.Add(priority);
            store.AddOrUpdate(priority, new ConcurrentQueue<T>(), (key, oldQ) => oldQ);
        }

        internal T Peek(uint priority)
        {
            store[priority].TryPeek(out T obj);

            return obj;
        }

        internal T Peek()
        {
            foreach (var i in _priorities.OrderByDescending(x => x))
            {
                if (store[i].Count > 0)
                {
                    store[i].TryPeek(out T obj);

                    return obj;
                }
            }

            return defaultObj;
        }

        internal int GetLengthOfQueue(uint priority) => store[priority].Count();

        internal void DeletePrio(uint priority)
        {
            ConcurrentQueue<T> queue;
            store.TryRemove(priority, out queue);
            if (_priorities.Contains(priority)) _priorities.Remove(priority);
        }
    }
}
