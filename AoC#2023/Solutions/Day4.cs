using AoC_2023.Utilities;
using System.Buffers;
using System.IO.Pipelines;

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
                Helpers.ReadNext(ref line, out _, ": ");
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
                Helpers.ReadNext(ref line, out _, ": ");
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

    internal sealed class Day4Pipelines : IPiplinesSolution
    {
        public async Task<string> Part1(PipeReader pipe)
        {
            var total = 0;

            await foreach (var matches in ReadMatches(pipe))
                total += matches == 0 ? 0 : (int)Math.Pow(2, matches - 1);

            return total.ToString();
        }

        public async Task<string> Part2(PipeReader pipe)
        {
            var matchHistory = new List<int>();
            await foreach (var matches in ReadMatches(pipe))
                matchHistory.Add(matches);

            var total = CalculateTotal(matchHistory, 0, matchHistory.Count);
            return total.ToString();
        }

        private async IAsyncEnumerable<int> ReadMatches(PipeReader pipe)
        {
            while (true)
            {
                var result = await pipe.ReadAsync();
                var buffer = result.Buffer;
                while (Helpers.TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                    yield return ProcessLine(line);

                pipe.AdvanceTo(buffer.Start, buffer.End);
                if (result.IsCompleted)
                    break;
            }
        }

        private int ProcessLine(ReadOnlySequence<byte> line)
        {
            var reader = new SequenceReader<byte>(line);
            reader.TryReadTo(out ReadOnlySpan<byte> _, ": "u8, true);
            reader.TryReadTo(out ReadOnlySpan<byte> winnningNumbers, " | "u8, true);
            Span<byte> myNumbers = stackalloc byte[(int)reader.UnreadSequence.Length];
            reader.UnreadSequence.CopyTo(myNumbers);
            winnningNumbers = winnningNumbers.Trim((byte)' ');
            myNumbers.TrimSpaces();
            var matches = 0;
            while (Helpers.ReadNext(ref winnningNumbers, out var winningNumberBytes, " "u8))
            {
                winningNumberBytes.TrimSpaces();
                if (ScanForNumber(myNumbers, winningNumberBytes))
                    matches++;

                winnningNumbers.TrimSpaces();
            }
            return matches;
        }

        private static bool ScanForNumber(ReadOnlySpan<byte> scanSource, ReadOnlySpan<byte> valueToFind)
        {
            while (Helpers.ReadNext(ref scanSource, out var myNumberChars, " "u8))
            {
                if (myNumberChars.SequenceEqual(valueToFind)) return true;
                scanSource.TrimSpaces();
            }
            return false;
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
    }
}
