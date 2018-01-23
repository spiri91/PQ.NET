using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using PQ.NET;
using System.Threading.Tasks;

namespace PQ.NET_Tests
{
    [TestFixture]
    public class PqTests
    {
        private readonly uint[] _priorities = new uint[] { 11, 22, 33, 44 };
        private string _defaultObject = "Empty queue";
        private string _defaultElementForEnqueue = "foo";
        private readonly List<string> _elementsToEnque = new List<string> { "foo", "boo", "goo", "voo" };

        private IPriorityQueue<string> pq;

        [SetUp]
        public void CreatePq()
        {
            pq = new Pq<string>(_priorities, _defaultObject);
        }

        [TestCase]
        public void Should_Create_PQ_Instance()
        {
            CreatePq();
        }

        [TestCase]
        public void Should_Enqueue_Element()
        {
            pq.Enqueue(_defaultElementForEnqueue);
            var dequeuedElement = pq.Dequeue();
            Assert.AreEqual(dequeuedElement, _defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Enqueue_Element_With_Prio()
        {
            var prio = _priorities[0];
            pq.Enqueue(_defaultElementForEnqueue, prio);
            var dequeuedElement = pq.Dequeue(prio);

            Assert.AreEqual(dequeuedElement, _defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Dequeue_Element()
        {
            pq.Enqueue(_defaultElementForEnqueue);
            var dequeuedElement = pq.Dequeue();
            Assert.AreEqual(dequeuedElement, _defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Dequeue_Element_With_Prio()
        {
            var prio = _priorities.Max();
            pq.Enqueue(_defaultElementForEnqueue, prio);
            var dequeuedElement = pq.Dequeue(prio);

            Assert.AreEqual(dequeuedElement, _defaultElementForEnqueue);
        }

        [TestCase]
        public void Should_Peek_Element()
        {
            pq.Enqueue(_defaultElementForEnqueue);
            var peekedElement = pq.Peek();
            var lengthOfQ = pq.GetLengthOfQueue();

            Assert.AreEqual(peekedElement, _defaultElementForEnqueue);
            Assert.AreEqual(1, lengthOfQ);
        }

        [TestCase]
        public void Should_Peek_Element_With_Prio()
        {
            var prio = _priorities.Min();
            pq.Enqueue(_defaultElementForEnqueue, prio);
            var peekedElement = pq.Peek(prio);
            var lengthOfQ = pq.GetLengthOfQueue(prio);

            Assert.AreEqual(peekedElement, _defaultElementForEnqueue);
            Assert.AreEqual(1, lengthOfQ);
        }

        [TestCase]
        public void Should_Add_Prio()
        {
            var newPrio = _priorities.Max() + 1;
            pq.AddPriorityLevel(newPrio);

            Assert.True(pq.ExistingPriorities.Contains(newPrio));
        }

        [TestCase]
        public void Should_Add_Prios()
        {
            var newPrios = Enumerable.Range((int)_priorities.Max() + 1, 10).Select(x => (uint)x);
            pq.AddPriorityLevels(newPrios);

            newPrios.ToList().ForEach(x => Assert.True(pq.ExistingPriorities.Contains(x)));
        }

        [TestCase]
        public void Should_Fire_Enqueue_Event()
        {
            var wasCalledEnqueuedEvent = false;
            var wasCalledDequeuedEvent = false;
            pq.ElementEnqueued += (a, b) => wasCalledEnqueuedEvent = true;
            pq.ElementDequeued += (a, b) => wasCalledDequeuedEvent = true;
            pq.Enqueue(_defaultElementForEnqueue);

            Assert.True(wasCalledEnqueuedEvent);
            Assert.False(wasCalledDequeuedEvent);
        }

        [TestCase]
        public void Should_Fire_Dequeue_Event()
        {
            var wasCalledEnqueuedEvent = false;
            var wasCalledDequeuedEvent = false;
            pq.Enqueue(_defaultElementForEnqueue);
            pq.ElementEnqueued += (a, b) => wasCalledEnqueuedEvent = true;
            pq.ElementDequeued += (a, b) => wasCalledDequeuedEvent = true;
            pq.Dequeue();

            Assert.True(wasCalledDequeuedEvent);
            Assert.False(wasCalledEnqueuedEvent);
        }

        [TestCase]
        public void Should_Add_Prio_With_Add_Element_Function()
        {
            var priority = _priorities.Max() + 1;
            pq.Enqueue(_defaultElementForEnqueue, priority);

            Assert.True(pq.ExistingPriorities.Contains(priority));
        }

        [TestCase]
        public void Should_Delete_All_Elements_With_Prio()
        {
            pq.AddPriorityLevels(_priorities);
            pq.Enqueue(_defaultElementForEnqueue, _priorities.Min());
            pq.DeleteAllElementsFromQueueWithPriorityLevel(_priorities.Min());

            Assert.IsTrue(pq.GetLengthOfQueue(_priorities.Min()) == 0);

            pq.Enqueue(_defaultElementForEnqueue, _priorities.Min());

            Assert.AreEqual(1, pq.GetLengthOfQueue(_priorities.Min()));
        }

        [TestCase]
        public void Should_Throw_Argument_Null_Error()
        {
            Assert.Throws<ArgumentNullException>(() => pq.Enqueue(null));
        }

        [TestCase]
        public void Should_Throw_Not_Found_Error()
        {
            Assert.Throws<KeyNotFoundException>(() => pq.Dequeue(_priorities.Max() + 1));
        }

        [TestCase]
        public void Should_Return_Default_Object_On_Empty_Queue()
        {
            Assert.AreEqual(_defaultObject, pq.Dequeue());
        }

        [TestCase]
        public void Should_Count_All_Elements_In_Queue()
        {
            foreach (var i in _priorities)
                foreach (var j in _elementsToEnque)
                    pq.Enqueue(j, i);

            var totalNumberOfElements = pq.GetLengthOfQueue();

            Assert.AreEqual(_priorities.Count() * _elementsToEnque.Count(), totalNumberOfElements);
        }

        public void Should_Get_All_Elements_In_Queue_With_Priority()
        {
            foreach (var i in _elementsToEnque)
                pq.Enqueue(i, _priorities.Max());

            var elementsWithPrio = pq[_priorities.Max()];
            elementsWithPrio = pq.GetFullQueueWithPriority(_priorities.Max());

            Assert.AreEqual(elementsWithPrio, _elementsToEnque);
            Assert.AreEqual(elementsWithPrio, _elementsToEnque);
        }

        [TestCase]
        public void Should_Add_History_Events()
        {
            foreach (var i in _elementsToEnque)
                pq.Enqueue(i);

            Assert.True(pq.EventsHistory.Count() == _elementsToEnque.Count);
        }

        [TestCase]
        public void Should_Add_History_Events_With_Enqueue_Action()
        {
            _elementsToEnque.ForEach(x => pq.Enqueue(x));

            Assert.True(pq.EventsHistory.All(x => x.Action == Actions.Enqueue));
            Assert.True(pq.EventsHistory.Count() == _elementsToEnque.Count());
        }

        [TestCase]
        public void Should_Add_History_Events_With_Dequeue_Action()
        {
            _elementsToEnque.ForEach(x => pq.Enqueue(x, _priorities.Max()));
            var countElementWithPrioMax = pq.GetLengthOfQueue(_priorities.Max());
            for (var i = 0; i < countElementWithPrioMax; i++)
                pq.Dequeue();

            var events = pq.EventsHistory;

            Assert.True(events.Count(x => x.Action == Actions.Dequeue) == _elementsToEnque.Count());
        }

        [TestCase]
        public void Should_See_New_Priority_In_List()
        {
            pq.AddPriorityLevel(_priorities.Max() + 1);
            pq.AddPriorityLevels(_priorities);

            Assert.True(pq.ExistingPriorities.Contains(_priorities.Max() + 1));
            Assert.AreEqual(pq.ExistingPriorities.Count(), _priorities.Count() + 1);

            pq.Enqueue(_defaultElementForEnqueue, _priorities.Max() + 2);

            Assert.True(pq.ExistingPriorities.Contains(_priorities.Max() + 2));
            Assert.True(pq.ExistingPriorities.Count() == _priorities.Count() + 2);
        }

        [TestCase]
        public void Should_Not_Loose_Elements_In_Queue()
        {
            _elementsToEnque.ForEach(x => pq.Enqueue(x, _priorities.Min()));
            pq.AddPriorityLevel(_priorities.Min());
            var elements = pq[_priorities.Min()];

            Assert.AreEqual(elements.Count(), _elementsToEnque.Count());
            Assert.True(pq.ExistingPriorities.Count() == _priorities.Count());
            Assert.True(pq.ExistingPriorities.Contains(_priorities.Min()));
        }

        [TestCase]
        public void Should_Return_Default_Object_On_Peek_Empty_Queue_With_Priority()
        {
            var obj = pq.Peek(_priorities.Max());

            Assert.AreSame(obj, _defaultObject);
        }

        [TestCase]
        public void Should_Return_Default_Object_On_Peek_Empty_Queue()
        {
            var obj = pq.Peek();

            Assert.AreSame(obj, _defaultObject);
        }

        [TestCase]
        public void Should_Broadcast_Same_Obj_On_Enqueue()
        {
            EventArgsContainer<string> dispatchedEvent = null;

            pq.ElementEnqueued += (sender, objWrapper) =>
            {
                dispatchedEvent = objWrapper as EventArgsContainer<string>;
            };

            pq.Enqueue(_defaultElementForEnqueue);

            Assert.IsTrue(dispatchedEvent.Obj.Action == Actions.Enqueue);
            Assert.AreSame(dispatchedEvent.Obj.Value, _defaultElementForEnqueue);
            Assert.AreEqual(dispatchedEvent.Obj.Priority, _priorities.Min());
        }

        [TestCase]
        public void Should_Brodcast_Save_Obj_On_Dequeue()
        {
            EventArgsContainer<string> dispatchedEvent = null;
            pq.ElementDequeued += (sender, objWrapper) =>
            {
                dispatchedEvent = objWrapper as EventArgsContainer<string>;
            };

            pq.Enqueue(_defaultElementForEnqueue);
            pq.Dequeue();

            Assert.IsTrue(dispatchedEvent.Obj.Action == Actions.Dequeue);
            Assert.AreSame(dispatchedEvent.Obj.Value, _defaultElementForEnqueue);
            Assert.AreEqual(dispatchedEvent.Obj.Priority, _priorities.Min());
        }

        [TestCase]
        public void Should_Dequeue_Elements_In_Priority_Descending_Order()
        {
            _elementsToEnque.ForEach(x => pq.Enqueue(x, _priorities.Max()));
            _elementsToEnque.ForEach(x =>
            {
                var obj = pq.Dequeue();

                Assert.AreSame(obj, x);
            });

            pq.Enqueue("goo", _priorities.Max() + 3);
            pq.Enqueue("moo", _priorities.Max() + 2);
            pq.Enqueue("roo", _priorities.Max() + 4);

            var _obj = pq.Dequeue();
            Assert.AreSame(_obj, "roo");

            _obj = pq.Dequeue();
            Assert.AreSame(_obj, "goo");

            _obj = pq.Dequeue();
            Assert.AreSame(_obj, "moo");
        }

        [TestCase]
        public void Should_Throw_Error_On_Invalid_Constructor()
        {
            Assert.Throws<ArgumentException>(() => new Pq<string>(new uint[0], _defaultObject));
            Assert.Throws<ArgumentNullException>(() => new Pq<string>(_priorities, null));
            Assert.Throws<ArgumentException>(() => new Pq<string>(new uint[0], null));
            Assert.Throws<ArgumentException>(() => new Pq<string>(null, _defaultObject));
        }

        [TestCase]
        public void Should_Not_Enqueue_Null_Elements()
        {
            Assert.Throws<ArgumentNullException>(() => pq.Enqueue(null));
            Assert.Throws<ArgumentNullException>(() => pq.Enqueue(null, _priorities.Max()));
        }

        [TestCase]
        public void Should_Throw_Exception_When_Dequeuing_Non_Existing_Queue()
        {
            Assert.Throws<KeyNotFoundException>(() => pq.Dequeue(_priorities.Max() + 1));
        }

        [TestCase]
        public void Should_Update_Min_Prio()
        {
            _elementsToEnque.ForEach(x => pq.Enqueue(x, _priorities.Min()));
            pq.Enqueue("Moo", _priorities.Min() - 1);
            EventArgsContainer<string> dispatchedEvent = null;
            pq.ElementEnqueued += (key, element) =>
            {
                dispatchedEvent = element as EventArgsContainer<string>;
            };

            pq.Enqueue("Moo2");

            Assert.True(dispatchedEvent.Obj.Priority == _priorities.Min() - 1);
            Assert.True(dispatchedEvent.Obj.Value == "Moo2");
        }

        [TestCase]
        public void Should_Update_Max_Prio()
        {
            _elementsToEnque.ForEach(x => pq.Enqueue(x, _priorities.Min()));
            pq.Enqueue("Moo", _priorities.Max() + 1);

            EventArgsContainer<string> dispatchedEvent = null;

            pq.ElementDequeued += (key, element) =>
            {
                dispatchedEvent = element as EventArgsContainer<string>;
            };

            pq.Dequeue();

            Assert.True(dispatchedEvent.Obj.Priority == _priorities.Max() + 1);
            Assert.True(dispatchedEvent.Obj.Value == "Moo");
        }

        [TestCase]
        public void Should_Enqueue_Same_Number_Of_Elements_On_Paralel()
        {
            uint range = _priorities.Max();
            var numberOfElements = 50000;
            new Task(() =>
               Enumerable.Range(1, numberOfElements).AsParallel().ToList()
               .ForEach(x => { pq.Enqueue(x.ToString(), range);
                   if (range % 2 == 0) range++; else range--; })
           ).RunSynchronously();

            Assert.IsTrue(pq.GetLengthOfQueue() == numberOfElements);
            Assert.IsTrue(pq.EventsHistory.Count == numberOfElements);
        }

        [TestCase]
        public void Should_Throw_Error_On_Peek_Non_Existing_Queue()
        {
            Assert.Throws<KeyNotFoundException>(
                () => pq.Peek(_priorities.Max() + 1));
        }

        [TestCase]
        public void Should_Throw_Error_On_Get_Length_Of_Non_Existing_Queue()
        {
            Assert.Throws<KeyNotFoundException>(
                () => pq.GetLengthOfQueue(_priorities.Max() + 1));
        }

        [TestCase]
        public void Should_Throw_Error_On_Delete_Non_Existing_Queue()
        {
            Assert.Throws<KeyNotFoundException>(
                () => pq.DeleteAllElementsFromQueueWithPriorityLevel(_priorities.Max() + 1));
        }

        [TestCase]
        public void Should_Not_Delete_Priority_Level_Once_With_The_Queue()
        {
            pq.DeleteAllElementsFromQueueWithPriorityLevel(_priorities.Min());

            Assert.True(pq.ExistingPriorities.Contains(_priorities.Min()));
            Assert.True(pq.GetLengthOfQueue(_priorities.Min()) == 0);

            _elementsToEnque.ForEach(x => pq.Enqueue(x));

            Assert.True(pq.GetLengthOfQueue(_priorities.Min()) == _elementsToEnque.Count);

            pq.DeleteAllElementsFromQueueWithPriorityLevel(_priorities.Min());

            Assert.True(pq.ExistingPriorities.Contains(_priorities.Min()));
            Assert.True(pq.GetLengthOfQueue(_priorities.Min()) == 0);
        }
    }
}
