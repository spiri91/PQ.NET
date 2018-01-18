using System;

namespace PQ.NET
{
    public interface IQueue<T>
    {
        void Enqueue(T obj, UInt32 priority);
        void Enqueue(T obj);
        T Dequeue(UInt32 priority);
        T Dequeue();
        T Peek(UInt32 priority);
        T Peek();
    }
}