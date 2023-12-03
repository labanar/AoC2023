
using AoC_2023.Utilities;

namespace AoC_2023.Solutions;

internal class Day2 : ISolution
{
    public string Part1(ReadOnlySpan<char> input)
    {
        var total = 0;
        while (Helpers.ReadLine(ref input, out var line))
        {
            Helpers.ReadNext(ref line, out _, " ");
            Helpers.ReadNext(ref line, out var gameIdChars, ":");
            var gameId = Helpers.IntFromChars(gameIdChars);
            var disqualified = false;
            while (TryReadRound(ref line, out var blue, out var red, out var green) && !disqualified)
            {
                if (red > 12) disqualified = true;
                if (green > 13) disqualified = true;
                if (blue > 14) disqualified = true;
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
            Helpers.ReadNext(ref line, out _, ":");
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
        if (line.Length == 0) return false;
        if (!Helpers.ReadNext(ref line, out var round, ";")) return false;
        while (Helpers.ReadNext(ref round, out var valueSpan, ","))
        {
            valueSpan = valueSpan.Trim();
            Helpers.ReadNext(ref valueSpan, out var numberChars, " ");
            var colorChars = valueSpan;
            var value = Helpers.IntFromChars(numberChars);
            switch (colorChars)
            {
                case "green":
                    green = value;
                    break;
                case "blue":
                    blue = value;
                    break;
                case "red":
                    red = value;
                    break;
            }
        }
        return true;
    }
}
