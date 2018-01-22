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
        /// <param name="levelsOfPriority">The levels of priority that the queue will initially have, levels can be added later </param>
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
        /// <param name="priorityLevel"> Level of priority assign</param>
        /// <exception cref="KeyNotFoundException">In case the priority with this level doesn't exist.</exception>
        /// <returns>Returns a deep copy of all elements with priority in order</returns>
        public IList<T> this[uint priorityLevel] => GetFullQueueWithPriority(priorityLevel);

        /// <summary>
        /// Query => return a deep copy of all elements with priority in order.
        /// </summary>
        /// <param name="index"> Level of priority assign</param>
        /// <exception cref="KeyNotFoundException">In case the priority with this level doesn't exist.</exception>
        /// <returns>Returns a deep copy of all elements with priority in order</returns>
        public IList<T> GetFullQueueWithPriority(uint priorityLevel)
        {
            if (!CheckIfPriorityExists(priorityLevel))
                throw new KeyNotFoundException($"Queue with priority {priorityLevel} does not exist.");

            return _coreStore.GetAllElementsWithPrio(priorityLevel);
        }

        /// <summary>
        /// Peeks the next element to be dequeued.
        /// </summary>
        /// <param name="priorityLevel"></param>
        /// <exception cref="KeyNotFoundException">In case the priority with this level doesn't exist.</exception>
        /// <returns>Default object if the queue with this priority is empty.</returns>
        public T Peek(uint priorityLevel)
        {
            if (!CheckIfPriorityExists(priorityLevel))
                throw new KeyNotFoundException($"Queue with priority {priorityLevel} does not exist.");

            return _coreStore.Peek(priorityLevel);
        }

        /// <summary>
        /// Peeks the next element to be dequeued with max priority.
        /// </summary>
        /// <returns>Default object if the queue is empty.</returns>
        public T Peek() => _coreStore.Peek();

        /// <summary>
        /// Gets the full length of the queue with all levels of priority.
        /// </summary>
        /// <returns>Gets the full length of the queue with all levels of priority</returns>
        public int GetLengthOfQueue() => _coreStore.GetLengthOfQueue();

        /// <summary>
        /// Gets the full length of the queue with specific priority.
        /// </summary>
        /// <exception cref="KeyNotFoundException">In case the priority with this level doesn't exist.</exception>
        /// <param name="priorityLevel">Represent the priority level of the queue</param>
        /// <returns>Gets the full length of the queue with specific priority.</returns>
        public int GetLengthOfQueue(uint priorityLevel)
        {
            if (!CheckIfPriorityExists(priorityLevel))
                throw new KeyNotFoundException($"Queue with priority {priorityLevel} does not exist.");

            return _coreStore.GetLengthOfQueue(priorityLevel);
        }
        #endregion

        /// <summary>
        /// Enqueues the object on the selected queue.
        /// If this level of priority is not already present in the queue, it will be automatically added.
        /// </summary>
        /// <exception cref="ArgumentNullException">If element passed is null.</exception>
        /// <param name="obj">Element to be enqueued, can not be null.</param>
        /// <param name="priorityLevel">Level of priority that this element will be enqueued to</param>
        #region Commands
        public void Enqueue(T obj, uint priorityLevel)
        {
            EnsureObjIsNotNull(obj);
            if (!CheckIfPriorityExists(priorityLevel))
                _coreStore.AddPriority(priorityLevel);

            AddEventToHistory(Actions.Enqueue, obj, priorityLevel);
            FireEnqueuedEvent(obj, priorityLevel);
            _coreStore.Append(obj, priorityLevel);
        }

        /// <summary>
        /// Enqueues the object on lowest level of priority that the queue already has.
        /// </summary>
        /// <exception cref="ArgumentNullException">If element passed is null.</exception>
        /// <param name="obj">Element to be enqueued, can not be null.</param>
        public void Enqueue(T obj)
        {
            EnsureObjIsNotNull(obj);
            AddEventToHistory(Actions.Enqueue, obj, ExistingPriorities.Min());
            FireEnqueuedEvent(obj, ExistingPriorities.Min());
            _coreStore.Append(obj);
        }

        /// <summary>
        /// Dequeues the next element in line from the queue with priority.
        /// </summary>
        /// <exception cref="KeyNotFoundException">If this priority level is not already present.</exception>
        /// <param name="priorityLevel">Represent the priority level of the queue</param>
        /// <returns>Dequeues the next element in line from the queue with priority</returns>
        public T Dequeue(uint priorityLevel)
        {
            if (!CheckIfPriorityExists(priorityLevel))
                throw new KeyNotFoundException($"Queue with priority {priorityLevel} does not exist.");

            var obj = _coreStore.Pop(priorityLevel);
            AddEventToHistory(Actions.Dequeue, obj, priorityLevel);
            FireDequeuedEvent(obj, priorityLevel);

            return obj;
        }

        /// <summary>
        /// Dequeues the next element in line from the queue with max priority.
        /// </summary>
        /// <returns>Dequeues the next element in line from the queue with max priority.</returns>
        public T Dequeue()
        {
            var element = _coreStore.Pop();
            AddEventToHistory(Actions.Dequeue, element.Item1, element.Item2);
            FireDequeuedEvent(element.Item1, element.Item2);

            return element.Item1;
        }

        /// <summary>
        /// Adds a new level of priority to exiting one, if this level is already present, it will not be overridden and no errors will be thrown
        /// </summary>
        /// <param name="priorityLevel">New priority level to be added.</param>
        public void AddPriorityLevel(uint priorityLevel) => _coreStore.AddPriority(priorityLevel);

        /// <summary>
        /// Adds a new levels of priority to exiting ones, if this levels are already present, it will not be overridden and no errors will be thrown
        /// </summary>
        /// <param name="priorityLevels">New priority levels to be added.</param>
        public void AddPriorityLevels(IEnumerable<uint> priorityLevels)
        {
            foreach (var i in priorityLevels)
                _coreStore.AddPriority(i);
        }

        /// <summary>
        /// Deleted a priority level and all it's elements
        /// </summary>
        /// <exception cref="KeyNotFoundException">If selected priority is not present</exception>
        /// <param name="priorityLevel">Priority level to be deleted.</param>
        public void DeleteAllElementsFromQueueWithPriorityLevel(uint priorityLevel)
        {
            if (!CheckIfPriorityExists(priorityLevel))
                throw new KeyNotFoundException($"Queue with priority {priorityLevel} does not exist.");

            _coreStore.DeletePrio(priorityLevel);
        }

        /// <summary>
        /// Every Enqueue or Dequeue action is stored in history events list;
        /// Can be overridden if needed 
        /// </summary>
        /// <param name="action">Enqueue or Dequeue</param>
        /// <param name="obj"></param>
        /// <param name="priorityLevel"></param>
        protected virtual void AddEventToHistory(Actions action, T obj, uint priorityLevel)
            => _eventHistoryStore.Add(new PqEvent<T>(action, obj, priorityLevel));
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
