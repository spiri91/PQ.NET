using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PQ.NET
{
    class CoreStore<T>
    {
        internal HashSet<uint> Priorities { get; private set; }

        private readonly ConcurrentDictionary<uint, ConcurrentQueue<T>> _store;
        private readonly T _defaultObj;
        private uint _minPrio;

        internal CoreStore(IEnumerable<uint> priorities, T defaultObj)
        {
            _store = new ConcurrentDictionary<uint, ConcurrentQueue<T>>();
            Priorities = new HashSet<uint>();
            _defaultObj = defaultObj;

            foreach (var i in priorities) Priorities.Add(i);

            AddPrioritiesInStore();
            SetMinAndMaxPrio();
        }

        private void SetMinAndMaxPrio()
        {
            _minPrio = Priorities.Min();
        }

        internal IList<T> GetAllElementsWithPrio(uint index)
        {
            return _store[index].ToList();
        }

        private void AddPrioritiesInStore()
        {
            foreach(var i in Priorities)
                _store.AddOrUpdate(i, new ConcurrentQueue<T>(), (key, old) => old);
        }

        internal void Append(T obj, uint priority) => _store[priority].Enqueue(obj);

        internal void Append(T obj) => _store[_minPrio].Enqueue(obj);
       
        internal T Pop(uint priority)
        {
            _store[priority].TryDequeue(out T obj);

            return obj;
        }

        internal Tuple<T, uint> Pop()
        {
            foreach(var i in Priorities.OrderByDescending(x => x))
            {
                if(_store[i].Count > 0)
                {
                    _store[i].TryDequeue(out T obj);
                    return new Tuple<T, uint>(obj, i);
                }
            }

            return new Tuple<T, uint>(_defaultObj, 0);
        }

        internal void AddPriority(uint priority)
        {
            Priorities.Add(priority);
            _store.AddOrUpdate(priority, new ConcurrentQueue<T>(), (key, oldQ) => oldQ);
        }

        internal T Peek(uint priority)
        {
            _store[priority].TryPeek(out T obj);

            return obj;
        }

        internal T Peek()
        {
            foreach (var i in Priorities.OrderByDescending(x => x))
            {
                if (_store[i].Count > 0)
                {
                    _store[i].TryPeek(out T obj);

                    return obj;
                }
            }

            return _defaultObj;
        }

        internal int GetLengthOfQueue(uint priority) => _store[priority].Count();

        internal void DeletePrio(uint priority)
        {
            _store.TryRemove(priority, out ConcurrentQueue<T> _);
            if (Priorities.Contains(priority)) Priorities.Remove(priority);
        }

        internal int GetLengthOfQueue()
        {
            var total = 0;
            foreach (var i in _store)
                total += i.Value.Count();

            return total;
        }

        internal void EmptyQueue()
        {
            _store.Clear();
            Priorities.Clear();
        }
    }
}
