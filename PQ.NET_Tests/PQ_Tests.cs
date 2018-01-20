using NUnit.Framework;
using System;
using PQ.NET;
using System.Linq;
using System.Collections.Generic;

namespace PQ.NET_Tests
{
    [TestFixture]
    public class PQ_Tests
    {
        private uint[] priorities = new uint[4] { 1, 2, 3, 4 };
        private string defaultObject = "Empty queue";
        private string defaultElementForEnqueue = "foo";
        private List<string> elementsToEnque = new List<string>() { "foo", "boo", "goo" };

        [TestCase]
        public void Should_Create_PQ_Instance()
        {
            var pq = CreatePQ();
        }

        [TestCase]
        public void Should_Enqueue_Element()
        {
            var pq = CreatePQ();
            pq.Enqueue(defaultElementForEnqueue);
            var dequeuedElement = pq.Dequeue();
            Assert.AreEqual(dequeuedElement, defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Enqueue_Element_With_Prio()
        {
            var prio = priorities[0];
            var pq = CreatePQ();
            pq.Enqueue(defaultElementForEnqueue, prio);
            var dequeuedElement = pq.Dequeue(prio);
            Assert.AreEqual(dequeuedElement, defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Dequeue_Element()
        {
            var pq = CreatePQ();
            pq.Enqueue(defaultElementForEnqueue);
            var dequeuedElement = pq.Dequeue();
            Assert.AreEqual(dequeuedElement, defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Dequeue_Element_With_Prio()
        {
            var prio = priorities.Max();
            var pq = CreatePQ();
            pq.Enqueue(defaultElementForEnqueue, prio);
            var dequeuedElement = pq.Dequeue(prio);
            Assert.AreEqual(dequeuedElement, defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Peek_Element()
        {
            var pq = CreatePQ();
            pq.Enqueue(defaultElementForEnqueue);
            var peekedElement = pq.Peek();
            Assert.AreEqual(peekedElement, defaultElementForEnqueue);
            var lengthOfQ = pq.GetLengthOfQueue();
            Assert.AreEqual(1, lengthOfQ);
        }

        [TestCase]
        public void Should_Peek_Element_With_Prio()
        {
            var prio = priorities.Min();
            var pq = CreatePQ();
            pq.Enqueue(defaultElementForEnqueue, prio);
            var peekedElement = pq.Peek(prio);
            Assert.AreEqual(peekedElement, defaultElementForEnqueue);

            var lengthOfQ = pq.GetLengthOfQueue(prio);
            Assert.AreEqual(1, lengthOfQ);
        }

        [TestCase]
        public void Should_Add_Prio()
        {
            var newPrio = priorities.Max() + 1;
            var pq = CreatePQ();
            pq.AddPriority(newPrio);
            Assert.True(pq.ExistingPriorities.Contains(newPrio));
        }

        [TestCase]
        public void Should_Add_Prios()
        {
            var newPrios = Enumerable.Range((int)priorities.Max() + 1, 10).Select(x => (uint)x);
            var pq = CreatePQ();
            pq.AddPriorities(newPrios);

            newPrios.ToList().ForEach(x => Assert.True(pq.ExistingPriorities.Contains(x)));
        }

        [TestCase]
        public void Should_Fire_Enqueue_Event()
        {
            var pq = CreatePQ();
            var wasCalledEnqueuedEvent = false;
            var wasCalledDequeuedEvent = false;
            pq.ElementEnqueued += (a,b) => wasCalledEnqueuedEvent = true;
            pq.ElementDequeued += (a, b) => wasCalledDequeuedEvent = true;
            pq.Enqueue(defaultElementForEnqueue);

            Assert.True(wasCalledEnqueuedEvent);
            Assert.False(wasCalledDequeuedEvent);
        }

        [TestCase]
        public void Should_Fire_Dequeue_Event()
        {
            var pq = CreatePQ();
            var wasCalledEnqueuedEvent = false;
            var wasCalledDequeuedEvent = false;
            pq.Enqueue(defaultElementForEnqueue);

            pq.ElementEnqueued += (a, b) => wasCalledEnqueuedEvent = true;
            pq.ElementDequeued += (a, b) => wasCalledDequeuedEvent = true;
           
            pq.Dequeue();

            Assert.True(wasCalledDequeuedEvent);
            Assert.False(wasCalledEnqueuedEvent);
        }

        [TestCase]
        public void Should_Add_Prio_With_Add_Element_Function()
        {
            var pq = CreatePQ();
            var priority = priorities.Max() + 1;
            pq.Enqueue(defaultElementForEnqueue, priority);

            Assert.True(pq.ExistingPriorities.Contains(priority));
        }

        [TestCase]
        public void Should_Delete_Full_Prio()
        {
            var pq = CreatePQ();
            pq.AddPriorities(priorities);
            pq.Enqueue(defaultElementForEnqueue, priorities.Min());
            pq.DeletePriority(priorities.Min());
            pq.DeletePriority(priorities.Max() + 1);
            Assert.False(pq.ExistingPriorities.Contains(priorities.Min()));

            pq.Enqueue(defaultElementForEnqueue, priorities.Min());

            Assert.AreEqual(1, pq.GetLengthOfQueue(priorities.Min()));
        }

        [TestCase]
        public void Should_Empty_Queue()
        {
            var pq = CreatePQ();
            pq.EmptyQueue();

            Assert.True(pq.ExistingPriorities.Count() == 0);
        }

        [TestCase]
        public void Should_Throw_Null_Reference_Error()
        {
            var pq = CreatePQ();

            Assert.Throws<NullReferenceException>(() => pq.Enqueue(null));
        }

        [TestCase]
        public void Should_Throw_Not_Found_Error()
        {
            var pq = CreatePQ();

            Assert.Throws<KeyNotFoundException>(() => pq.Dequeue(priorities.Max() + 1));
        }

        [TestCase]
        public void Should_Return_Default_Object_On_Empty_Queue()
        {
            var pq = CreatePQ();

            Assert.AreEqual(defaultObject, pq.Dequeue());
        }

        [TestCase]
        public void Should_Count_All_Elements_In_Queue()
        {
            var pq = CreatePQ();
            foreach (var i in priorities)
                foreach (var j in elementsToEnque)
                    pq.Enqueue(j, i);

            var totalNumberOfElements = pq.GetLengthOfQueue();

            Assert.AreEqual(priorities.Count() * elementsToEnque.Count(), totalNumberOfElements);
        }

        public void Should_Get_All_Elements_In_Queue_With_Priority()
        {
            var pq = CreatePQ();
            foreach (var i in elementsToEnque)
                pq.Enqueue(i, priorities.Max());

            var elementsWithPrio = pq[priorities.Max()];
            Assert.AreEqual(elementsWithPrio, elementsToEnque);
            elementsWithPrio = pq.GetFullQueueWithPriority(priorities.Max());
            Assert.AreEqual(elementsWithPrio, elementsToEnque);
        }

        [TestCase]
        public void Should_Add_History_Events()
        {
            var pq = CreatePQ();
            foreach (var i in elementsToEnque)
                pq.Enqueue(i);

            Assert.True(pq.EventsHistory.Count() == elementsToEnque.Count);
        }

        [TestCase]
        public void Should_Add_History_Events_With_Enqueue_Action()
        {
            var pq = CreatePQ();
            elementsToEnque.ForEach(x => pq.Enqueue(x));

            Assert.True(pq.EventsHistory.All(x => x.Action == Actions.Enqueue));
            Assert.True(pq.EventsHistory.Count() == elementsToEnque.Count());
        }

        [TestCase]
        public void Should_Add_History_Events_With_Dequeue_Action()
        {
            var pq = CreatePQ();
            elementsToEnque.ForEach(x => pq.Enqueue(x, priorities.Max()));

            var countElementWithPrioMax = pq.GetLengthOfQueue(priorities.Max());
            for (var i = 0; i < countElementWithPrioMax; i++)
                pq.Dequeue();

            var events = pq.EventsHistory;
            Assert.True(events.Count(x => x.Action == Actions.Dequeue) == elementsToEnque.Count());
        }

        [TestCase]
        public void Should_See_New_Priority_In_List()
        {
            var pq = CreatePQ();

            pq.AddPriority(priorities.Max() + 1);
            pq.AddPriorities(priorities);

            Assert.True(pq.ExistingPriorities.Contains(priorities.Max() + 1));
            Assert.AreEqual(pq.ExistingPriorities.Count(), priorities.Count() + 1);

            pq.Enqueue(defaultElementForEnqueue, priorities.Max() + 2);

            Assert.True(pq.ExistingPriorities.Contains(priorities.Max() + 2));
            Assert.True(pq.ExistingPriorities.Count() == priorities.Count() + 2);
        }

        private PQ<string> CreatePQ()
        {
            return new PQ<string>(priorities, defaultObject);
        }
    }
}
