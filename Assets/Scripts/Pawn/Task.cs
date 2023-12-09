using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
    public enum Status { Pending, InProgress, Completed, Failed, Cancelled }
    public abstract class Task
    {
        public Status TaskStatus { get; private set; } = Status.Pending;
        public int Priority { get; set; }
        public List<Task> Prerequisites { get; } = new List<Task>();

        public event Action<Task> OnStart;
        public event Action<Task> OnComplete;
        public event Action<Task> OnFail;
        public event Action<Task> OnCancel;

        public float Progress { get; protected set; }
        public event Action<Task> OnProgressUpdate;

        public TimeSpan Timeout { get; set; }


        public async void Execute(Pawn pawn)
        {
            if (Prerequisites.Any(t => t.TaskStatus != Status.Completed))
            {
                Fail();
                return;
            }

            TaskStatus = Status.InProgress;
            OnStart?.Invoke(this);

            var task = System.Threading.Tasks.Task.Run(() => Perform(pawn));
            if (await System.Threading.Tasks.Task.WhenAny(task, System.Threading.Tasks.Task.Delay(Timeout)) == task)
            {
                // Task completed within timeout
                Complete();
            }
            else
            {
                // Task timed out
                Fail();
            }
        }

        protected abstract void Perform(Pawn pawn);

        protected void UpdateProgress(float progress)
        {
            Progress = progress;
            OnProgressUpdate?.Invoke(this);
        }
        private void Complete()
        {
            TaskStatus = Status.Completed;
            OnComplete?.Invoke(this);
        }

        private void Fail()
        {
            TaskStatus = Status.Failed;
            OnFail?.Invoke(this);
        }

        public void Cancel()
        {
            TaskStatus = Status.Cancelled;
            OnCancel?.Invoke(this);
        }


        public Task(int priority, TimeSpan timeout, List<Task> prerequisites = null)
        {
            Priority = priority;
            Timeout = timeout;
            if (prerequisites != null)
            {
                Prerequisites.AddRange(prerequisites);
            }
        }

    }
}