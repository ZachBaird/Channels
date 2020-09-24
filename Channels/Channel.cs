using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Channels
{
    /// <summary>
    /// A channel class. A channel that is essentially a queue that regulates how many items
    /// can be in the queue, or how many can be in the queue. All of this is done in an 
    /// asynchronous manner.
    /// </summary>
    /// <typeparam name="T">Data of some type that will be in the channel.</typeparam>
    public class Channel<T> : IRead<T>, IWrite<T>
    {
        private bool Finished;

        private ConcurrentQueue<T> _queue;
        private SemaphoreSlim _gate;

        public Channel()
        {
            _queue = new ConcurrentQueue<T>();
            _gate = new SemaphoreSlim(0);
        }

        /// <summary>
        /// Pushes the message into the channel.
        /// </summary>
        /// <param name="msg">The message being pushed into the channel.</param>
        public void Push(T msg)
        {
            // We push the message onto the channel and say it's ready for collection.
            _queue.Enqueue(msg);
            _gate.Release();
        }

        /// <summary>
        /// Reads a message from the channel if it exists.
        /// </summary>
        /// <returns>A message that was in the channel.</returns>
        public async Task<T> Read()
        {
            // If there is a thread on the queue, it'll come in here and read out
            //  a message.
            await _gate.WaitAsync();

            if (_queue.TryDequeue(out var msg))
            {
                return msg;
            }

            // Returns a default value if a message wasn't found in the channel.
            return default;
        }

        /// <summary>
        /// Method that completes the channel.
        /// </summary>
        public void Complete()
        {
            Finished = true;
        }

        /// <summary>
        /// Returns whether the channel has completed and its queue is empty.
        /// </summary>
        /// <returns>A boolean indicating if the channel is finished.</returns>
        public bool IsComplete()
        {
            return Finished && _queue.IsEmpty;
        }
    }
}
