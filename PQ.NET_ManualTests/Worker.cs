using System;
using PQ.NET;

namespace PQ.NET_Examples
{
    internal class Worker
    {

        internal void WorkOnTaks(Pq<CarRepairTask> pq)
        {
            CarRepairTask obj;
            do
            {
                obj = pq.Dequeue();
            }
            while (obj.GetType() != typeof(NoTask));
        }
    }
}