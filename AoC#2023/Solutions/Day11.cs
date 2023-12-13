using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions
{
    internal class Day11 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            var (width, height) = GetExpandedDimensions(input);
            Span<char> expanded = stackalloc char[(width * height) + height];
            WriteExpandedToSpan(input, expanded);

            var space = expanded.AsSpan2D(0, height, width, 1);
            var universe = new Universe(space);
            Span<Galaxy> galaxies = stackalloc Galaxy[universe.NumGalaxies];
            universe.WriteGalaxies(galaxies);

            Console.WriteLine("Original");
            Console.WriteLine(input.ToString());

            Console.WriteLine("Expanded");
            Console.WriteLine(expanded.ToString());

            HashSet<(int A, int B)> pairs = [];
            var totalSteps = 0;
            for (int i = 0; i < galaxies.Length; i++)
                for (int j = 0; j < galaxies.Length; j++)
                {
                    if (i == j) continue;
                    if (pairs.Contains((i, j)) || pairs.Contains((j, i)))
                        continue;

                    pairs.Add((i, j));
                    var a = galaxies[i];
                    var b = galaxies[j];

                    var adjustment = 0;
                    var xDiff = Math.Abs(b.X - a.X);
                    var yDiff = Math.Abs(b.Y - a.Y);
                    if (yDiff > 0 && xDiff > 0)
                        adjustment = 0;

                    var steps = xDiff + yDiff + adjustment;
                    totalSteps += steps;
                }

            return totalSteps.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            return "X";
        }


        private static void WriteExpandedToSpan(ReadOnlySpan<char> input, Span<char> buffer)
        {
            var (width, height) = GetDimensions(input);
            var (expandedGridW, expandedGridH) = GetExpandedDimensions(input);
            var sourceGrid = input.AsSpan2D(0, height, width, 1);
            var size = (expandedGridW * sourceGrid.Height) + (sourceGrid.Height - 1);
            Span<char> expandedW = stackalloc char[size];
            for (var i = 0; i < sourceGrid.Height - 1; i++)
            {
                var writeOffset = (i * (expandedGridW + 1)) + expandedGridW;
                expandedW[writeOffset] = '\n';
            }
            var colOffset = 0;
            for (var x = 0; x < sourceGrid.Width; x++)
            {
                var isEmpty = true;
                var col = sourceGrid.GetColumn(x);
                var y = 0;

                foreach (var c in col)
                {
                    isEmpty &= c == '.';
                    var effectiveCol = x + colOffset;
                    var writeOffset = (y * (expandedGridW + 1)) + effectiveCol;
                    expandedW[writeOffset] = c;
                    y++;
                }

                if (isEmpty)
                {
                    colOffset++;
                    while (--y >= 0)
                    {
                        var effectiveCol = x + colOffset;
                        var writeOffset = (y * (expandedGridW + 1)) + x + colOffset;
                        expandedW[writeOffset] = '.';
                    }
                }
            }
            var colExpandedGrid = expandedW.AsSpan2D(0, sourceGrid.Height, expandedGridW, 1);

            for (var i = 0; i < expandedGridH - 1; i++)
            {
                var writeOffset = (i * (expandedGridW + 1)) + expandedGridW;
                buffer[writeOffset] = '\n';
            }
            var rowOffset = 0;
            for (var y = 0; y < colExpandedGrid.Height; y++)
            {
                var isEmpty = true;
                var row = colExpandedGrid.GetRow(y);
                var x = 0;

                foreach (var c in row)
                {
                    isEmpty &= c == '.';
                    var effectiveRow = y + rowOffset;

                    var writeOffset = (effectiveRow * (expandedGridW + 1)) + x;
                    buffer[writeOffset] = c;
                    x++;
                }

                if (isEmpty)
                {
                    rowOffset++;
                    while (--x >= 0)
                    {
                        var effectiveRow = y + rowOffset;
                        var writeOffset = (effectiveRow * (expandedGridW + 1)) + x;
                        buffer[writeOffset] = '.';
                    }
                }
            }
        }

        private static (int width, int height) GetExpandedDimensions(ReadOnlySpan<char> input)
        {
            var width = input.IndexOf('\n');
            var height = ReadOnlySpanExtensions.Count(input, '\n');

            var grid = input.AsSpan2D(0, height, width, 1);
            var emptyCols = 0;
            var emptyRows = 0;
            for (var i = 0; i < grid.Width; i++)
            {
                var isEmpty = true;
                var col = grid.GetColumn(i);
                foreach (var c in col)
                    isEmpty &= c == '.';

                if (isEmpty) emptyCols++;
            }
            for (var i = 0; i < grid.Height; i++)
            {
                var isEmpty = true;
                var row = grid.GetRowSpan(i);
                foreach (var c in row)
                    isEmpty &= c == '.';

                if (isEmpty) emptyRows++;
            }

            return (width + emptyCols, height + emptyRows);
        }

        private static (int width, int height) GetDimensions(ReadOnlySpan<char> input)
        {
            var width = input.IndexOf('\n');
            var height = ReadOnlySpanExtensions.Count(input, '\n');
            return (width, height);
        }
    }

    internal readonly ref struct Universe
    {
        private readonly Span2D<char> Space;
        public readonly int NumGalaxies;
        public Universe(Span2D<char> grid)
        {
            var numGalaxies = 0;
            for (var y = 0; y < grid.Height; y++)
            {
                var row = grid.GetRow(y);
                var x = 0;
                foreach (var c in row)
                {
                    if (c != '#') continue;
                    numGalaxies++;
                }
            }
            NumGalaxies = numGalaxies;
            Space = grid;
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
