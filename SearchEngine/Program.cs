// Author : Puneet Sharma
// Copyright (C) 2016, Stevens Institute of Technology
// Created : 12/06/2016 15:17

using System;

namespace SearchEngine
{
    internal class Program
    {
        private const string Shell = "search> ";

        private static void Main(string[] args)
        {
            var engine = new Engine();
            engine.Process();
            Cli(engine);
        }

        private static void Cli(Engine engine)
        {
            PrintHelp();
            Console.Write(Shell);
            var maxDepth = 1;
            while (true)
            {
                var input = Console.ReadLine();
                if (input.StartsWith("-"))
                {
                    input = input.Substring(1);
                    var parts = input.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                    var cmd = parts[0];
                    switch (cmd)
                    {
                        case "help":
                            PrintHelp();
                            break;
                        case "quit":
                            return;
                        case "depth":
                            int depth;
                            if (parts.Length < 2 || !int.TryParse(parts[1], out depth) || depth < 0)
                            {
                                PrintHelp();
                            }
                            else
                            {
                                maxDepth = depth;
                                Console.WriteLine("Crawl depth set to {0}", maxDepth);
                            }
                            break;
                        case "index":
                            Uri uri;
                            if (parts.Length < 2 || !Uri.TryCreate(parts[1], UriKind.Absolute, out uri))
                            {
                                PrintHelp();
                            }
                            else
                            {
                                engine.Index(uri, maxDepth);
                            }
                            break;
                        default:
                            PrintHelp();
                            break;
                    }
                }
                else if (input == "")
                {
                }
                else
                {
                    engine.Search(input);
                }
                Console.Write(Shell);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("The application searches the entered text.");
            Console.WriteLine("Commands can be executed by using '-'");
            Console.WriteLine("Following commands are available:");
            Console.WriteLine("\t-depth <num>\t\tNon-negative number to set link crawl depth (default is 0)");
            Console.WriteLine("\t\t\t\t'-depth 2' will crawl start->depth1->depth2");
            Console.WriteLine("\t-index <url> \t\tex. -index https://en.wikipedia.org/wiki/Binary_search_algorithm");
            Console.WriteLine("\t-quit \t\t\tTo exit the application.");
            Console.WriteLine("\t-help \t\t\tDisplay this message.");
            Console.WriteLine();
        }
    }
}