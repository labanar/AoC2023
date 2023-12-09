using System.IO.Pipelines;

namespace AoC_2023.Solutions
{
    internal interface ISolution
    {
        string Part1(ReadOnlySpan<char> input);
        string Part2(ReadOnlySpan<char> input);
    }

    internal interface IPiplinesSolution
    {
        Task<string> Part1(PipeReader pipe);
        Task<string> Part2(PipeReader pipe);
    }
}
