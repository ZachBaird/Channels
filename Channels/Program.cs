using System;
using System.Threading.Tasks;

namespace Channels
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = new Channel<string>();

            Task.WaitAll(Consumer(channel), 
                         Producer<string>(channel),
                         Producer<string>(channel),
                         Producer<string>(channel),
                         Producer<string>(channel));
        }

        // This isn't async because it's unnecessary, but writing it as a task allows
        //  us to run it in parallel.
        public static async Task Producer<T>(IWrite<string> writer)
        {
            for (int i = 0; i < 10; i++)
            {
                writer.Push(i.ToString());
                await Task.Delay(100);
            }

            writer.Complete();
        }

        // This will be async because the reader has a task in it.
        public static async Task Consumer(IRead<string> reader)
        {
            while(!reader.IsComplete())
            {
                var msg = await reader.Read();
                Print(msg);
            }
        }

        static void Print(string msg) => Console.WriteLine(msg);
    }
}
