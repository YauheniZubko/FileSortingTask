using FileSorting.FileGeneration;

namespace FileCreationDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var (fileName, fileSizeMB, batchSizeBytes, threadCount) = ParseArguments(args);

            Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);

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
                BatchSizeBytes = batchSizeBytes,
                ThreadCount = threadCount
            };
            var fileGenerator = new FileGenerator(stringGenerator, options);
            Console.WriteLine($"Starting file generation");

            fileGenerator.GenerateFile(fileName);

            Console.WriteLine($"File {fileName} created. Size - {options.FileSizeMB}MB");
        }

        private static (string fileName, int fileSizeMB, int batchSizeBytes, int threadCount) ParseArguments(string[] args)
        {
            // Default values
            string fileName = @"C:\Temp\test.txt";
            int fileSizeMB = 1024;
            int batchSizeBytes = 64 * 1024;
            int threadCount = 1;

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
                    case "filesizemb":
                        if (int.TryParse(value, out int mb))
                        {
                            fileSizeMB = mb;
                        }
                        break;
                    case "batchsizebytes":
                        if (int.TryParse(value, out int bsb))
                        {
                            batchSizeBytes = bsb;
                        }
                        break;
                    case "threadcount":
                        if (int.TryParse(value, out int tc))
                        {
                            threadCount = tc;
                        }
                        break;
                }
            }

            return (fileName, fileSizeMB, batchSizeBytes, threadCount);
        }
    }
}
