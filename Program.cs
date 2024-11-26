using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static SemaphoreSlim semaphore = new SemaphoreSlim(3);

    private const int MAXREQUEST = 3;

    private static int activeRequests = 0;

    private static readonly object lockObject = new object();

    static async Task Main(string[] args)
    {
        Task[] tasks = new Task[10];

        for (int i = 1; i <= 10; i++)
        {
            int requestId = i;

            tasks[i - 1] = ProcesSemaphoreAsync(requestId);
        }

        await Task.WhenAll(tasks);

        Print("The security guard tasted all products, bottling was successful. Denaturat not found, security guard also", ConsoleColor.Yellow);

        for (int i = 1; i <= 10; i++)
        {
            int requestId = i;

            tasks[i - 1] = ProcessMonitorAsync(requestId);
        }

        await Task.WhenAll(tasks);

        Print("The security guard tasted all products, bottling was successful. Denaturat not found, security guard also", ConsoleColor.Blue);
    }

    private static async Task ProcesSemaphoreAsync(int requestId)
    {
        //Print($"[{DateTime.Now:HH:mm:ss}] Request {requestId} wait access to the criticat access ", ConsoleColor.Yellow);

        await semaphore.WaitAsync();

        try
        {
            Print($"[{DateTime.Now:HH:mm:ss}] Request {requestId} start", ConsoleColor.Yellow);

            Random random = new Random();

            int delay = random.Next(1000, 3001);

            await Task.Delay(delay);

            Print($"[{DateTime.Now:HH:mm:ss}] Request {requestId} Finish", ConsoleColor.Yellow);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static async Task ProcessMonitorAsync(int requestId)
    {
        //Print($"[{DateTime.Now:HH:mm:ss}] Request {requestId} wait access to the criticat access ", ConsoleColor.Blue);

        lock (lockObject)
        {
            while (activeRequests >= MAXREQUEST)
            {
                Monitor.Wait(lockObject);
            }

            activeRequests++;
        }
        
        try
        {
            Print($"[{DateTime.Now:HH:mm:ss}] Request {requestId} start", ConsoleColor.Blue);

            Random random = new Random();

            int delay = random.Next(1000, 3001);

            await Task.Delay(delay);

            Print($"[{DateTime.Now:HH:mm:ss}] Request {requestId} finish", ConsoleColor.Blue);
        }
        finally
        {
            lock (lockObject)
            {
                activeRequests--;

                Monitor.Pulse(lockObject);
            }
        }
    }

    private static void Print(string message, ConsoleColor color)
    {
        ConsoleColor originalColor = Console.ForegroundColor;

        Console.ForegroundColor = color;

        Console.WriteLine(message);

        Console.ForegroundColor = originalColor;
    }
}
