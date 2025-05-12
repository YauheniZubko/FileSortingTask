using FileSorting.FileSorting;
using System.Diagnostics;

namespace FileGeneratorTask
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var (fileName, sortedFileName, chunkLineCount) = ParseArguments(args);
            
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            var comparer = new LinesContentComparer();
            var parser = new LineContentParser();
            var chunksSplitter = new ChunksSplitter(comparer, parser);
            var chunksMerger = new ChunksMerger(comparer, parser);
            var manager = new FileSorter(comparer, parser, chunksSplitter, chunksMerger);

            Console.WriteLine($"File {fileName} starts sorting.");

            var sw = Stopwatch.StartNew();

            await manager.Sort(fileName, sortedFileName, chunkLineCount);

            Console.WriteLine($"Sorting Finished. Sorted file - {sortedFileName} . Time: {sw.Elapsed.TotalSeconds} s");
        }

        private static (string fileName, string sortedFileName, int chunkLineCount) ParseArguments(string[] args)
        {
            string fileName = @"C:\Temp\test.txt";
            string sortedFileName = @"C:\Temp\sorted_test.txt";
            int chunkLineCount = 500000;

            foreach (var arg in args)
            {
                var split = arg.Split('=');
                if (split.Length != 2)
                {
                    continue;
                }

                var key = split[0].Trim().ToLowerInvariant();
                var value = split[1].Trim();

                switch (key)
                {
                    case "filename":
                        fileName = value;
                        break;
                    case "sortedfilename":
                        sortedFileName = value;
                        break;
                    case "chunklinecount":
                        if (int.TryParse(value, out int parsedChunkLineCount))
                        {
                            chunkLineCount = parsedChunkLineCount;
                        }
                        break;
                }
            }

            return (fileName, sortedFileName, chunkLineCount);
        }

    }
}
