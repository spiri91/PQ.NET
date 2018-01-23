using PQ.NET;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PQ.NET_Examples
{
    internal class Demo
    {
        public enum Priorities
        {
            Low = 1,
            Medium = 11,
            Hi = 111
        }

        List<Worker> workers;
        CarRepairShopManager manager;
        Pq<CarRepairTask> pq;
        public Demo()
        {
            manager = new CarRepairShopManager();
            pq = new Pq<CarRepairTask>(new uint[] { 1, 11, 111 }, new NoTask());
            workers = new List<Worker>() { new Worker(), new Worker(), new Worker()};

            pq.ElementEnqueued += (x, e) => AppendInfoToConsole(e);
            pq.ElementDequeued += (x, e) => AppendInfoToConsole(e);

            manager.AddTasks(pq);
            Parallel.ForEach(workers, (x => x.WorkOnTaks(pq)));
        }

        private static void AppendInfoToConsole(EventArgs e)
        {
            var container = (EventArgsContainer<CarRepairTask>)e;
            Console.WriteLine($"Prio : {container.Obj.Priority} \t value : {container.Obj.Value.Id} \t action : {container.Obj.Action}");
        }
    }
}