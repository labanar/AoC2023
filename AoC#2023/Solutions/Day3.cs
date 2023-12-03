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
                    isAdjacentToSymbol |= leading.Length > 0 && IsSymbol(leading[leading.Length - 1]);  //Check to our right
                    isAdjacentToSymbol |= current.Length > 0 && IsSymbol(current[0]);   //Check to our left
                    isAdjacentToSymbol |= ScanForSymbol(next, offset, numeric); //Check below
                    isAdjacentToSymbol |= ScanForSymbol(previous, offset, numeric); //Check above
                    offset += numeric.Length;
                    if (isAdjacentToSymbol) total += Helpers.IntFromChars(numeric);
                }

                previous = currentUntouched;
                current = next;
                Helpers.ReadLine(ref input, out next);
            }
            return total.ToString();
        }


        public string Part2(ReadOnlySpan<char> input)
        {
            long total = 0;
            Helpers.ReadLine(ref input, out var current);
            Helpers.ReadLine(ref input, out var next);
            var previous = current[..0];
            while (current.Length > 0)
            {
                var offset = 0;
                var currentUntouched = current;
                while (Helpers.FindNext(ref current, '*', out var leading))
                {
                    offset += leading.Length;
                    var adjacentNumberCount = 0;
                    long gearRatio = 1;
                    ScanForGearRatios(offset, currentUntouched, ref adjacentNumberCount, ref gearRatio);
                    ScanForGearRatios(offset, previous, ref adjacentNumberCount, ref gearRatio);
                    ScanForGearRatios(offset, next, ref adjacentNumberCount, ref gearRatio);
                    offset += 1;
                    if (adjacentNumberCount == 2)
                        total += gearRatio;
                }

                previous = currentUntouched;
                current = next;
                Helpers.ReadLine(ref input, out next);
            }
            return total.ToString();
        }

        private static bool ScanForSymbol(ReadOnlySpan<char> input, int offset, ReadOnlySpan<char> numeric)
        {
            if (input.Length == 0) return false;
            var peekMin = Math.Max(offset - 1, 0);
            var peekMax = Math.Min(offset + numeric.Length + 1, input.Length - 1);
            var scanWindow = input.Slice(peekMin, peekMax - peekMin);
            return SequenceContainsSymbols(scanWindow);
        }

        private static void ScanForGearRatios(int offset, ReadOnlySpan<char> scanSource, ref int adjacentNumberCount, ref long gearRatio)
        {
            var scanOffset = 0;
            while (scanSource.Length > 0 & Helpers.ReadNextInt(ref scanSource, out var numericSpan, out var pLeading))
            {
                scanOffset += pLeading.Length;
                var minBound = scanOffset - 1;
                var maxBound = scanOffset + numericSpan.Length + 1;
                if (offset >= minBound && offset < maxBound)
                {
                    gearRatio *= Helpers.IntFromChars(numericSpan);
                    adjacentNumberCount++;
                }
                scanOffset += numericSpan.Length;
            }
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
