using AoC_2023.Utilities;
using CommunityToolkit.HighPerformance;

namespace AoC_2023.Solutions
{
    internal class Day8 : ISolution
    {
        public string Part1(ReadOnlySpan<char> input)
        {
            input.ReadTo(out var instructions, '\n');
            input = input.Trim();
            var nodeMap = ReadNodes(input);
            var endNodeId = new NodeId("ZZZ");
            var current = new NodeId("AAA");
            var steps = 0;
            while (current.Id != endNodeId.Id)
            {
                foreach (var instruction in instructions)
                {
                    current = instruction switch
                    {
                        'L' => nodeMap[current].Left,
                        'R' => nodeMap[current].Right,
                        _ => throw new ArgumentException("invalid instruction")
                    };
                    steps += 1;
                    if (current.Id == endNodeId.Id)
                        break;
                }
            }
            return steps.ToString();
        }

        public string Part2(ReadOnlySpan<char> input)
        {
            input.ReadTo(out var instructions, '\n');
            input = input.Trim();

            var nodeMap = ReadNodes(input);
            var numNodes = nodeMap.Count(x => x.Key.EndsWith == 'A');
            Span<NodeId> startingNodes = stackalloc NodeId[numNodes];
            var i = 0;
            foreach (var node in nodeMap.Where(x => x.Key.EndsWith == 'A'))
                startingNodes[i++] = node.Key;

            Span<TerminalNode> terminalNodes = stackalloc TerminalNode[numNodes];
            for (i = 0; i < numNodes; i++)
                terminalNodes[i] = FindTerminalNode(startingNodes[i], instructions, nodeMap);

            var num = terminalNodes[0].Steps;
            for (i = 1; i < numNodes; i++)
                num = LCM(num, terminalNodes[i].Steps);

            return num.ToString();
        }

        private static Dictionary<NodeId, (NodeId Left, NodeId Right)> ReadNodes(ReadOnlySpan<char> nodesRaw)
        {
            var nodes = new Dictionary<NodeId, (NodeId left, NodeId right)>();
            foreach (var token in nodesRaw.Tokenize('\n'))
            {
                var line = token;
                line.ReadTo(out var nodeId, " = ");
                line = line.TrimStart('(').TrimEnd(')');
                line.ReadTo(out var leftNodeId, ", ");
                var rightNodeId = line;
                nodes.Add(new(nodeId), (new(leftNodeId), new(rightNodeId)));
            }
            return nodes;
        }

        private static TerminalNode FindTerminalNode(NodeId node, ReadOnlySpan<char> instructions, Dictionary<NodeId, (NodeId Left, NodeId Right)> nodeMap)
        {
            var current = node;
            var steps = 0L;
            while (current.EndsWith != 'Z')
            {
                foreach (var instruction in instructions)
                {
                    current = instruction switch
                    {
                        'L' => nodeMap[current].Left,
                        'R' => nodeMap[current].Right,
                        _ => throw new ArgumentException("invalid instruction")
                    };
                    steps++;
                }
            }
            return new(node, current, steps);
        }

        private static long GCD(long a, long b)
        {
            if (a > b)
            {
                var rem = a % b;
                if (rem == 0) return b;
                return GCD(b, rem);
            }
            else
                return GCD(b, a);
        }

        private static long LCM(long a, long b) => a * b / GCD(a, b);
    }


    public readonly struct TerminalNode
    {
        public readonly NodeId StartNode;
        public readonly NodeId EndNode;
        public readonly long Steps;

        public TerminalNode(NodeId start, NodeId end, long steps)
        {
            StartNode = start;
            EndNode = end;
            Steps = steps;
        }
    }

    public readonly struct NodeId
    {
        public readonly int Id;
        public readonly char EndsWith;
        public NodeId(ReadOnlySpan<char> nodeIdChars)
        {
            var id = 0;
            var factor = 1;
            for (var i = 0; i < nodeIdChars.Length; i++)
            {
                var c = nodeIdChars[nodeIdChars.Length - 1 - i];
                var numericValue = c - 'A';
                id += numericValue * factor;
                factor *= 100;
            }
            Id = id;
            EndsWith = nodeIdChars[nodeIdChars.Length - 1];
        }
    }
}
