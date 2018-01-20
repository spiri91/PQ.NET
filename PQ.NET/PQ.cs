using System;
using System.Collections.Generic;
using System.Linq;

namespace PQ.NET
{
    public class PQ<T> : IQueue<T>
    {
        public IEnumerable<uint> ExistingPriorities { get { return _coreStore._priorities; } }
        public event EventHandler ElementEnqueued;
        public event EventHandler ElementDequeued;

        private CoreStore<T> _coreStore;

        public PQ(IEnumerable<uint> priorities, T defaultObject)
        {
            _coreStore = new CoreStore<T>(priorities, defaultObject);
        }

        #region Queries
        public IList<T> this[uint index]
        {
            get
            {
                return GetFullQueueWithPriority(index);
            }
        }

        public IList<T> GetFullQueueWithPriority(uint index)
        {
            if (!ExistingPriorities.Contains(index))
                return new List<T>();

            return _coreStore.GetAllElementsWithPrio(index);
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

        public int GetLengthOfQueue() => _coreStore.GetLengthOfQueue();

        public int GetLengthOfQueue(uint priority)
        {
            if (!CheckIfPriorityExists(priority))
                return 0;

            return _coreStore.GetLengthOfQueue(priority);
        }

        private bool CheckIfPriorityExists(uint priority) => _coreStore._priorities.Contains(priority);
        #endregion

        #region Commands
        public void Enqueue(T obj, uint priority)
        {
            if (!CheckIfPriorityExists(priority))
                _coreStore.AddPriority(priority);

            FireEnqueuedEvent(obj, priority);
            _coreStore.Append(obj, priority);
        }

        public void Enqueue(T obj)
        {
            EnsureObjIsNotNull(obj);
            FireEnqueuedEvent(obj, ExistingPriorities.Min());
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

        public void AddPriority(uint priority) => _coreStore.AddPriority(priority);

        public void AddPriorities(IEnumerable<uint> priorities)
        {
            foreach (var i in priorities)
                _coreStore.AddPriority(i);
        }

        public void DeletePriority(uint priority)
        {
            _coreStore.DeletePrio(priority);
        }

        public void EmptyQueue() => _coreStore.EmptyQueue();
        #endregion

        private void EnsureObjIsNotNull(T obj)
        {
            if (obj == null)
                throw new NullReferenceException();
        }

        private void FireDequeuedEvent(T obj, uint priority) => ElementDequeued?.Invoke(this, new EventArgsContainer<T>(obj, priority));

        private void FireEnqueuedEvent(T obj, uint priority) => ElementEnqueued?.Invoke(this, new EventArgsContainer<T>(obj, priority));
    }
}
