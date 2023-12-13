using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions
{
    internal class Day11 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input) =>
            CalculateExpandedDistances(input, 2).ToString();

        public string Part2(ReadOnlySpan<char> input) =>
            CalculateExpandedDistances(input, 1000000).ToString();

        private long CalculateExpandedDistances(ReadOnlySpan<char> input, int expansionFactor)
        {
            var universe = new Universe(input, expansionFactor);
            Span<int> rowExpansions = stackalloc int[universe.Space.Height];
            rowExpansions.Fill(1);
            Span<int> colExpansions = stackalloc int[universe.Space.Width];
            colExpansions.Fill(1);
            universe.Expand(rowExpansions, colExpansions);
            Span<Galaxy> galaxies = stackalloc Galaxy[universe.NumGalaxies];
            universe.WriteGalaxies(galaxies);
            var totalSteps = 0L;
            for (int i = 0; i < galaxies.Length - 1; i++)
                for (int j = i + 1; j < galaxies.Length; j++)
                {
                    var a = galaxies[i];
                    var b = galaxies[j];
                    var xMin = Math.Min(b.X, a.X);
                    var xMax = Math.Max(a.X, b.X);
                    var yMin = Math.Min(a.Y, b.Y);
                    var yMax = Math.Max(a.Y, b.Y);
                    var steps = 0L;

                    for (var x = xMin; x < xMax; x++)
                        steps += colExpansions[x];

                    for (var y = yMin; y < yMax; y++)
                        steps += rowExpansions[y];

                    totalSteps += steps;
                }

            return totalSteps;
        }
    }

    internal readonly ref struct Universe
    {
        public readonly ReadOnlySpan2D<char> Space;
        public readonly int NumGalaxies;
        private readonly int _expansionFactor;

        public Universe(ReadOnlySpan<char> input, int expansionFactor)
        {
            var width = input.IndexOf('\n');
            var height = ReadOnlySpanExtensions.Count(input, '\n');
            Space = input.AsSpan2D(0, height, width, 1);
            NumGalaxies = ReadOnlySpanExtensions.Count(input, '#');
            _expansionFactor = expansionFactor;
        }

        public void WriteGalaxies(Span<Galaxy> buffer)
        {
            var i = 0;
            for (var y = 0; y < Space.Height; y++)
            {
                var row = Space.GetRow(y);
                var x = 0;
                foreach (var c in row)
                {
                    if (c == '#')
                        buffer[i++] = new(i, x, y);

                    x++;
                }
            }
        }


        public void Expand(Span<int> rowExpansions, Span<int> colExpansions)
        {
            for (var i = 0; i < Space.Width; i++)
            {
                var isEmpty = true;
                var col = Space.GetColumn(i);
                foreach (var c in col)
                    isEmpty &= c == '.';

                if (isEmpty)
                    colExpansions[i] = _expansionFactor;
            }
            for (var i = 0; i < Space.Height; i++)
            {
                var isEmpty = true;
                var row = Space.GetRowSpan(i);
                foreach (var c in row)
                    isEmpty &= c == '.';

                if (isEmpty)
                    rowExpansions[i] = _expansionFactor;
            }
        }


    }

    internal readonly struct Galaxy
    {
        public readonly int Id;
        public readonly int X;
        public readonly int Y;

        public Galaxy(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }
}
