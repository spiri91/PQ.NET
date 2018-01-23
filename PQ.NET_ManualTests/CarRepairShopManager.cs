using System;
using System.Linq;
using PQ.NET;

namespace PQ.NET_Examples
{
    internal class CarRepairShopManager
    {
        public CarRepairShopManager()
        {
        }

        internal void AddTasks(Pq<CarRepairTask> pq)
        {
            Enumerable.Range(1, 300000).ToList().ForEach(x =>
             {
                 if (x < 100000)
                     pq.Enqueue(new CarRepairTask(x), (uint)(int)Demo.Priorities.Medium);
                 else if (x < 200000)
                     pq.Enqueue(new CarRepairTask(x), (uint)(int)Demo.Priorities.Hi);
                 else if (x <= 300000)
                     pq.Enqueue(new CarRepairTask(x), (uint)(int)Demo.Priorities.Low);
             });
        }
    }
}