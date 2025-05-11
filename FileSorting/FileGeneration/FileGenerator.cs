using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FileSorting.FileGeneration
{
    public interface IFileGenerator
    {
        void GenerateFile(string fileName);
    }

    public class FileGeneratorOptions
    {
        public int FileSizeMB { get; set; }
        public int BatchSizeBytes { get; set; }
        public int ThreadCount { get; set; }
        public int BlockingCollectionCapacity { get; set; }
    }

    public class FileGenerator : IFileGenerator
    {
        private readonly int _batchSize;
        private readonly int _fileSizeMB;
        private readonly int _threadCount;
        private readonly int _blockingCollectionCapacity;
        private IStringGenerator _stringGenerator;

        public FileGenerator(IStringGenerator stringGenerator, FileGeneratorOptions options)
        {
            ValidateOptions(options);

            _batchSize = options.BatchSizeBytes;
            _fileSizeMB = options.FileSizeMB;
            _threadCount = options.ThreadCount;
            _blockingCollectionCapacity = options.BlockingCollectionCapacity;

            _stringGenerator = stringGenerator;
        }

        public void GenerateFile(string fileName)
        {
            long remainingBytes = (long)_fileSizeMB * 1024 * 1024;

            var queue = new BlockingCollection<string>(_blockingCollectionCapacity);

            var writerTask = Task.Run(() =>
            {
                using var writer = new StreamWriter(fileName);
                foreach (var line in queue.GetConsumingEnumerable())
                {
                    writer.Write(line);
                }
            });

            var producerTasks = new Task[_threadCount];
            for (int i = 0; i < _threadCount; i++)
            {
                producerTasks[i] = Task.Run(() =>
                {
                    while (true)
                    {
                        var sb = new StringBuilder(_batchSize * 2);
                        while (sb.Length < _batchSize)
                        {
                            sb.AppendLine(_stringGenerator.GenerateString());
                        }

                        string batch = sb.ToString();
                        int byteCount = Encoding.UTF8.GetByteCount(batch);
                        long newRemaining = Interlocked.Add(ref remainingBytes, -byteCount);

                        if (newRemaining >= 0)
                        {
                            queue.Add(batch);
                        }
                        else
                        {
                            break;
                        }
                    }
                });
            }

            Task.WaitAll(producerTasks);
            queue.CompleteAdding();
            writerTask.Wait();
        }
        private static void ValidateOptions(FileGeneratorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.BatchSizeBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.BatchSizeBytes), "BatchSize must be greater than 0.");
            if (options.FileSizeMB <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.FileSizeMB), "FileSizeMB must be greater than 0.");
            if (options.ThreadCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.ThreadCount), "ThreadCount must be greater than 0.");
            if (options.BlockingCollectionCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.BlockingCollectionCapacity), "BlockingCollectionCapacity must be greater than 0.");
        }
    }
}
