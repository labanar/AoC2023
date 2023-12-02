namespace AoC_2023.Utilities
{
    internal static class Helpers
    {
        internal static bool ReadLine(ref ReadOnlySpan<char> input, out ReadOnlySpan<char> currentLine)
        {
            if (input == null || input.Length == 0)
            {
                currentLine = ReadOnlySpan<char>.Empty;
                return false;
            }

            var delimiterPos = input.IndexOf('\n');
            if (delimiterPos != -1)
            {
                currentLine = input.Slice(0, delimiterPos);
                input = input.Slice(delimiterPos);
                if (input.Length > 0)
                {
                    input = input.Slice(1);
                }
                return true;
            }

            currentLine = input.Slice(0, input.Length);
            input = ReadOnlySpan<char>.Empty;
            return true;
        }
    }
}
