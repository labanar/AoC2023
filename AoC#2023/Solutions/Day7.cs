using AoC_2023.Utilities;
using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions;

internal class Day7 : ISolution
{
    public string Part1(ReadOnlySpan<char> input) =>
         Solve(input, false).ToString();

    public string Part2(ReadOnlySpan<char> input) =>
        Solve(input, true).ToString();

    private long Solve(ReadOnlySpan<char> input, bool withJokers)
    {
        var size = ReadOnlySpanExtensions.Count(input, '\n') + 1;
        Span<Hand> hands = stackalloc Hand[size];
        var i = 0;
        foreach (var lineToken in input.Tokenize('\n'))
        {
            var line = lineToken.Trim();
            if (line.Length == 0) continue;
            line.ReadTo(out var hand, ' ');
            var bid = int.Parse(line);
            hands[i++] = new Hand(hand, bid, withJokers);
        }
        var rank = 1;
        var total = 0L;
        hands.Sort();
        foreach (var hand in hands)
            total += hand.Bid * rank++;

        return total;
    }
}

internal enum HandType
{
    HighCard = 1,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind
}

internal readonly struct Hand : IComparable<Hand>
{
    public readonly int Bid;
    public readonly double Score;
    public Hand(ReadOnlySpan<char> cards, int bid, bool treatAsJokers = false)
    {
        Bid = bid;

        var (maxOfOneKind, numPairsExcludingJokers, numJokers) = GetCounts(cards);
        var handType =
            !treatAsJokers
                ? GetHandType(Math.Max(maxOfOneKind, numJokers), numPairsExcludingJokers + (numJokers == 2 ? 1 : 0))
                : GetHandTypeWithJokers(maxOfOneKind, numPairsExcludingJokers, numJokers);

        //Assign a score based on hand type + cards in hand
        //Score = HandType.{C1}{C2}{C3}{C4}{C5}
        var score = (double)handType;
        var scalingFactor = 100.0;
        foreach (var c in cards)
        {
            var value = GetCardValue(c);
            if (treatAsJokers && value == 11) value = 1;
            var scaledValue = value / scalingFactor;
            scalingFactor *= 100;
            score += scaledValue;
        }

        Score = score;
    }

    private static (int maxOfOneKind, int numPairsExcludingJokers, int numOfJokers) GetCounts(ReadOnlySpan<char> cards)
    {
        Span<char> tempCards = stackalloc char[cards.Length];
        cards.CopyTo(tempCards);
        var maxOfOneKindExcludingPairs = 0;
        var pairCount = 0;
        var jokerCount = ReadOnlySpanExtensions.Count(cards, 'J');

        tempCards.Replace('J', '*');
        while (!tempCards.SequenceEqual("*****"))
        {
            var cIndex = tempCards.IndexOfAnyExcept('*');
            var c = tempCards[cIndex];
            var occurrences = ReadOnlySpanExtensions.Count(cards, c);
            tempCards.Replace(c, '*');
            if (occurrences == 2)
                pairCount++;
            else if (occurrences > maxOfOneKindExcludingPairs)
                maxOfOneKindExcludingPairs = occurrences;
        }
        return (maxOfOneKindExcludingPairs, pairCount, jokerCount);
    }

    private static int GetCardValue(char c)
    {
        return c switch
        {
            'T' => 10,
            'J' => 11,
            'Q' => 12,
            'K' => 13,
            'A' => 14,
            _ => Helpers.CharToDigit(c)
        };
    }

    private static HandType GetHandType(int maxOfOneKindExcludingPairs, int pairCount)
    {
        if (maxOfOneKindExcludingPairs == 5)
            return HandType.FiveOfAKind;
        else if (maxOfOneKindExcludingPairs == 4)
            return HandType.FourOfAKind;
        else if (maxOfOneKindExcludingPairs == 3 && pairCount == 1)
            return HandType.FullHouse;
        else if (maxOfOneKindExcludingPairs == 3)
            return HandType.ThreeOfAKind;
        else if (pairCount == 2)
            return HandType.TwoPair;
        else if (pairCount == 1)
            return HandType.OnePair;
        else
            return HandType.HighCard;
    }

    private static HandType GetHandTypeWithJokers(int maxOfOneKindExcludingPairs, int pairCountExcludingJokers, int jokerCount)
    {
        if (maxOfOneKindExcludingPairs + jokerCount == 5 || (pairCountExcludingJokers == 1 && jokerCount == 3))
            return HandType.FiveOfAKind;
        else if (maxOfOneKindExcludingPairs + jokerCount == 4 || (pairCountExcludingJokers == 1 && jokerCount == 2))
            return HandType.FourOfAKind;
        else if ((maxOfOneKindExcludingPairs + jokerCount == 3 && pairCountExcludingJokers == 1) || (pairCountExcludingJokers == 2 && jokerCount == 1))
            return HandType.FullHouse;
        else if (maxOfOneKindExcludingPairs + jokerCount == 3 || (pairCountExcludingJokers == 1 && jokerCount == 1))
            return HandType.ThreeOfAKind;
        else if (pairCountExcludingJokers == 2)
            return HandType.TwoPair;
        else if (pairCountExcludingJokers == 1 || jokerCount == 1)
            return HandType.OnePair;
        else
            return HandType.HighCard;
    }

    public int CompareTo(Hand other) => Score.CompareTo(other.Score);
}
