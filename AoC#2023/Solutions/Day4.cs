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

        public string Part2(ReadOnlySpan<char> input)
        {
            var orderedMatches = new List<int>();
            while (Helpers.ReadLine(ref input, out var line))
            {
                var numMatches = 0;
                line = line.Slice(5);
                Helpers.ReadNext(ref line, out var cardNumberChars, ": ");
                var cardNumber = Helpers.IntFromChars(cardNumberChars.Trim());
                Helpers.ReadNext(ref line, out var winningNumbers, " | ");
                var myNumbers = line.Trim();
                winningNumbers = winningNumbers.Trim();
                while (Helpers.ReadNext(ref winningNumbers, out var winningNumberChars, " "))
                {
                    if (ScanForNumber(myNumbers, winningNumberChars))
                        numMatches++;

                    winningNumbers = winningNumbers.Trim();
                }
                orderedMatches.Add(numMatches);
            }

            var total = CalculateTotal(orderedMatches, 0, orderedMatches.Count);
            return total.ToString();
        }

        private static int CalculateTotal(List<int> orderedMatches, int startCard, int numToCalculate)
        {
            var total = 0;
            for (int i = startCard; i < numToCalculate; i++)
            {
                total += 1;
                var matches = orderedMatches[i];
                total += CalculateTotal(orderedMatches, i + 1, i + matches + 1);
            }
            return total;
        }

        private static void ProcessLines(ReadOnlySpan<char> input, int numLines, ref int total)
        {
            for (int i = 0; i < numLines; i++)
            {
                var numMatches = 0;
                if (Helpers.ReadLine(ref input, out var line))
                {
                    total += 1;
                    line = line.Slice(5);
                    Helpers.ReadNext(ref line, out var cardNumberChars, ": ");
                    var cardNumber = Helpers.IntFromChars(cardNumberChars.Trim());
                    Console.WriteLine("Evaluating Card " + cardNumber);
                    Helpers.ReadNext(ref line, out var winningNumbers, " | ");
                    var myNumbers = line.Trim();
                    winningNumbers = winningNumbers.Trim();
                    while (Helpers.ReadNext(ref winningNumbers, out var winningNumberChars, " "))
                    {
                        if (ScanForNumber(myNumbers, winningNumberChars))
                            numMatches++;

                        winningNumbers = winningNumbers.Trim();
                    }

                    Console.WriteLine($"Won {numMatches} more cards");
                    ProcessLines(input.Trim(), numMatches, ref total);
                }
                else
                    break;
            }
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
    }
}
