# PQ.NET

Priority queue for net standard v1.3 and up, thread safe, no 3-rd party dependencies.

## Installing

Package manager: Install-Package PQ.NET  
.NET CLI: dotnet add package PQ.NET
 
## Prerequisites

No prerequisites.

## Usage

instantiation: 
```
var pq = new Pq<T>(new uint[], defaultObjectReturnedInCaseTheQueueIsEmpty);
```

enqueuing:
```
pq.Enqueue(T obj); => will enqueue on the lowest level of priority
pq.Enqueue(T obj, uint newLevelOfPriority);
```

dequeuing:
```
pq.Dequeue(); => will dequeue from the max level priority downwards
pq.Dequeue(uint priorityLevel);
````

peeking:
```
var peekedElement = pq.Peek();
var peekedElement = pq.Peek(priorityLevel);
```

adding new priority level:
```
pq.AddPriorityLevel(newPriorityLevel);
```

adding new priority levels:
```
pq.AddPriorityLevels(newPriorityLevels); where newPriorityLevels = uint[]
```

adding new priority level with enqueue method:
```
pq.Enqueue(_defaultElementForEnqueue, newPriorityLevel);
```

deleting all elements with priority level:
```
pq.DeleteAllElementsFromQueueWithPriorityLevel(priorityLevel);
```

counting elements in queue
```
pq.GetLengthOfQueue() => full length including all priority levels
pq.GetLengthOfQueue(priorityLevel) only the priority level specified
```

## Events, and events sourcing
registering for an enqueuing event: 
```
pq.ElementEnqueued += (sender, objWrapper) =>
            {
                dispatchedEvent = objWrapper as EventArgsContainer<T>;
            };
```

registering for dequeuing event:
```
pq.ElementDequeued += (sender, objWrapper) =>
            {
                dispatchedEvent = objWrapper as EventArgsContainer<T>;
            };
```

properties:
```
pq.ExistingPriorities => IList<uint> of all currently existing levels of priority;
```

## Event History
Each action for enqueue or dequeue will be registerd here in pq.EventsHistory as PqEvent

structure of PqEvent:
```
 public class PqEvent<T>
    {
        public Actions Action { get; } => Enum with two elements Enqueue and Dequeue;
        public T Value { get; }
        public DateTimeOffset TimeItHappened { get; }
        public uint Priority { get; }

        public PqEvent(Actions action, T value, uint priority)
        {
            Action = action;
            Value = value;
            TimeItHappened = DateTimeOffset.UtcNow;
            Priority = priority;
        }
    }
```	
This adding is done in method: AddEventToHistory, method that can be overriden if history becomes to big

## Event broadcasting
```
public class EventArgsContainer<T> : EventArgs
    {
        public PqEvent<T> Obj { get; }

        public EventArgsContainer(PqEvent<T> obj)
        {
            Obj = obj;
        }
    }
```

Just cast EventArgs e to EventArgsContainer<T> and you will have the element that triggered the event.

## Each method has a good documentation in code also , use it wisely

