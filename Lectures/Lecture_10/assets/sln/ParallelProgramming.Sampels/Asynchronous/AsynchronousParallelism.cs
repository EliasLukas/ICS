﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ParallelProgramming.Samples.Asynchronous
{
    public class AsynchronousParallelism(ITestOutputHelper output)
    {
        private readonly SemaphoreSlim _testOutputSemaphore = new(1);

        private void PrintCurrentThreadInfo()
        {
            _testOutputSemaphore.Wait();
            try
            {
                Thread th = Thread.CurrentThread;
                output.WriteLine("Managed thread #{0}: ", th.ManagedThreadId);
                output.WriteLine("   Background thread: {0}", th.IsBackground);
                output.WriteLine("   Thread pool thread: {0}", th.IsThreadPoolThread);
                output.WriteLine("   Priority: {0}", th.Priority);
                output.WriteLine("   Culture: {0}", th.CurrentCulture.Name);
                output.WriteLine("   UI culture: {0}", th.CurrentUICulture.Name);
            }
            finally
            {
                _testOutputSemaphore.Release();
            }
        }


        [Fact]
        public async Task AsynchronousTaskRunSample()
        {
            IList<Task> taskPool = new List<Task>();

            output.WriteLine("Main thread");
            PrintCurrentThreadInfo();

            for (var i = 0; i < 40; i++)
            {
                taskPool.Add(Task.Run(async () =>
                {
                    output.WriteLine("Task started");
                    PrintCurrentThreadInfo();
                    await Task.Delay(1000); 
                }));
            }

            await Task.WhenAll(taskPool);
        }
    }
}
