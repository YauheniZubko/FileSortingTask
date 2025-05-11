using System.Numerics;
namespace FileSorting.FileSorting
{
    public interface IParser<T>
    {
        T Parse(string input);
    }
    public class LineContentParser : IParser<LineContent>
    {
        public LineContent Parse(string input)
        {
            var parts = input.Split(new[] { ": " }, 2, StringSplitOptions.None);

            if (parts.Length != 2 || !BigInteger.TryParse(parts[0], out var number))
            {
                throw new FormatException($"Invalid line format: '{input}'");
            }

            return new LineContent
            {
                Number = number,
                Text = parts[1]
            };
        }
    }
}
