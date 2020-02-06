using System;
using System.Threading;
using System.Collections.Generic;

namespace PEDollController.Threads
{

    // BlockingQueue<T> provides blocking version of Enqueue() & Dequeue(), implementing basic thread safety
    // Idea from: https://stackoverflow.com/a/530228

    class BlockingQueue<T> : Queue<T>
    {
        public void BlockingEnqueue(T item)
        {
            lock(this)
            {
                this.Enqueue(item);
                // If just recovered from empty state, send a pulse to the blocked GetCommand() thread
                if (this.Count == 1)
                    Monitor.PulseAll(this);
            }
        }

        public T BlockingDequeue()
        {
            lock(this)
            {
                if (this.Count == 0)
                    Monitor.Wait(this);
                return this.Dequeue();
            }
        }
    }
}
