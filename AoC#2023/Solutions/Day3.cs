using AoC_2023.Utilities;
using Microsoft.Extensions.Logging;

namespace AoC_2023.Solutions
{
    internal class Day3(ILogger<Day3> logger)
        : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            Helpers.ReadLine(ref input, out var current);
            Helpers.ReadLine(ref input, out var next);
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
                _ = Helpers.ReadLine(ref input, out next);
            }
            return total.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            Helpers.ReadLine(ref input, out var current);
            Helpers.ReadLine(ref input, out var next);
            var previous = current[..0];

            long total = 0;
            while (current.Length > 0)
            {
                var offset = 0;
                var currentUntouched = current;
                var previousUntouched = previous;
                var nextUntouched = next;
                while (Helpers.FindNext(ref current, '*', out var leading))
                {
                    var adjacentNumberCount = 0;
                    long gearRatio = 1;

                    offset += leading.Length;
                    if (leading.Length > 0 && char.IsDigit(leading[leading.Length - 1]))
                    {
                        var window = currentUntouched.Slice(0, offset);
                        var numeric = 0;
                        while (Helpers.ReadNextInt(ref window, out var num, out _))
                            numeric = Helpers.IntFromChars(num);

                        gearRatio *= numeric;
                        adjacentNumberCount++;
                    }
                    if (current.Length > 0 && char.IsDigit(current[0]))
                    {
                        var window = currentUntouched.Slice(Math.Min(offset + 1, currentUntouched.Length - 1));
                        var numeric = 0;
                        if (Helpers.ReadNextInt(ref window, out var num, out _))
                            numeric = Helpers.IntFromChars(num);

                        gearRatio *= numeric;
                        adjacentNumberCount++;
                    }

                    //Read through the numbers on the previous line
                    var pOffset = 0;
                    var prevWindow = previousUntouched;
                    while (previous.Length > 0 & Helpers.ReadNextInt(ref prevWindow, out var numericSpan, out var pLeading))
                    {
                        pOffset += pLeading.Length;
                        var minBound = pOffset - 1;
                        var maxBound = pOffset + numericSpan.Length + 1;
                        if (offset >= minBound && offset < maxBound)
                        {
                            gearRatio *= Helpers.IntFromChars(numericSpan);
                            adjacentNumberCount++;
                        }
                        pOffset += numericSpan.Length;
                    }

                    var nOffset = 0;
                    var nextWindow = nextUntouched;
                    while (next.Length > 0 & Helpers.ReadNextInt(ref nextWindow, out var numericSpan, out var nLeading))
                    {
                        nOffset += nLeading.Length;
                        var minBound = nOffset - 1;
                        var maxBound = nOffset + numericSpan.Length + 1;
                        if (offset >= minBound && offset < maxBound)
                        {
                            gearRatio *= Helpers.IntFromChars(numericSpan);
                            adjacentNumberCount++;
                        }
                        nOffset += numericSpan.Length;
                    }

                    offset += 1;
                    if (adjacentNumberCount == 2)
                        total += gearRatio;
                }


                previous = currentUntouched;
                current = nextUntouched;
                Helpers.ReadLine(ref input, out next);
            }
            return total.ToString();
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
