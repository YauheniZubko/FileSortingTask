using FileSorting.FileGeneration;

namespace FileCreationDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"C:\Temp\test.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            var stringGeneratorOptions = new StringGeneratorOptions()
            {
                MaxWordLength = 20,
                MaxWordsInStringCount = 10,
                WordsSetCount = 500,
            };
            var stringGenerator = new StringGenerator(stringGeneratorOptions);
            var options = new FileGeneratorOptions()
            {
                FileSizeMB = 1024,
                BatchSizeBytes = 64 * 1024,
                ThreadCount = 1,
                BlockingCollectionCapacity = 10000
            };
            var fileGenerator = new FileGenerator(stringGenerator, options);
            Console.WriteLine($"Starting file generation");

            fileGenerator.GenerateFile(fileName);

            Console.WriteLine($"File {fileName} created. Size - {options.FileSizeMB}MB");
        }
    }
}
