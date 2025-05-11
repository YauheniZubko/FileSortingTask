namespace FileSorting.FileSorting
{
    public class FileSorter
    {
        private readonly IComparer<LineContent> _comparer;
        private readonly IParser<LineContent> _parser;
        private readonly IChunksSplitter _chunksSplitter;
        private readonly IChunksMerger _chunksMerger;

        public FileSorter(
            IComparer<LineContent> comparer,
            IParser<LineContent> parser,
            IChunksSplitter chunksSplitter,
            IChunksMerger chunksMerger)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _chunksSplitter = chunksSplitter ?? throw new ArgumentNullException(nameof(chunksSplitter));
            _chunksMerger = chunksMerger ?? throw new ArgumentNullException(nameof(chunksMerger));
        }

        public async Task Sort(string inputPath, string outputPath, int chunkLineCount)
        {
            Console.WriteLine($"Start splitting. Lines in chunk - {chunkLineCount}");
            var tempFiles = await _chunksSplitter.SplitIntoChunksAsync(inputPath, chunkLineCount);
            Console.WriteLine($"Split into {tempFiles.Count} chunks. Start merging.");

            try
            {
                _chunksMerger.MergeSortedChunks(tempFiles, outputPath);
            }
            finally
            {
                foreach (var file in tempFiles)
                {
                    File.Delete(file);
                }
            }
        }
    }
}
