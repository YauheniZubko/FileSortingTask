namespace FileSorting.FileSorting
{
    public class LinesContentComparer : IComparer<LineContent>
    {
        public int Compare(LineContent a, LineContent b)
        {
            int textCompare = string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase);
            return textCompare != 0 ? textCompare : a.Number.CompareTo(b.Number);
        }
    }
}
