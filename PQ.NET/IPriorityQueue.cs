using System;
using System.Collections.Generic;

namespace PQ.NET
{
    public interface IPriorityQueue<T>
    {
        IEnumerable<uint> ExistingPriorities { get; }
        IList<PqEvent<T>> EventsHistory { get; }
        IList<T> this[uint priorityLevel] { get; }

        event EventHandler ElementEnqueued;
        event EventHandler ElementDequeued;

        void Enqueue(T obj, UInt32 priority);
        void Enqueue(T obj);
        IList<T> GetFullQueueWithPriority(uint priorityLevel);
        T Dequeue(UInt32 priority);
        T Dequeue();
        T Peek(UInt32 priority);
        T Peek();
        int GetLengthOfQueue();
        int GetLengthOfQueue(uint priorityLevel);
        void AddPriorityLevel(uint priorityLevel);
        void AddPriorityLevels(IEnumerable<uint> priorityLevels);
        void DeleteAllElementsFromQueueWithPriorityLevel(uint priorityLevel);
    }
}