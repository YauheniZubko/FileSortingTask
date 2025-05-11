using FileSorting.FileSorting;
using System.Diagnostics;

namespace FileGeneratorTask
{
    internal class Program
    {
        static async Task Main()
        { 
            int chunkLineCount = 500000;
            string fileName = @"C:\Temp\test.txt";

            var comparer = new LinesContentComparer();
            var parser = new LineContentParser();
            var chunksSplitter = new ChunksSplitter(comparer, parser);
            var chunksMerger = new ChunksMerger(comparer, parser);
            var manager = new FileSorter(comparer, parser, chunksSplitter, chunksMerger);

            string sortedFileName = @"C:\Temp\sorted_test.txt";

            Console.WriteLine($"File {fileName} starts sorting.");

            var sw = Stopwatch.StartNew();

            await manager.Sort(fileName, sortedFileName, chunkLineCount);

            Console.WriteLine($"Sorting Finished. Sorted file - {sortedFileName} . Time: {sw.Elapsed.TotalSeconds} s");
        }

    }
}
