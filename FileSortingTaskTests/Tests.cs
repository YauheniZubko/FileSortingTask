using FileSorting.FileGeneration;
using FileSorting.FileSorting;


[TestFixture]
public class ExternalSortTests
{
    IComparer<LineContent> comparer;
    IParser<LineContent> parser;
    IChunksSplitter chunksSplitter;
    IChunksMerger chunksMerger;
    FileSorter sortManger;

    [SetUp]
    public void Setup()
    {
        comparer = new LinesContentComparer();
        parser = new LineContentParser();
        chunksSplitter = new ChunksSplitter(comparer, parser);
        chunksMerger = new ChunksMerger(comparer, parser);
        sortManger = new FileSorter(comparer, parser, chunksSplitter, chunksMerger);
    }

    [Test]
    public async Task File_Is_Sorted_Correctly()
    {
        string fileName = "test.txt";
        int fileSizeMB = 150;
        int chunkLineCount = 500000;
        GenerateFile(fileName, fileSizeMB);
        var sortedFileName = "sorted_" + fileName;

        await sortManger.Sort(fileName, sortedFileName, chunkLineCount);

        Assert.That(new FileInfo(sortedFileName).Length, Is.EqualTo(new FileInfo(fileName).Length));
        VerifyFileSorted(sortedFileName);
    }

    private void VerifyFileSorted(string sortedFileName)
    {
        using (var reader = new StreamReader(sortedFileName))
        {
            LineContent? prev = null;
            LineContent current;
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                var str = reader.ReadLine();
                if (str == null)
                {
                    break;
                }
                current = parser.Parse(str);
                if (prev != null)
                {
                    Assert.LessOrEqual(comparer.Compare(prev.Value, current), 0, $"Line '{current}' is not in correct order after '{prev}'");
                }
                prev = current;
            }
        }
    }

    private static void GenerateFile(string fileName, int fileSizeMB)
    {
        var stringGeneratorOptions = new StringGeneratorOptions()
        {
            MaxWordLength = 20,
            MaxWordsInStringCount = 10,
            WordsSetCount = 500,
        };
        var stringGenerator = new StringGenerator(stringGeneratorOptions);
        var options = new FileGeneratorOptions()
        {
            FileSizeMB = fileSizeMB,
            BatchSizeBytes = 64 * 1024,
            ThreadCount = 1
        };
        var fileGenerator = new FileGenerator(stringGenerator, options);
        fileGenerator.GenerateFile(fileName);
    }
}
