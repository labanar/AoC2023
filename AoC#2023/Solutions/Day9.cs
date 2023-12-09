using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions;

internal class Day9 : ISolution
{
    public string Part1(ReadOnlySpan<char> input)
    {
        var total = 0;
        foreach (var lineToken in input.Tokenize('\n'))
        {
            var line = lineToken;
            Stack<int[]> difFHistories = GetDiffStack(line);
            var prediction = 0;
            do
            {
                var currentDiffs = difFHistories.Pop();
                prediction = currentDiffs.Last() + prediction;
            }
            while (difFHistories.Count > 0);
            total += prediction;
        }
        return total.ToString();
    }

    public string Part2(ReadOnlySpan<char> input)
    {
        var total = 0;
        foreach (var lineToken in input.Tokenize('\n'))
        {
            var line = lineToken;
            Stack<int[]> difFHistories = GetDiffStack(line);
            var prediction = 0;
            do
            {
                var currentDiffs = difFHistories.Pop();
                prediction = currentDiffs.First() - prediction;
            }
            while (difFHistories.Count > 0);
            total += prediction;
        }
        return total.ToString();
    }
    private static Stack<int[]> GetDiffStack(ReadOnlySpan<char> line)
    {
        Stack<int[]> historyStack = new Stack<int[]>();
        var history = ReadHistory(line);
        historyStack.Push(history);
        var diffs = GetDiffs(history);
        while (!diffs.All(x => x == 0))
        {
            historyStack.Push(diffs);
            diffs = GetDiffs(diffs);
        }
        return historyStack;
    }

    private static int[] ReadHistory(ReadOnlySpan<char> line)
    {
        var size = ReadOnlySpanExtensions.Count(line, ' ') + 1;
        var values = new int[size];
        var i = 0;
        foreach (var numberToken in line.Tokenize(' '))
        {
            if (numberToken.Length == 0) continue;
            values[i++] = int.Parse(numberToken);
        }
        return values;
    }

    private static int[] GetDiffs(int[] values)
    {
        var diffs = new int[values.Length - 1];
        var previous = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            diffs[i - 1] = values[i] - previous;
            previous = values[i];
        }
        return diffs;
    }


}
