using AoC_2023.Utilities;
using Microsoft.Extensions.Logging;

namespace AoC_2023.Solutions
{
    internal class Day3(ILogger<Day3> logger)
        : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var hasMore = true;
            hasMore &= Helpers.ReadLine(ref input, out var current);
            hasMore &= Helpers.ReadLine(ref input, out var next);
            var previous = current.Slice(0, 0);

            var total = 0;
            while (current.Length > 0)
            {
                var offset = 0;
                var currentUntouched = current;
                while (Helpers.ReadNextInt(ref current, out var numeric, out var leading))
                {
                    offset += leading.Length;
                    var isAdjacentToSymbol = false;
                    if (leading.Length > 0 && IsSymbol(leading[leading.Length - 1]))
                        isAdjacentToSymbol |= true;
                    if (current.Length > 0 && IsSymbol(current[0]))
                        isAdjacentToSymbol |= true;

                    if (!isAdjacentToSymbol)
                    {
                        if (next.Length > 0)
                        {
                            var peekMin = Math.Max(offset - 1, 0);
                            var peekMax = Math.Min(offset + numeric.Length + 1, next.Length - 1);
                            var nextWindow = next.Slice(peekMin, peekMax - peekMin);
                            if (SequenceContainsSymbols(nextWindow))
                                isAdjacentToSymbol |= true;
                        }

                        if (previous.Length > 0)
                        {
                            var peekMin = Math.Max(offset - 1, 0);
                            var peekMax = Math.Min(offset + numeric.Length + 1, previous.Length - 1);
                            var previousWindow = previous.Slice(peekMin, peekMax - peekMin);
                            if (SequenceContainsSymbols(previousWindow))
                                isAdjacentToSymbol |= true;
                        }
                    };

                    offset += numeric.Length;
                    if (isAdjacentToSymbol)
                        total += Helpers.IntFromChars(numeric);
                }

                previous = currentUntouched;
                current = next;
                hasMore = Helpers.ReadLine(ref input, out next);
            }
            return total.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            return "X";
        }

        private static bool SequenceContainsSymbols(ReadOnlySpan<char> sequence)
        {
            foreach (var c in sequence)
            {
                if (IsSymbol(c)) return true;
            }
            return false;
        }

        private static bool IsSymbol(char c)
        {
            if (!char.IsDigit(c) && c != '.')
                return true;

            return false;
        }
    }
}
