﻿using AoC_2023.Utilities;
using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions
{
    internal class Day7 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var hands = new List<Hand>();
            foreach (var lineToken in input.Tokenize('\n'))
            {
                if (lineToken.Trim().Length == 0) continue;
                var line = lineToken.Trim();
                line.ReadTo(out var hand, ' ');
                var bid = int.Parse(line);
                var handView = new Hand(hand, bid);
                hands.Add(handView);
            }
            var rank = 1;
            var total = 0L;
            foreach (var hand in hands.OrderBy(x => x.Score))
                total += hand.Bid * rank++;

            return total.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            var hands = new List<Hand>();
            foreach (var lineToken in input.Tokenize('\n'))
            {
                if (lineToken.Trim().Length == 0) continue;
                var line = lineToken.Trim();
                line.ReadTo(out var hand, ' ');
                var bid = int.Parse(line);
                var handView = new Hand(hand, bid, true);
                hands.Add(handView);
            }
            var rank = 1;
            var total = 0L;
            foreach (var hand in hands.OrderBy(x => x.Score))
                total += hand.Bid * rank++;

            return total.ToString();
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

    internal record Hand
    {
        public int Bid { get; }
        public double Score { get; }
        public Hand(ReadOnlySpan<char> cards, int bid, bool treatAsJokers = false)
        {
            Bid = bid;

            Span<char> tempCards = stackalloc char[cards.Length];
            cards.CopyTo(tempCards);
            var maxOfOneKindExcludingPairs = 0;
            var pairCount = 0;
            var jCount = ReadOnlySpanExtensions.Count(cards, 'J');

            //Replace Js for now we will determine what to do with them afterwards
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

            var handType = !treatAsJokers ?
                GetHandType(Math.Max(maxOfOneKindExcludingPairs, jCount), pairCount + (jCount == 2 ? 1 : 0))
                : GetHandTypeWithJokers(maxOfOneKindExcludingPairs, pairCount, jCount);

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

        public static int GetCardValue(char c)
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


        public static HandType GetHandType(int maxOfOneKindExcludingPairs, int pairCount)
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

        public static HandType GetHandTypeWithJokers(int maxOfOneKindExcludingPairs, int pairCountExcludingJokers, int jokerCount)
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
    }
}
