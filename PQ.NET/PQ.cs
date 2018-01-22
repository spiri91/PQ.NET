using System;
using System.Collections.Generic;
using System.Linq;

namespace PQ.NET
{
    public class Pq<T> : IQueue<T>
    {
        /// <summary>
        /// Just a GETTER in order to see the current levels of priority, in order to add new one use method AddPriority 
        /// </summary>
        public IEnumerable<uint> ExistingPriorities => _coreStore.Priorities;

        /// <summary>
        /// Returns a list of all Enqueued and Dequeued events, if the list becomes huge you can override the AddEventToHistoryMethod
        /// </summary>
        public IList<PqEvent<T>> EventsHistory => _eventHistoryStore.History.ToList();

        /// <summary>
        /// It is triggered before one element is enqueued.
        /// </summary>
        public event EventHandler ElementEnqueued;

        /// <summary>
        /// It is triggered after one element has been dequeued.
        /// </summary>
        public event EventHandler ElementDequeued;

        private readonly CoreStore<T> _coreStore;
        private readonly EventHistoryStore<T> _eventHistoryStore;

        /// <summary>
        /// Return a new instance of Priority Queue 
        /// </summary>
        /// <exception cref="ArgumentException">When levelsOfPriority are null or don't contain any elements</exception>
        /// <exception cref="ArgumentNullException">When default object is null </exception>
        /// <param name="levelsOfPriority">The levels of priority that the queue will initially have, levels cand be added later </param>
        /// <param name="defaultObject">In case of the queue is empty this object will be returned </param>
        public Pq(IEnumerable<uint> levelsOfPriority, T defaultObject)
        {
            if (levelsOfPriority == null || levelsOfPriority.Count() == 0)
                throw new ArgumentException("Levels of priority should contain at least one element.");

            if (defaultObject == null)
                throw new ArgumentNullException("Default object can not be null.");

            _coreStore = new CoreStore<T>(levelsOfPriority, defaultObject);
            _eventHistoryStore = new EventHistoryStore<T>();
        }

        #region Queries
        /// <summary>
        /// Query => return a deep copy of all elements with priority in order.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IList<T> this[uint index] => GetFullQueueWithPriority(index);

        /// <summary>
        /// Query => return a deep copy of all elements with priority in order.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IList<T> GetFullQueueWithPriority(uint index)
        {
            if (!CheckIfPriorityExists(index))
                throw new KeyNotFoundException($"Queue with priority {index} does not exist.");

            return _coreStore.GetAllElementsWithPrio(index);
        }


        public T Peek(uint priority) => _coreStore.Peek(priority);

        public T Peek() => _coreStore.Peek();
        
        public int GetLengthOfQueue() => _coreStore.GetLengthOfQueue();

        public int GetLengthOfQueue(uint priority)
        {
            if (!CheckIfPriorityExists(priority))
                return 0;

            return _coreStore.GetLengthOfQueue(priority);
        }
        #endregion

        #region Commands
        public void Enqueue(T obj, uint priority)
        {
            EnsureObjIsNotNull(obj);
            if (!CheckIfPriorityExists(priority))
                _coreStore.AddPriority(priority);

            AddEventToHistory(Actions.Enqueue, obj, priority);
            FireEnqueuedEvent(obj, priority);
            _coreStore.Append(obj, priority);
        }

        public void Enqueue(T obj)
        {
            EnsureObjIsNotNull(obj);
            AddEventToHistory(Actions.Enqueue, obj, ExistingPriorities.Min());
            FireEnqueuedEvent(obj, ExistingPriorities.Min());
            _coreStore.Append(obj);
        }

        public T Dequeue(uint priority)
        {
            if (!CheckIfPriorityExists(priority))
                throw new KeyNotFoundException($"Queue with priority {priority} does not exist.");

            var obj = _coreStore.Pop(priority);
            AddEventToHistory(Actions.Dequeue, obj, priority);
            FireDequeuedEvent(obj, priority);

            return obj;
        }

        public T Dequeue()
        {
            var element = _coreStore.Pop();
            AddEventToHistory(Actions.Dequeue, element.Item1, element.Item2);
            FireDequeuedEvent(element.Item1, element.Item2);

            return element.Item1;
        }

        public void AddPriority(uint priority) => _coreStore.AddPriority(priority);

        public void AddPriorities(IEnumerable<uint> priorities)
        {
            foreach (var i in priorities)
                _coreStore.AddPriority(i);
        }

        public void DeletePriority(uint priority) => _coreStore.DeletePrio(priority);
    
        public void EmptyQueue() => _coreStore.EmptyQueue();

        protected virtual void AddEventToHistory(Actions action, T obj, uint priority) 
            => _eventHistoryStore.Add(new PqEvent<T>(action, obj, priority));
        #endregion

        // ReSharper disable once UnusedParameter.Local
        private void EnsureObjIsNotNull(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("The argument can not be null.");
        }

        private bool CheckIfPriorityExists(uint priority) => _coreStore.Priorities.Contains(priority);

        private void FireDequeuedEvent(T obj, uint priority) => ElementDequeued?.Invoke(this, new EventArgsContainer<T>(obj, priority));

        private void FireEnqueuedEvent(T obj, uint priority) => ElementEnqueued?.Invoke(this, new EventArgsContainer<T>(obj, priority));
    }
}
