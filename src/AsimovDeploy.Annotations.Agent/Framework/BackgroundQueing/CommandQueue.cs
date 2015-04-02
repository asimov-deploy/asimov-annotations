using System.Collections.Concurrent;
using System.Threading;
using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public class ConcurrentCommandQueue : ICommandQueue
    {
        private readonly ConcurrentQueue<AsimovCommand> _queue;
        private int count = 0;
        public ConcurrentCommandQueue()
        {
            _queue = new ConcurrentQueue<AsimovCommand>();
        }

        public void EnqueueCommand(AsimovCommand command)
        {
            _queue.Enqueue(command);
            Interlocked.Increment(ref count);
        }

        public AsimovCommand Dequeue()
        {
            AsimovCommand command;
            if (_queue.TryDequeue(out command))
            {
                Interlocked.Decrement(ref count);
                return command;
            }
            return null;
        }

        public int Count
        {
            get { return count; }
        }
    }
}