namespace AoC_2023.Utilities
{
    internal static class SpanExtensions
    {
        internal static bool ReadTo(this ref ReadOnlySpan<char> input, out ReadOnlySpan<char> slice, char delimiter)
        {
            if (input.Length == 0)
            {
                slice = ReadOnlySpan<char>.Empty;
                return false;
            }
            var delimiterPos = input.IndexOf(delimiter);
            slice = delimiterPos == -1 ? input : input[..delimiterPos];
            input = delimiterPos == -1 ? ReadOnlySpan<char>.Empty : input[(delimiterPos + 1)..];
            return true;
        }
        internal static bool ReadTo(this ref ReadOnlySpan<char> input, out ReadOnlySpan<char> slice, ReadOnlySpan<char> delimiter)
        {
            if (input.Length == 0)
            {
                slice = ReadOnlySpan<char>.Empty;
                return false;
            }
            var delimiterPos = input.IndexOf(delimiter);
            slice = delimiterPos == -1 ? input : input[..delimiterPos];
            input = delimiterPos == -1 ? ReadOnlySpan<char>.Empty : input[(delimiterPos + delimiter.Length)..];
            return slice.Length > 0;
        }
    }
}
