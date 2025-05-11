using System.Numerics;
using System.Text;

namespace FileSorting.FileGeneration
{
    public interface IStringGenerator
    {
        string GenerateString();
    }

    public class StringGeneratorOptions
    {
        public int WordsSetCount { get; set; }
        public int MaxWordLength { get; set; }
        public int MaxWordsInStringCount { get; set; }
    }

    public class StringGenerator : IStringGenerator
    {
        private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly string[] wordsSet;
        private readonly Random random = Random.Shared;
        private readonly int _wordsSetCount;
        private readonly int _maxWordLength;
        private readonly int _maxWordsInStringCount;

        private BigInteger NextBigInteger
        {
            get
            {
                byte[] bytes = new byte[16];
                random.NextBytes(bytes);
                return new BigInteger(bytes);
            }
        }
        private string NextWord
        {
            get
            {
                return wordsSet[random.Next(wordsSet.Length)];
            }
        }

        public StringGenerator(StringGeneratorOptions options)
        {
            ValidateOptions(options);

            _wordsSetCount = options.WordsSetCount;
            _maxWordLength = options.MaxWordLength;
            _maxWordsInStringCount = options.MaxWordsInStringCount;

            wordsSet = GenerateWordsSet();
        }

        public string GenerateString()
        {
            int wordsCount = random.Next(1, _maxWordsInStringCount);
            var sb = new StringBuilder();
            sb.Append($"{NextBigInteger}: ");

            for (int i = 0; i < wordsCount; i++)
            {
                if (i > 0)
                {
                    sb.Append(' ');
                }
                sb.Append(NextWord);
            }

            return sb.ToString();
        }

        private string[] GenerateWordsSet()
        {
            string[] words = new string[_wordsSetCount];

            for (int i = 0; i < _wordsSetCount; i++)
            {
                int wordLength = random.Next(1, _maxWordLength + 1);
                var sb = new StringBuilder(wordLength);
                for (int j = 0; j < wordLength; j++)
                {
                    sb.Append(chars[random.Next(chars.Length)]);
                }
                words[i] = sb.ToString();
            }

            return words;
        }

        private static void ValidateOptions(StringGeneratorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.WordsSetCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.WordsSetCount), "WordsSetCount must be greater than 0.");
            if (options.MaxWordLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.MaxWordLength), "MaxWordLength must be greater than 0.");
            if (options.MaxWordsInStringCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.MaxWordsInStringCount), "MaxWordsInStringCount must be greater than 0.");
        }
    }
}
