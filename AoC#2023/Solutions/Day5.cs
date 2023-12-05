using AoC_2023.Utilities;
using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions
{
    internal class Day5 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            Helpers.ReadLine(ref input, out var seedLine);
            var seedIdsRaw = seedLine[7..];
            Helpers.ReadLine(ref input, out _);
            Helpers.ReadLine(ref input, out _);
            var seedToSoilMap = ReadMap(ref input, "soil-to-fertilizer map:\n");
            var soilToFertMap = ReadMap(ref input, "fertilizer-to-water map:\n");
            var fertToWaterMap = ReadMap(ref input, "water-to-light map:\n");
            var waterToLightMap = ReadMap(ref input, "light-to-temperature map:\n");
            var lightToTempMap = ReadMap(ref input, "temperature-to-humidity map:\n");
            var tempToHumidMap = ReadMap(ref input, "humidity-to-location map:\n");
            long minLocationId = long.MaxValue;
            foreach (var seedIdToken in seedIdsRaw.Tokenize(' '))
            {
                if (seedIdToken.Length == 0) continue;
                var seedId = long.Parse(seedIdToken);
                var soilId = GetDestValue(seedToSoilMap, seedId);
                var fertId = GetDestValue(soilToFertMap, soilId);
                var waterId = GetDestValue(fertToWaterMap, fertId);
                var lightId = GetDestValue(waterToLightMap, waterId);
                var tempId = GetDestValue(lightToTempMap, lightId);
                var humidId = GetDestValue(tempToHumidMap, tempId);
                var locationId = GetDestValue(input, humidId);
                if (locationId < minLocationId) minLocationId = locationId;
            }
            return minLocationId.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            Helpers.ReadLine(ref input, out var seedLine);
            var seedIdsRaw = seedLine[7..];
            Helpers.ReadLine(ref input, out _);
            Helpers.ReadLine(ref input, out _);
            var seedToSoilMap = ReadMap(ref input, "soil-to-fertilizer map:\n");
            var soilToFertMap = ReadMap(ref input, "fertilizer-to-water map:\n");
            var fertToWaterMap = ReadMap(ref input, "water-to-light map:\n");
            var waterToLightMap = ReadMap(ref input, "light-to-temperature map:\n");
            var lightToTempMap = ReadMap(ref input, "temperature-to-humidity map:\n");
            var tempToHumidMap = ReadMap(ref input, "humidity-to-location map:\n");
            long minLocationId = long.MaxValue;
            while (seedIdsRaw.Length > 0)
            {
                seedIdsRaw.ReadTo(out var seedRangeStartChars, ' ');
                seedIdsRaw.ReadTo(out var seedRangeChars, ' ');
                var seedRangeStart = long.Parse(seedRangeStartChars);
                var seedRange = long.Parse(seedRangeChars);
                var soilRanges = GetDestRanges(seedToSoilMap, seedRangeStart, seedRange);
                var fertRanges = new List<(long, long)>();
                foreach (var r in soilRanges)
                    fertRanges.AddRange(GetDestRanges(soilToFertMap, r.Item1, r.Item2));

                var waterRanges = new List<(long, long)>();
                foreach (var r in fertRanges)
                    waterRanges.AddRange(GetDestRanges(fertToWaterMap, r.Item1, r.Item2));

                var lightRanges = new List<(long, long)>();
                foreach (var r in waterRanges)
                    lightRanges.AddRange(GetDestRanges(waterToLightMap, r.Item1, r.Item2));

                var tempRanges = new List<(long, long)>();
                foreach (var r in lightRanges)
                    tempRanges.AddRange(GetDestRanges(lightToTempMap, r.Item1, r.Item2));

                var humidRanges = new List<(long, long)>();
                foreach (var r in tempRanges)
                    humidRanges.AddRange(GetDestRanges(tempToHumidMap, r.Item1, r.Item2));

                foreach (var r in humidRanges)
                    foreach (var locationRange in GetDestRanges(input, r.Item1, r.Item2))
                        if (locationRange.Item1 < minLocationId) minLocationId = locationRange.Item1;
            }

            return minLocationId.ToString();
        }

        private static ReadOnlySpan<char> ReadMap(ref ReadOnlySpan<char> input, ReadOnlySpan<char> endDelimiter)
        {
            input.ReadTo(out var map, endDelimiter);
            map = map.TrimEnd('\n');
            return map;
        }

        private static long GetDestValue(ReadOnlySpan<char> map, long srcValue)
        {
            foreach (var mapToken in map.Tokenize('\n'))
            {
                if (mapToken.Trim().Length == 0) continue;
                var mapLine = mapToken.Trim();
                mapLine.ReadTo(out var destRangeStartChars, ' ');
                mapLine.ReadTo(out var srcRangeStartChars, ' ');
                var destRangeStart = long.Parse(destRangeStartChars);
                var srcRangeStart = long.Parse(srcRangeStartChars);
                var rangeLength = long.Parse(mapLine);

                if (srcValue >= srcRangeStart && srcValue < srcRangeStart + rangeLength)
                {
                    var distanceToStart = srcValue - srcRangeStart;
                    return destRangeStart + distanceToStart;
                }
            }
            return srcValue;
        }

        private static List<(long, long)> GetDestRanges(ReadOnlySpan<char> map, long srcValue, long range)
        {
            var ranges = new List<(long, long)>();
            foreach (var mapToken in map.Tokenize('\n'))
            {
                if (mapToken.Trim().Length == 0) continue;
                var mapLine = mapToken.Trim();
                mapLine.ReadTo(out var destRangeStartChars, ' ');
                mapLine.ReadTo(out var srcRangeStartChars, ' ');
                var destRangeStart = long.Parse(destRangeStartChars);
                var srcRangeStart = long.Parse(srcRangeStartChars);
                var mapRange = long.Parse(mapLine);

                if (srcValue < srcRangeStart && srcValue + range >= srcRangeStart + mapRange)
                {
                    ranges.Add((destRangeStart, mapRange));
                }
                else if (srcValue >= srcRangeStart && srcValue + range <= srcRangeStart + mapRange)
                {
                    var distanceToStart = srcValue - srcRangeStart;

                    ranges.Add((destRangeStart + distanceToStart, range));
                }
                else if (srcValue >= srcRangeStart && srcValue + range > srcRangeStart + mapRange && srcValue < srcRangeStart + mapRange)
                {
                    var maxValue = srcRangeStart + mapRange;
                    var adjustedRange = maxValue - srcValue;
                    var distanceToStart = srcValue - srcRangeStart;
                    ranges.Add((destRangeStart + distanceToStart, adjustedRange));

                    var nextSrcValue = srcValue + adjustedRange;
                    var nextRange = range - adjustedRange;
                    ranges.AddRange(GetDestRanges(map, nextSrcValue, nextRange));
                }
            }

            if (ranges.Count == 0)
                ranges.Add((srcValue, range));

            return ranges;
        }

    }
}
