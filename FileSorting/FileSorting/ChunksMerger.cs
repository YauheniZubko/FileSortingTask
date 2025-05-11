using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSorting.FileSorting
{
    public interface IChunksMerger
    {
        void MergeSortedChunks(IEnumerable<string> tempFileNames, string outputPath);
    }

    public class ChunksMerger : IChunksMerger
    {
        private IComparer<LineContent> _comparer;
        private IParser<LineContent> _parser;

        public ChunksMerger(IComparer<LineContent> comparer, IParser<LineContent> parser)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public void MergeSortedChunks(IEnumerable<string> tempFileNames, string outputPath)
        {
            var readers = tempFileNames.Select(File.OpenText).ToList();
            try
            {
                var heap = new SortedDictionary<LineContent, List<int>>(_comparer);
                FillResultFile(outputPath, readers, heap);
            }
            finally
            {
                foreach (var reader in readers)
                {
                    reader.Dispose();
                }
            }
        }

        private void FillResultFile(string outputPath, List<StreamReader> readers, SortedDictionary<LineContent, List<int>> heap)
        {
            for (int i = 0; i < readers.Count; i++)
            {
                AddReaderLineToHeap(readers[i], i, heap);
            }

            using var writer = new StreamWriter(outputPath);

            while (heap.Count > 0)
            {
                var first = heap.First();
                LineContent lineContent = first.Key;
                var readerIndexes = first.Value;

                heap.Remove(lineContent);

                foreach (var readerIndex in readerIndexes)
                {
                    writer.WriteLine($"{lineContent.Number}: {lineContent.Text}");
                    AddReaderLineToHeap(readers[readerIndex], readerIndex, heap);
                }
            }
        }

        private void AddReaderLineToHeap(StreamReader reader, int readerIndex, SortedDictionary<LineContent, List<int>> heap)
        {
            var line = reader.ReadLine();
            if (line != null)
            {
                var lineContent = _parser.Parse(line);

                if (!heap.TryGetValue(lineContent, out var list))
                {
                    list = new List<int>();
                    heap[lineContent] = list;
                }
                list.Add(readerIndex);
            }
        }
    }
}
