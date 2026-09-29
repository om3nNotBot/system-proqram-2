using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static Dictionary<int, Thread> threads = new Dictionary<int, Thread>();

    static Dictionary<int, CancellationTokenSource> tokens = new Dictionary<int, CancellationTokenSource>();

    static void Main()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("Go To Files - 1");
            Console.WriteLine("Exit         - 0");
            Console.Write("Choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
                FilesMenu();
            else if (choice == "0")
                running = false;
        }

        StopAll();
        Console.WriteLine("Program terminated. Press Enter to close.");
        Console.ReadLine();
    }

    static void FilesMenu()
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine("file 1");
            Console.WriteLine("file 2");
            Console.WriteLine("file 3");
            Console.WriteLine();
            Console.WriteLine("write no of file start (1, 2 or 3)");
            Console.WriteLine("s-1     -> stop file 1");
            Console.WriteLine("s-2     -> stop file 2");
            Console.WriteLine("s-3     -> stop file 3");
            Console.WriteLine("stopall -> stop all files");
            Console.WriteLine("exit    -> back to main menu");
            Console.Write("Command: ");

            string cmd = (Console.ReadLine() ?? "").Trim().ToLower();

            switch (cmd)
            {
                case "1":
                case "2":
                case "3":
                    StartFile(int.Parse(cmd));
                    break;

                case "s-1":
                    StopFile(1);
                    break;

                case "s-2":
                    StopFile(2);
                    break;

                case "s-3":
                    StopFile(3);
                    break;

                case "stopall":
                    StopAll();
                    break;

                case "exit":
                    inMenu = false;
                    break;

                default:
                    Console.WriteLine("Unknown command. Press Enter to continue...");
                    Console.ReadLine();
                    break;
            }
        }
    }
    static void StartFile(int fileNo)
    {
        if (threads.ContainsKey(fileNo) && threads[fileNo].IsAlive)
        {
            Console.WriteLine($"File {fileNo} is already running.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        var cts = new CancellationTokenSource();
        tokens[fileNo] = cts;

        Thread t = new Thread(() => FileWork(fileNo, cts.Token));
        t.IsBackground = true;
        threads[fileNo] = t;
        t.Start();

        Console.WriteLine($"File {fileNo} started.");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    static void FileWork(int fileNo, CancellationToken token)
    {
        int step = 0;
        while (!token.IsCancellationRequested)
        {
            step++;
            Console.WriteLine($"[File {fileNo}] working... step {step}");
            Thread.Sleep(1000);
        }
        Console.WriteLine($"[File {fileNo}] stopped.");
    }

    static void StopFile(int fileNo)
    {
        if (tokens.ContainsKey(fileNo) && threads[fileNo].IsAlive)
        {
            tokens[fileNo].Cancel();
            Console.WriteLine($"Stop signal sent to file {fileNo}.");
        }
        else
        {
            Console.WriteLine($"File {fileNo} is not running.");
        }
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    static void StopAll()
    {
        foreach (var kv in tokens)
        {
            if (threads[kv.Key].IsAlive)
                kv.Value.Cancel();
        }
        Console.WriteLine("Stop signal sent to all files.");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}