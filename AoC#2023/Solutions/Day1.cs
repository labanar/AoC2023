using AoC_2023.Utilities;

namespace AoC_2023.Solutions;

internal class Day1 : ISolution
{
    private static readonly DigitTrie ForwardTrie = DigitTrie.Forward;
    private static readonly DigitTrie BackwardTrie = DigitTrie.Backward;
    public static int Day => 1;

    public string Part1(ReadOnlySpan<char> input)
    {
        var total = 0;

        while (Helpers.ReadLine(ref input, out var line))
        {
            int? digit = null;
            foreach (var c in line)
            {
                if (!char.IsDigit(c)) continue;
                if (digit == null) total += 10 * Helpers.CharToDigit(c);
                digit = Helpers.CharToDigit(c);
            }


            total += digit.HasValue ? digit.Value : 0;
        }

        return total.ToString();
    }

    public string Part2(ReadOnlySpan<char> input)
    {
        var total = 0;
        while (Helpers.ReadLine(ref input, out var line))
        {
            TryGetNext(line, out var firstDigit, false);
            TryGetNext(line, out var lastDigit, true);
            total += (firstDigit * 10) + lastDigit;
        }

        return total.ToString();
    }

    public static bool TryGetNext(ReadOnlySpan<char> line, out int digit, bool reverse)
    {
        var current = reverse ? BackwardTrie.Root : ForwardTrie.Root;
        int? trieStartPosition = null;
        for (int i = 0; i < line.Length; i++)
        {
            var c = reverse ? line[line.Length - 1 - i] : line[i];
            if (current.Descendants.TryGetValue(c, out var descendant))
            {
                if (trieStartPosition == null)
                    trieStartPosition = i;

                current = descendant;
                if (current.TerminalDigit != null)
                {
                    digit = current.TerminalDigit.Value;
                    return true;
                }

                continue;
            }
            else if (char.IsDigit(c))
            {
                digit = Helpers.CharToDigit(c);
                return true;
            }

            if (trieStartPosition != null)
            {
                i = trieStartPosition.Value;
                trieStartPosition = null;
                current = reverse ? BackwardTrie.Root : ForwardTrie.Root;
                continue;
            }
        }

        digit = 0;
        return false;
    }


}

public record DigitTrie
{
    public DigitNode Root;

    private DigitTrie()
    {

        Root = new DigitNode { Value = '\0' };
    }

    public static DigitTrie Forward => new DigitTrie()
        .Add("one", 1)
        .Add("two", 2)
        .Add("three", 3)
        .Add("four", 4)
        .Add("five", 5)
        .Add("six", 6)
        .Add("seven", 7)
        .Add("eight", 8)
        .Add("nine", 9);

    public static DigitTrie Backward => new DigitTrie()
        .Add("one", 1, true)
        .Add("two", 2, true)
        .Add("three", 3, true)
        .Add("four", 4, true)
        .Add("five", 5, true)
        .Add("six", 6, true)
        .Add("seven", 7, true)
        .Add("eight", 8, true)
        .Add("nine", 9, true);

    public DigitTrie Add(ReadOnlySpan<char> word, int value, bool reverse = false)
    {
        var current = Root;
        for (int i = 0; i < word.Length; i++)
        {
            var c = word[i];
            if (reverse)
                c = word[word.Length - 1 - i];

            if (current.Descendants.TryGetValue(c, out var descendant))
            {
                current = descendant;
                continue;
            }

            descendant = new DigitNode { Value = c };
            current.Descendants.Add(c, descendant);
            current = descendant;
        }

        current.TerminalDigit = value;
        return this;
    }
}

public record DigitNode
{
    public required char Value { get; set; }
    public Dictionary<char, DigitNode> Descendants { get; set; } = [];
    public int? TerminalDigit { get; set; }
}
