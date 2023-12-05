using AoC_2023.Utilities;
using CommunityToolkit.HighPerformance;
using System.Buffers;
using System.IO.Pipelines;

namespace AoC_2023.Solutions
{
    internal class Day4 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var total = 0;
            foreach (var token in input.Tokenize('\n'))
            {
                if (token.Length == 0) continue;
                var numMatches = GetNumberOfMatches(token);
                total += numMatches == 0 ? 0 : (int)Math.Pow(2, numMatches - 1);
            }
            return total.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            var orderedMatches = new List<int>();
            foreach (var token in input.Tokenize('\n'))
            {
                if (token.Length == 0) continue;
                var numMatches = GetNumberOfMatches(token);
                orderedMatches.Add(numMatches);
            }
            var total = CalculateTotal(orderedMatches, 0, orderedMatches.Count);
            return total.ToString();
        }

        private int GetNumberOfMatches(ReadOnlySpan<char> line)
        {
            var matches = 0;
            line.ReadTo(out _, ": ");
            line.ReadTo(out var winningNumbers, " | ");
            foreach (var token in winningNumbers.Tokenize(' '))
            {
                if (token.Length == 0) continue;
                if (ScanForNumber(line, token))
                    matches++;
            }
            return matches;
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
            foreach (var token in scanSource.Tokenize(' '))
            {
                if (token.Length == 0) continue;
                if (token.SequenceEqual(valueToFind)) return true;
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
                while (TryProcessNextLine(ref buffer, out var numMatches))
                    yield return numMatches;

                pipe.AdvanceTo(buffer.Start, buffer.End);
                if (result.IsCompleted)
                    break;
            }
        }

        private bool TryProcessNextLine(ref ReadOnlySequence<byte> buffer, out int matches)
        {
            matches = 0;
            var reader = new SequenceReader<byte>(buffer);
            if (!reader.TryReadTo(out ReadOnlySpan<byte> _, ": "u8, true)) return false;
            if (!reader.TryReadTo(out ReadOnlySpan<byte> winnningNumbers, " | "u8, true)) return false;
            if (!reader.TryReadTo(out ReadOnlySpan<byte> myNumbers, (byte)'\n', true)) return false;
            foreach (var token in winnningNumbers.Tokenize((byte)' '))
            {
                if (token.Length == 0) continue;
                if (ScanForNumber(myNumbers, token))
                    matches++;
            }
            buffer = buffer.Slice(reader.Position);
            return true;
        }

        private static bool ScanForNumber(ReadOnlySpan<byte> scanSource, ReadOnlySpan<byte> valueToFind)
        {
            foreach (var token in scanSource.Tokenize((byte)' '))
            {
                if (token.Length == 0) continue;
                if (token.SequenceEqual(valueToFind)) return true;
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
