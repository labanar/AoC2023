using AoC_2023.Utilities;
using CommunityToolkit.HighPerformance;
using System.Buffers;
using System.IO.Pipelines;

namespace AoC_2023.Solutions;

internal sealed class Day2 : ISolution
{
    public string Part1(ReadOnlySpan<char> input)
    {
        var total = 0;
        foreach (var line in input.Tokenize('\n'))
        {
            if (line.Length == 0) continue;
            (var gameId, var red, var green, var blue) = ReadGame(line);
            if (red <= 12 && green <= 13 && blue <= 14)
                total += gameId;
        }
        return total.ToString();
    }

    public string Part2(ReadOnlySpan<char> input)
    {
        var total = 0;
        foreach (var line in input.Tokenize('\n'))
        {
            if (line.Length == 0) continue;
            (_, var red, var green, var blue) = ReadGame(line);
            var power = 1;
            if (red.HasValue) power *= red.Value;
            if (green.HasValue) power *= green.Value;
            if (blue.HasValue) power *= blue.Value;
            total += power;
        }
        return total.ToString();
    }

    private static (int gameId, int? red, int? green, int? blue) ReadGame(ReadOnlySpan<char> line)
    {
        line.ReadTo(out _, " ");
        line.ReadTo(out var gameIdChars, ":");
        var gameId = int.Parse(gameIdChars);
        (var maxRed, var maxGreen, var maxBlue) = GetMaxObservedCubes(line);
        return (gameId, maxRed, maxGreen, maxBlue);
    }

    private static (int? maxRed, int? maxGreen, int? maxBlue) GetMaxObservedCubes(ReadOnlySpan<char> rounds)
    {
        int? red = null;
        int? green = null;
        int? blue = null;
        foreach (var roundToken in rounds.Tokenize(';'))
        {
            if (roundToken.Length == 0) continue;
            var round = roundToken.Trim();
            if (round.Length == 0) continue;
            foreach (var valueToken in round.Tokenize(','))
            {
                if (valueToken.Length == 0) continue;
                var value = valueToken.Trim();
                if (value.Length == 0) continue;
                value.ReadTo(out var numberChars, " ");
                var number = int.Parse(numberChars);
                switch (value)
                {
                    case "green":
                        green.SetIfGreater(number);
                        break;
                    case "blue":
                        blue.SetIfGreater(number);
                        break;
                    case "red":
                        red.SetIfGreater(number);
                        break;
                }
            }
        }
        return (red, green, blue);
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
