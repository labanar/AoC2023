
using AoC_2023.Utilities;
using Microsoft.Extensions.Logging;

namespace AoC_2023.Solutions
{
    internal class Day2(ILogger<Day2> logger)
        : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var total = 0;
            while (Helpers.ReadLine(ref input, out var line))
            {
                line = line.Slice(5);
                var gameDelimiter = line.IndexOf(':');
                var gameIdChars = line.Slice(0, gameDelimiter);
                var gameId = Helpers.IntFromChars(gameIdChars);

                line = line.Slice(gameDelimiter + 2);
                var disqualified = false;
                while (TryReadRound(ref line, out var blue, out var red, out var green) && !disqualified)
                {
                    if (red > 12) disqualified = true;
                    if (green > 13) disqualified = true;
                    if (blue > 14) disqualified = true;
                    logger.LogInformation("Round processed for {Game}", gameId);
                }

                if (!disqualified) total += gameId;
            }

            return total.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            var total = 0;
            while (Helpers.ReadLine(ref input, out var line))
            {
                line = line.Slice(5);
                var gameDelimiter = line.IndexOf(':');
                var gameIdChars = line.Slice(0, gameDelimiter);
                var gameId = Helpers.IntFromChars(gameIdChars);

                line = line.Slice(gameDelimiter + 2);
                int? maxRed = null;
                int? maxBlue = null;
                int? maxGreen = null;
                while (TryReadRound(ref line, out var blue, out var red, out var green))
                {
                    if (!maxBlue.HasValue && blue.HasValue) maxBlue = blue;
                    else if (maxBlue.HasValue && blue.HasValue && blue.Value > maxBlue.Value) maxBlue = blue;

                    if (!maxRed.HasValue && red.HasValue) maxRed = red;
                    else if (maxRed.HasValue && red.HasValue && red.Value > maxRed.Value) maxRed = red;

                    if (!maxGreen.HasValue && green.HasValue) maxGreen = green;
                    else if (maxGreen.HasValue && green.HasValue && green.Value > maxGreen.Value) maxGreen = green;
                }

                var power = 1;
                if (maxRed.HasValue) power *= maxRed.Value;
                if (maxGreen.HasValue) power *= maxGreen.Value;
                if (maxBlue.HasValue) power *= maxBlue.Value;
                total += power;
            }

            return total.ToString();
        }


        private static bool TryReadRound(ref ReadOnlySpan<char> line, out int? blue, out int? red, out int? green)
        {
            blue = null;
            red = null;
            green = null;

            ReadOnlySpan<char> round = ReadOnlySpan<char>.Empty;
            var roundDelimiterPos = line.IndexOf(";");
            if (roundDelimiterPos == -1)
            {
                round = line;
                line = ReadOnlySpan<char>.Empty;
            }
            else
            {
                round = line.Slice(0, roundDelimiterPos);
                line = line.Slice(roundDelimiterPos + 1);
            }


            var hasValue = false;
            while (TryReadNextValue(ref round, out var value, out var color))
            {
                hasValue = true;
                switch (color)
                {
                    case Color.R: red = value; break;
                    case Color.G: green = value; break;
                    case Color.B: blue = value; break;
                }
            }

            return hasValue;
        }

        private static bool TryReadNextValue(ref ReadOnlySpan<char> round, out int? value, out Color? color)
        {
            value = null;
            color = null;
            if (round.Length == 0)
                return false;

            round = round.Trim();
            var valueDelimiterPos = round.IndexOf(",");
            ReadOnlySpan<char> rawValue = ReadOnlySpan<char>.Empty;
            if (valueDelimiterPos == -1)
            {
                rawValue = round.Trim();
                round = ReadOnlySpan<char>.Empty;
            }
            else
            {
                rawValue = round.Slice(0, valueDelimiterPos);
                round = round.Slice(valueDelimiterPos + 1);
            }

            var colorDelimiterPos = rawValue.IndexOf(' ');
            var numberChars = rawValue.Slice(0, colorDelimiterPos);
            var colorChars = rawValue.Slice(colorDelimiterPos + 1);

            value = Helpers.IntFromChars(numberChars);
            color = colorChars switch
            {
                "green" => Color.G,
                "red" => Color.R,
                "blue" => Color.B,
                _ => throw new ArgumentException("Unexpected color")
            };
            return true;
        }
    }

    internal enum Color
    {
        R,
        G,
        B
    }
}
