
using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions;

internal class Day10 : ISolution
{
    public string Part1(ReadOnlySpan<char> input)
    {
        var grid = new Grid(input);
        var (_, length) = FindPath(grid);
        return (length / 2).ToString();
    }

    public string Part2(ReadOnlySpan<char> input)
    {
        var grid = new Grid(input);
        //grid.Print();

        var (direction, length) = FindPath(grid);
        Span<Pipe> path = stackalloc Pipe[length];
        grid.WritePath(direction, path);

        //Stretch the grid out by a factor of 2, to create space to squeeze out a tile
        var expandedWidth = grid.Width * 2;
        var expandedHeight = grid.Height * 2;
        Span<char> expandedGridInput = stackalloc char[((expandedWidth + 1) * expandedHeight) - 1];
        expandedGridInput.Fill('¥');
        for (var i = 0; i < (grid.Height * 2) - 1; i++)
        {
            var offset = (i * (expandedWidth + 1)) + expandedWidth;
            expandedGridInput[offset] = '\n';
        }

        for (var i = 0; i < grid.Width; i++)
            for (var j = 0; j < grid.Height - 1; j++)
            {
                var c = grid.GetCharAt(i, j);
                var scaledX = i * 2;
                var scaledY = j * 2;
                var offset = (scaledY * (expandedWidth + 1)) + scaledX;
                if (!c.HasValue)
                    continue;

                //We don't really care about the value of the tiles that are not part of the path
                //Therefore we'll make them . to make traversal easier later
                expandedGridInput[offset] = '.';
            }

        //Now write in the expanded out path
        for (var i = 0; i < path.Length; i++)
        {
            var current = path[i];
            var nextIdx = (i + 1) % path.Length;
            var next = path[nextIdx];

            var intermediateChar = current.GetIntermediatePipe(next);
            var d = current.GetConnectionDirection(next);

            var scaledX = current.Position.X * 2;
            var scaledY = current.Position.Y * 2;

            var scaledNextX = next.Position.X * 2;
            var scaledNextY = next.Position.Y * 2;

            var intermediateX = scaledX + d.Vector.x;
            var intermediateY = scaledY + d.Vector.y;

            var offset = (scaledY * (expandedWidth + 1)) + scaledX;
            var intermediateOffset = (intermediateY * (expandedWidth + 1)) + intermediateX;

            expandedGridInput[offset] = current.Symbol.Value;
            expandedGridInput[intermediateOffset] = intermediateChar;
        }

        var expandedGrid = new Grid(expandedGridInput);
        //expandedGrid.PrintPretty();


        //Start at the end of the expanded graph (the final row should be all '¥' characters from our expansion step
        //We can use this as a starting point to walk to the rest of the tiles that are outside of the pipe loop
        var visited = BFS(expandedGrid, expandedGrid.Width - 1, expandedHeight - 1);

        var enclosedCount = 0;
        for (var x = 0; x < expandedGrid.Width; x++)
            for (var y = 0; y < expandedGrid.Height; y++)
            {
                var offset = (y * (expandedGrid.Width + 1)) + x;

                //Since we injected a bunch of '¥' chars, we only want to count the dots
                if (expandedGrid.GetCharAt(x, y) != '.') continue;
                if (visited[x, y])
                {
                    continue;
                }

                expandedGridInput[offset] = '$';
                enclosedCount++;
            }

        return enclosedCount.ToString();
    }

    private static bool[,] BFS(Grid grid, int startX, int startY)
    {
        var queue = new Queue<(int x, int y)>();
        var visited = new bool[grid.Width, grid.Height];
        queue.Enqueue((startX, startY));
        visited[startX, startY] = true;
        Span<Direction> directions = [Direction.Left, Direction.Right, Direction.Up, Direction.Down];
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            for (int i = 0; i < directions.Length; i++)
            {
                var nextX = x + directions[i].Vector.x;
                var nextY = y + directions[i].Vector.y;

                if (nextX < 0 || nextX >= grid.Width || nextY < 0 || nextY >= grid.Height)
                    continue;

                //If the neighbouring node is a '.' or '#' and we have not visited it, then add it to our queue 
                var nextChar = grid.GetCharAt(nextX, nextY);
                if ((nextChar == '.' || nextChar == '¥') && !visited[nextX, nextY])
                {
                    queue.Enqueue((nextX, nextY));
                    visited[nextX, nextY] = true;
                }
            }
        }
        return visited;
    }

    private static (Direction, int) FindPath(Grid grid)
    {
        var startPos = grid.Find('S');
        var pathDown = grid.GetPathLength(startPos, Direction.Down);
        if (pathDown.HasValue) return (Direction.Down, pathDown.Value);

        var pathUp = grid.GetPathLength(startPos, Direction.Up);
        if (pathUp.HasValue) return (Direction.Up, pathUp.Value);

        var pathRight = grid.GetPathLength(startPos, Direction.Right);
        if (pathRight.HasValue) return (Direction.Right, pathRight.Value);

        var pathLeft = grid.GetPathLength(startPos, Direction.Left);
        if (pathLeft.HasValue) return (Direction.Left, pathLeft.Value);

        return (Direction.None, 0);
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


        public void PrintPath(Span<Pipe> path)
        {
            Span<char> prettied = stackalloc char[_data.Length];
            _data.CopyTo(prettied);

            foreach (var pipe in path)
            {
                var offset = (pipe.Position.Y * (Width + 1)) + pipe.Position.X;
                prettied[offset] = '&';
            }

            foreach (var c in prettied)
                Console.Write(c);

            Console.WriteLine();
        }

        public void Print()
        {
            var (direction, length) = FindPath();
            Span<Pipe> path = new Pipe[length];
            WritePath(direction, path);

            Span<char> prettied = stackalloc char[_data.Length];
            _data.CopyTo(prettied);

            foreach (var pipe in path)
            {
                var offset = (pipe.Position.Y * (Width + 1)) + pipe.Position.X;
                prettied[offset] = pipe.Symbol.Value;
            }

            foreach (var c in prettied)
                Console.Write(c);

            Console.WriteLine();
        }

        public void PrintPretty()
        {
            var (direction, length) = FindPath();
            Span<Pipe> path = new Pipe[length];
            WritePath(direction, path);

            Span<char> prettied = stackalloc char[_data.Length];
            _data.CopyTo(prettied);

            foreach (var pipe in path)
            {
                var offset = (pipe.Position.Y * (Width + 1)) + pipe.Position.X;
                var symbol = pipe.Symbol!.Value;
                var prettyChar = symbol switch
                {
                    'F' => '┌',
                    '7' => '┐',
                    'J' => '┘',
                    'L' => '└',
                    '-' => '─',
                    '|' => '│',
                    _ => symbol
                };
                prettied[offset] = prettyChar;
            }

            foreach (var c in prettied)
                Console.Write(c);

            Console.WriteLine();
        }

        public (Direction, int) FindPath()
        {
            var startPos = Find('S');
            var pathDown = GetPathLength(startPos, Direction.Down);
            if (pathDown.HasValue) return (Direction.Down, pathDown.Value);

            var pathUp = GetPathLength(startPos, Direction.Up);
            if (pathUp.HasValue) return (Direction.Up, pathUp.Value);

            var pathRight = GetPathLength(startPos, Direction.Right);
            if (pathRight.HasValue) return (Direction.Right, pathRight.Value);

            var pathLeft = GetPathLength(startPos, Direction.Left);
            if (pathLeft.HasValue) return (Direction.Left, pathLeft.Value);

            return (Direction.None, 0);
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

        public int? GetPathLength((int x, int y) startPosition, Direction direction)
        {
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

        public void WritePath(Direction direction, Span<Pipe> buffer)
        {
            var startPosition = Find('S');
            var previous = new Pipe(GetCharAt(startPosition), startPosition);
            var currentPos = (startPosition.x + direction.Vector.x, startPosition.y + direction.Vector.y);
            var current = new Pipe(GetCharAt(currentPos), currentPos);
            var next = Advance(current, previous);
            buffer[0] = previous;
            buffer[1] = current;
            var i = 2;
            while (!next.Ground && next.Position != startPosition)
            {
                buffer[i++] = next;
                previous = current;
                current = next;
                next = Advance(current, previous);
            }
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
            //Grounds can connect together for our part 2 solution
            //if (Symbol == '.' && other.Symbol == '.') return true;
            //if (Symbol == '.' && other.Symbol == '#') return true;
            //if (Symbol == '#' && other.Symbol == '.') return true;

            //Anything can terminate to S
            if (other.Symbol == 'S') return true;
            if (OutletA.Vector == other.InletA.Vector) return true;
            if (OutletA.Vector == other.InletB.Vector) return true;
            if (OutletB.Vector == other.InletA.Vector) return true;
            if (OutletB.Vector == other.InletB.Vector) return true;
            return false;
        }

        public Direction GetConnectionDirection(Pipe next)
        {
            return new Direction(next.Position.X - Position.X, next.Position.Y - Position.Y);
        }


        //If we're trying to "stretch" the connection between two pipes, this will tell us which pipe to place between our pipe and the next pipe
        public char GetIntermediatePipe(Pipe next)
        {
            var d = GetConnectionDirection(next);
            if (d.Vector.x != 0) return '-';
            if (d.Vector.y != 0) return '|';
            return '?';
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
