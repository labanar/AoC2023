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
            input = delimiterPos == -1 ? [] : input[(delimiterPos + 1)..];
            return slice.Length > 0;
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
            input = delimiterPos == -1 ? [] : input[(delimiterPos + delimiter.Length)..];
            return slice.Length > 0;
        }

        internal static bool ReadFromEnd(this ref ReadOnlySpan<char> input, out ReadOnlySpan<char> slice, char delimiter)
        {
            if (input.Length == 0)
            {
                slice = ReadOnlySpan<char>.Empty;
                return false;
            }

            var delimiterPos = input.LastIndexOf(delimiter);
            slice = delimiterPos == -1 ? input : input[(delimiterPos + 1)..];
            input = delimiterPos == -1 ? [] : input[..delimiterPos];
            return slice.Length > 0;
        }

        internal static bool ReadFromEnd(this ref ReadOnlySpan<char> input, out ReadOnlySpan<char> slice, ReadOnlySpan<char> delimiter)
        {
            if (input.Length == 0)
            {
                slice = ReadOnlySpan<char>.Empty;
                return false;
            }

            var delimiterPos = input.LastIndexOf(delimiter);
            slice = delimiterPos == -1 ? input : input[(delimiterPos + delimiter.Length)..];
            input = delimiterPos == -1 ? [] : input[..delimiterPos];
            return slice.Length > 0;
        }
    }
}
