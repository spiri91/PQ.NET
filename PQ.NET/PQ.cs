using System;
using System.Collections.Generic;
using System.Linq;

namespace PQ.NET
{
    public class PQ<T> : IQueue<T> where T : new()
    {
        private CoreStore<T> _coreStore;

        public event EventHandler ElementEnqueued;
        public event EventHandler ElementDequeued;

        public PQ(IEnumerable<uint> priorities)
        {
            _coreStore = new CoreStore<T>(priorities);
        }

        public void Enqueue(T obj, uint priority)
        {
            CheckIfPriorityExists(priority);
            FireEnqueuedEvent(obj, priority);
            _coreStore.Append(obj, priority);
        }

        public void Enqueue(T obj)
        {
            EnsureObjIsNotNull(obj);
            _coreStore.Append(obj);
        }


        public T Dequeue(uint priority)
        {
            CheckIfPriorityExists(priority);
            var obj = _coreStore.Pop(priority);
            FireDequeuedEvent(obj, priority);

            return obj;
        }

        public T Dequeue()
        {
            Tuple<T, uint> element = _coreStore.Pop();
            FireDequeuedEvent(element.Item1, element.Item2);

            return element.Item1;
        }

        public void AddPriority(uint priority)
        {
            _coreStore.AddPriority(priority);
        }

        public void AddPriorities(IEnumerable<uint> priorities)
        {
            foreach (var i in priorities)
                _coreStore.AddPriority(i);
        }

        public T Peek(uint priority)
        {
            var obj = _coreStore.Peek(priority);

            return obj;
        }

        public T Peek()
        {
            var obj = _coreStore.Peek();

            return obj;
        }

        private void CheckIfPriorityExists(uint priority) => _coreStore._priorities.Contains(priority);

        private void EnsureObjIsNotNull(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
        }

        private void FireEnqueuedEvent(T obj, uint priority) => ElementEnqueued?.Invoke(this, new EventArgsContainer<T>(obj, priority));

        private void FireDequeuedEvent(T obj, uint priority) => ElementDequeued?.Invoke(this, new EventArgsContainer<T>(obj, priority));
    }
}
