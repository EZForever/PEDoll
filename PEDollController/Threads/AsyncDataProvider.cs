using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PEDollController.Threads
{

    // AsyncDataProvider<T> converts any infinite blocking data fetch operation into a series of await-able Tasks
    // Idea from: https://stackoverflow.com/a/54443866

    /*
     * Usage example:
     * AsyncDataProvider<string> provider = new AsyncDataProvider<string>(Console.ReadLine);
     * ...
     * while(true)
     * {
     *     Task<string> task = provider.Get();
     *     int idx = Task.WaitAny(task, ...);
     *     
     *     if(idx == 0)
     *     {
     *         string line = task.Result;
     *         ...
     *     }
     *     else
     *     {
     *         ....
     *     }
     * }
     */

    class AsyncDataProvider<T>
    {

        IEnumerator<Task<T>> getResultEnumerator;

        static IEnumerator<Task<T>> LoopEnumerator(Func<T> func)
        {
            while (true)
            {
                Task<T> task = new Task<T>(func);
                task.Start();
                yield return task;
            }
        }

        public AsyncDataProvider(Func<T> getResult)
        {
            getResultEnumerator = LoopEnumerator(getResult);
            getResultEnumerator.MoveNext();
        }

        public Task<T> Get()
        {
            if (getResultEnumerator.Current.IsCompleted)
                getResultEnumerator.MoveNext();

            return getResultEnumerator.Current;
        }

    }

}
