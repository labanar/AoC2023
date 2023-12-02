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

        internal static int CharToDigit(char c)
        {
            return c switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                _ => throw new ArgumentException()
            };
        }


        internal static int IntFromChars(ReadOnlySpan<char> input)
        {
            var value = 0;
            for (var i = 0; i < input.Length; i++)
            {
                var pos = input.Length - 1 - i;
                value += Helpers.CharToDigit(input[pos]) * (int)Math.Pow(10, i);
            }
            return value;
        }
    }

}
