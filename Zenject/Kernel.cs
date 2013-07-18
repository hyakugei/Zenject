using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    public interface ITickable
    {
        int TickPriority { get; }

        void Update();
    }

    public class Kernel
    {
        LinkedList<TickableInfo> _tasks = new LinkedList<TickableInfo>();
        List<ITickable> _queuedTasks;

        public Kernel(List<ITickable> tasks)
        {
            _queuedTasks = tasks;
        }

        public void AddTask(ITickable task)
        {
            ZenUtil.Assert(!_queuedTasks.Contains(task), "Duplicate task added to kernel: " + task.GetType().FullName);
            ZenUtil.Assert(!_tasks.Any(t => ReferenceEquals(t.Tickable, task)), "Duplicate task added to kernel");

            // Wait until next frame to add the task, otherwise whether it gets updated
            // on the current frame depends on where in the update order it was added
            // from, so you might get off by one frame issues
            _queuedTasks.Add(task);
        }

        public void RemoveTask(ITickable task)
        {
            var info = _tasks.FirstOrDefault(i => ReferenceEquals(i.Tickable, task));
            ZenUtil.Assert(info != null);
            ZenUtil.Assert(!info.IsRemoved, "Tried to remove a task twice");

            info.IsRemoved = true;
        }

        public void Update()
        {
            AddQueuedTasks();

            foreach (var taskInfo in _tasks)
            {
                taskInfo.Tickable.Update();
            }

            ClearRemovedTasks();
        }

        void ClearRemovedTasks()
        {
            var node = _tasks.First;

            while (node != null)
            {
                var next = node.Next;
                var info = node.Value;

                if (info.IsRemoved)
                {
                    Debug.Log("Removed task '" + info.Tickable.GetType().ToString() + "'");
                    _tasks.Remove(node);
                }

                node = next;
            }           
        }

        void AddQueuedTasks()
        {
            foreach (var task in _queuedTasks)
            {
                InsertTaskSorted(task);
            }
            _queuedTasks.Clear();
        }

        void InsertTaskSorted(ITickable task)
        {
            var newInfo = new TickableInfo(task);

            for (var current = _tasks.First; current != null; current = current.Next)
            {
                if (current.Value.Tickable.TickPriority > task.TickPriority)
                {
                    _tasks.AddBefore(current, newInfo);
                    return;
                }
            }

            _tasks.AddLast(newInfo);
        }

        class TickableInfo
        {
            public ITickable Tickable;
            public bool IsRemoved;

            public TickableInfo(ITickable tickable)
            {
                Tickable = tickable;
            }
        }
    }
}
