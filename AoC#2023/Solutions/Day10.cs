
using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions;

internal class Day10 : ISolution
{
    public string Part1(ReadOnlySpan<char> input)
    {
        var grid = new Grid(input);
        var pathDown = grid.GetPathLength(Direction.Down);
        var pathUp = grid.GetPathLength(Direction.Up);
        var pathRight = grid.GetPathLength(Direction.Right);
        var pathLeft = grid.GetPathLength(Direction.Left);
        var pathTotal = (pathDown ?? 0) + (pathUp ?? 0) + (pathRight ?? 0) + (pathLeft ?? 0);
        return (pathTotal / 4).ToString();
    }

    public string Part2(ReadOnlySpan<char> input)
    {
        return "X";
    }


    internal readonly ref struct Grid
    {
        public readonly int Width;
        public readonly int Height;
        public int NumberOfNodes => Width * Height;
        private readonly ReadOnlySpan<char> _data;

        public Grid(ReadOnlySpan<char> input)
        {
            _data = input;
            Width = input.IndexOf('\n');
            Height = ReadOnlySpanExtensions.Count(input, '\n') + 1;
        }
        public char? GetCharAt(int x, int y)
        {
            if (x >= Width || x < 0) return null;
            if (y >= Height || y < 0) return null;
            var offset = (y * (Width + 1)) + x;
            return _data[offset];
        }

        public char? GetCharRelative((int x, int y) p, Direction direction)
        {
            var x = p.x + direction.Vector.x;
            var y = p.y + direction.Vector.y;
            return GetCharAt(x, y);
        }

        public char? GetCharAt((int x, int y) p) => GetCharAt(p.x, p.y);
        public char? GetRight((int x, int y) p) => GetCharAt(p.x + 1, p.y);
        public char? GetLeft((int x, int y) p) => GetCharAt(p.x - 1, p.y);
        public char? GetAbove((int x, int y) p) => GetCharAt(p.x, p.y - 1);
        public char? GetBelow((int x, int y) p) => GetCharAt(p.x, p.y + 1);

        public (int x, int y) Find(char c)
        {
            var y = 0;
            foreach (var line in _data.Tokenize('\n'))
            {
                var x = line.IndexOf(c);
                if (x != -1) return (x, y);
                y++;
            }
            return (-1, -1);
        }

        public Pipe Advance(Pipe current, Pipe previous)
        {
            if (current.OutletA.Vector == Direction.None.Vector && current.OutletB.Vector == Direction.None.Vector)
                return Pipe.GroundPipe;

            //Get the direction vector from our current to previous
            var directionToPrevious = new Direction(previous.Position.X - current.Position.X, previous.Position.Y - current.Position.Y);
            if (current.OutletA.Vector == directionToPrevious.Vector)
            {
                var nextPos = (current.Position.X + current.OutletB.Vector.x, current.Position.Y + current.OutletB.Vector.y);
                Pipe nextPipe = new(GetCharAt(nextPos), nextPos);
                if (!current.CanConnectTo(nextPipe))
                    return Pipe.GroundPipe;

                return nextPipe;
            }
            else if (current.OutletB.Vector == directionToPrevious.Vector)
            {
                var nextPos = (current.Position.X + current.OutletA.Vector.x, current.Position.Y + current.OutletA.Vector.y);
                Pipe nextPipe = new(GetCharAt(nextPos), nextPos);
                if (!current.CanConnectTo(nextPipe))
                    return Pipe.GroundPipe;

                return nextPipe;
            }

            return Pipe.GroundPipe;
        }

        public int? GetPathLength(Direction direction)
        {
            var startPosition = Find('S');
            var previous = new Pipe(GetCharAt(startPosition), startPosition);
            var currentPos = (startPosition.x + direction.Vector.x, startPosition.y + direction.Vector.y);
            var current = new Pipe(GetCharAt(currentPos), currentPos);
            var next = Advance(current, previous);
            var i = 2;
            while (!next.Ground && next.Position != startPosition)
            {
                previous = current;
                current = next;
                next = Advance(current, previous);
                i++;
            }

            if (next.Position == startPosition)
                return i;

            return null;
        }
    }

    internal readonly struct Pipe
    {
        private readonly Direction InletA;
        private readonly Direction InletB;
        public readonly Direction OutletA => InletA.Reverse;
        public readonly Direction OutletB => InletB.Reverse;

        public readonly char? Symbol;
        public readonly (int X, int Y) Position;
        public readonly bool Ground;

        public static Pipe GroundPipe => new(null, (0, 0));

        public Pipe(char? c, (int x, int y) position)
        {
            switch (c)
            {
                case 'F':
                    InletA = Direction.Left;
                    InletB = Direction.Up;
                    break;
                case '7':
                    InletA = Direction.Right;
                    InletB = Direction.Up;
                    break;
                case 'L':
                    InletA = Direction.Left;
                    InletB = Direction.Down;
                    break;
                case 'J':
                    InletA = Direction.Right;
                    InletB = Direction.Down;
                    break;
                case '-':
                    InletA = Direction.Right;
                    InletB = Direction.Left;
                    break;
                case '|':
                    InletA = Direction.Up;
                    InletB = Direction.Down;
                    break;
                default:
                    Ground = true;
                    InletA = Direction.None;
                    InletB = Direction.None;
                    break;
            }

            Position = (position.x, position.y);
            Symbol = c;
        }

        public bool CanConnectTo(Pipe other)
        {
            //Anything can terminate to S
            if (other.Symbol == 'S') return true;
            if (OutletA.Vector == other.InletA.Vector) return true;
            if (OutletA.Vector == other.InletB.Vector) return true;
            if (OutletB.Vector == other.InletA.Vector) return true;
            if (OutletB.Vector == other.InletB.Vector) return true;
            return false;
        }
    }

    internal readonly struct Direction
    {
        public static Direction Right => new(1, 0);
        public static Direction Left => new(-1, 0);
        public static Direction Up => new(0, -1);
        public static Direction Down => new(0, 1);
        public static Direction None => new(0, 0);
        public Direction Reverse => new(-1 * Vector.x, -1 * Vector.y);

        public readonly (int x, int y) Vector;
        internal Direction(int x, int y)
        {
            Vector = (x, y);
        }
    }
}
