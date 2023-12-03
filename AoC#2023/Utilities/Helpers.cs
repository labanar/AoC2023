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
            return ReadNext(ref input, out currentLine, '\n');
        }

        internal static bool ReadNext(ref ReadOnlySpan<char> input, out ReadOnlySpan<char> slice, char delimiter)
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

        internal static bool ReadNext(ref ReadOnlySpan<char> input, out ReadOnlySpan<char> slice, ReadOnlySpan<char> delimiter)
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

        internal static bool ReadNextInt(ref ReadOnlySpan<char> input, out ReadOnlySpan<char> valueSpan, out ReadOnlySpan<char> leadingChars)
        {
            if (input.Length == 0)
            {
                leadingChars = ReadOnlySpan<char>.Empty;
                valueSpan = ReadOnlySpan<char>.Empty;
                return false;
            }

            int? startNumber = null;
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (!char.IsDigit(c))
                {
                    if (startNumber == null) continue;
                    leadingChars = input.Slice(0, startNumber.Value);
                    valueSpan = input.Slice(startNumber.Value, i - startNumber.Value);
                    input = input.Slice(startNumber.Value + valueSpan.Length);
                    return true;
                }
                if (startNumber == null)
                {
                    startNumber = i;
                    continue;
                }
            }

            if (startNumber == null)
            {
                leadingChars = ReadOnlySpan<char>.Empty;
                valueSpan = ReadOnlySpan<char>.Empty;
                return false;
            }

            leadingChars = input.Slice(0, startNumber.Value);
            valueSpan = input.Slice(startNumber.Value);
            input = ReadOnlySpan<char>.Empty;
            return true;
        }

        internal static bool FindNext(ref ReadOnlySpan<char> input, char toFind, out ReadOnlySpan<char> leadingChars)
        {
            if (input.Length == 0)
            {
                leadingChars = ReadOnlySpan<char>.Empty;
                return false;
            }

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (c == toFind)
                {
                    leadingChars = input.Slice(0, i);
                    input = input.Slice(i + 1);
                    return true;
                }
            }

            leadingChars = ReadOnlySpan<char>.Empty;
            input = ReadOnlySpan<char>.Empty;
            return false;
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
                value += CharToDigit(input[pos]) * (int)Math.Pow(10, i);
            }
            return value;
        }
    }

}
