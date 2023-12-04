
using AoC_2023.Utilities;
using System.Buffers;
using System.IO.Pipelines;

namespace AoC_2023.Solutions;
internal sealed class Day2 : ISolution
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
        if (!Helpers.ReadNext(ref line, out var round, "; ")) return false;
        while (Helpers.ReadNext(ref round, out var valueSpan, ", "))
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

internal sealed class Day2Pipelines : IPiplinesSolution
{
    public async Task<string> Part1(PipeReader pipe)
    {
        (var p1, _) = await Solve(pipe);
        return p1.ToString();
    }

    public async Task<string> Part2(PipeReader pipe)
    {
        (_, var p2) = await Solve(pipe);
        return p2.ToString();
    }

    private async ValueTask<(int, int)> Solve(PipeReader pipe)
    {
        int totalP1 = 0, totalP2 = 0;
        while (true)
        {
            var result = await pipe.ReadAsync();
            var buffer = result.Buffer;
            while (Helpers.TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                ProcessLine(line, ref totalP1, ref totalP2);

            pipe.AdvanceTo(buffer.Start, buffer.End);
            if (result.IsCompleted)
                break;
        }
        return (totalP1, totalP2);
    }

    private void ProcessLine(ReadOnlySequence<byte> line, ref int totalP1, ref int totalP2)
    {
        if (line.Length == 0)
            return;

        var reader = new SequenceReader<byte>(line);
        reader.TryReadTo(out ReadOnlySpan<byte> _, " "u8, true);
        reader.TryReadTo(out ReadOnlySpan<byte> gameIdBytes, ": "u8, true);
        var gameId = Helpers.IntFromCharUtf8Bytes(gameIdBytes);
        int? maxRed = null, maxGreen = null, maxBlue = null;
        while (reader.TryReadTo(out ReadOnlySpan<byte> roundBytes, "; "u8, true))
            ReadRoundMaxes(roundBytes, ref maxRed, ref maxGreen, ref maxBlue);

        Span<byte> finalRound = stackalloc byte[(int)reader.UnreadSequence.Length];
        reader.UnreadSequence.CopyTo(finalRound);
        ReadRoundMaxes(finalRound, ref maxRed, ref maxGreen, ref maxBlue);
        if (maxRed <= 12 && maxGreen <= 13 && maxBlue <= 14)
            totalP1 += gameId;

        var power = 1;
        if (maxRed.HasValue) power *= maxRed.Value;
        if (maxGreen.HasValue) power *= maxGreen.Value;
        if (maxBlue.HasValue) power *= maxBlue.Value;
        totalP2 += power;
    }

    private static void ReadRoundMaxes(ReadOnlySpan<byte> round, ref int? maxRed, ref int? maxGreen, ref int? maxBlue)
    {
        ReadRound(round, out var red, out var green, out var blue);
        maxRed = SetIfMax(maxRed, red);
        maxGreen = SetIfMax(maxGreen, green);
        maxBlue = SetIfMax(maxBlue, blue);
    }

    private static int? SetIfMax(int? max, int? current)
    {
        if (!max.HasValue && current.HasValue) max = current;
        else if (max.HasValue && current.HasValue && current.Value > max.Value) max = current;
        return max;
    }

    private static void ReadRound(ReadOnlySpan<byte> round, out int? red, out int? green, out int? blue)
    {
        red = null;
        green = null;
        blue = null;

        while (round.Length > 0)
        {
            if (Helpers.ReadNext(ref round, out var valueBytes, ", "u8))
            {
                Helpers.ReadNext(ref valueBytes, out var numberCharBytes, " "u8);
                var value = Helpers.IntFromCharUtf8Bytes(numberCharBytes);
                if (valueBytes.SequenceEqual("green"u8))
                    green = value;
                else if (valueBytes.SequenceEqual("red"u8))
                    red = value;
                else if (valueBytes.SequenceEqual("blue"u8))
                    blue = value;
                else
                    throw new ArgumentException("Failed to parse color value");
            }
        }
    }
}
