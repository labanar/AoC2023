using AoC_2023.Utilities;

namespace AoC_2023.Solutions
{
    internal class Day4 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var total = 0;
            while (Helpers.ReadLine(ref input, out var line))
            {
                line = line.Slice(5);
                Helpers.ReadNext(ref line, out var cardNumberChars, ": ");
                var cardNumber = Helpers.IntFromChars(cardNumberChars.Trim());
                Helpers.ReadNext(ref line, out var winningNumbers, " | ");
                var myNumbers = line.Trim();
                winningNumbers = winningNumbers.Trim();
                int? pointsForCard = null;
                while (Helpers.ReadNext(ref winningNumbers, out var winningNumberChars, " "))
                {
                    if (ScanForNumber(myNumbers, winningNumberChars))
                    {
                        if (pointsForCard == null) pointsForCard = 1;
                        else pointsForCard *= 2;
                    }
                    winningNumbers = winningNumbers.Trim();
                }
                if (pointsForCard != null)
                    total += pointsForCard.Value;
            }
            return total.ToString();
        }


        private static bool ScanForNumber(ReadOnlySpan<char> scanSource, ReadOnlySpan<char> valueToFind)
        {
            while (Helpers.ReadNext(ref scanSource, out var myNumberChars, " "))
            {
                if (myNumberChars.SequenceEqual(valueToFind)) return true;
                scanSource = scanSource.Trim();
            }
            return false;
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            return string.Empty;
        }
    }
}
