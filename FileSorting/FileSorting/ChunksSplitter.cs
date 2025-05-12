using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSorting.FileSorting
{
    public interface IChunksSplitter
    {
        Task<List<string>> SplitIntoChunksAsync(string inputFile, int chunkLinesCount);
    }
    public class ChunksSplitter : IChunksSplitter
    {
        private const int _blockingCollectionCapacity = 1000;

        private IComparer<LineContent> _comparer;
        private IParser<LineContent> _parser;

        public ChunksSplitter(IComparer<LineContent> comparer, IParser<LineContent> parser)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public async Task<List<string>> SplitIntoChunksAsync(string inputFile, int maxLinesInChunk)
        {
            var chunks = new BlockingCollection<List<LineContent>>(_blockingCollectionCapacity);
            var producer = Task.Run(() => FillChunksCollection(inputFile, maxLinesInChunk, chunks));

            List<string> tempFiles = await FillTempFiles(chunks);
            producer.Wait();

            return tempFiles;
        }

        private void FillChunksCollection(string inputFile, int maxLinesInChunk, BlockingCollection<List<LineContent>> chunks)
        {
            using var reader = new StreamReader(inputFile);
            var currentChunk = new List<LineContent>(maxLinesInChunk);
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                var parsed = _parser.Parse(line);

                currentChunk.Add(parsed);

                if (currentChunk.Count >= maxLinesInChunk)
                {
                    chunks.Add(currentChunk);
                    currentChunk = new List<LineContent>(maxLinesInChunk);
                }
            }

            if (currentChunk.Count > 0)
            {
                chunks.Add(currentChunk);
            }

            chunks.CompleteAdding();
        }

        private async Task<List<string>> FillTempFiles(BlockingCollection<List<LineContent>> chunks)
        {
            var writeTasks = new List<Task<string[]>>();

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                writeTasks.Add(Task.Run(() =>
                {
                    var tempFileNames = new List<string>();
                    foreach (var chunk in chunks.GetConsumingEnumerable())
                    {
                        string tempFileName = $"chunk_{Guid.NewGuid()}.tmp";
                        FillTempFile(tempFileName, chunk);
                        Console.WriteLine($"Created chunk {tempFileName} .");
                        tempFileNames.Add(tempFileName);
                    }
                    return tempFileNames.ToArray();
                }));
            }

            var allFileNames = await Task.WhenAll(writeTasks);
            return allFileNames.SelectMany(x => x).ToList();
        }

        private string FillTempFile(string tempFileName, List<LineContent> chunk)
        {
            chunk.Sort(_comparer);
            File.WriteAllLines(tempFileName, chunk.Select(x => $"{x.Number}: {x.Text}"));
            return tempFileName;
        }

    }
}
