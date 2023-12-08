// See https://aka.ms/new-console-template for more information
using AoC_2023;
using AoC_2023.Solutions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;


var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<AdventClientOptions>(builder.Configuration.GetSection("AdventClient"));
builder.Services.AddHttpClient<AdventClient>((serviceProvider, client) =>
{
    var adventOptions = serviceProvider.GetRequiredService<IOptions<AdventClientOptions>>().Value;
    client.BaseAddress = new Uri($"{adventOptions.BaseUrl}");
})
.ConfigurePrimaryHttpMessageHandler((serviceProvider) =>
{
    var adventOptions = serviceProvider.GetRequiredService<IOptions<AdventClientOptions>>().Value;
    var cookies = new CookieContainer();
    cookies.Add(new Cookie("session", adventOptions.SessionCookie, "/", ".adventofcode.com") { HttpOnly = true, Secure = true });
    return new SocketsHttpHandler
    {
        CookieContainer = cookies,
        UseCookies = true
    };
});
builder.Services.AddKeyedSingleton<ISolution, Day1>(1);
builder.Services.AddKeyedSingleton<ISolution, Day2>(2);
builder.Services.AddKeyedSingleton<IPiplinesSolution, Day2Pipelines>(2);
builder.Services.AddKeyedSingleton<ISolution, Day3>(3);
builder.Services.AddKeyedSingleton<ISolution, Day4>(4);
builder.Services.AddKeyedSingleton<IPiplinesSolution, Day4Pipelines>(4);
builder.Services.AddKeyedSingleton<ISolution, Day5>(5);
builder.Services.AddKeyedSingleton<ISolution, Day6>(6);
builder.Services.AddKeyedSingleton<ISolution, Day7>(7);
builder.Services.AddKeyedSingleton<ISolution, Day8>(8);
builder.Services.AddSingleton<Solver>();

var app = builder.Build();
var runner = app.Services.GetRequiredService<Solver>();
await app.StartAsync();

var day = 8;
await runner.Solve(day);
await runner.SolvePipelines(day);

await app.StopAsync();


internal class Solver(AdventClient client, ILogger<Solver> logger, IServiceProvider serviceProvider)
{
    public async Task Solve(int day)
    {
        var solution = serviceProvider.GetKeyedService<ISolution>(day);
        if (solution == null)
        {
            logger.LogInformation("{Day} solution not found", day);
            return;
        }
        var input = await client.DownloadInputFileAsync(day);

        //var input =
        //    """
        //    LLR

        //    AAA = (BBB, BBB)
        //    BBB = (AAA, ZZZ)
        //    ZZZ = (ZZZ, ZZZ)
        //    """;


        var sw = Stopwatch.StartNew();
        var part1 = solution.Part1(input);
        var p1Timing = sw.ElapsedMilliseconds;
        logger.LogInformation("{Day} Part 1: {Answer} ({ElapsedMs}ms)", day, part1, p1Timing);
        var part2 = solution.Part2(input);
        var p2Timing = sw.ElapsedMilliseconds - p1Timing;
        logger.LogInformation("{Day} Part 2: {Answer} ({ElapsedMs}ms)", day, part2, p2Timing);
        sw.Stop();

    }

    public async Task SolvePipelines(int day)
    {
        var solution = serviceProvider.GetKeyedService<IPiplinesSolution>(day);
        if (solution == null)
        {
            logger.LogInformation("{Day} solution not found", day);
            return;
        }
        var p1Reader = await client.GetInputAsPipeReader(day);
        var p2Reader = await client.GetInputAsPipeReader(day);
        var sw = Stopwatch.StartNew();
        var p1 = await solution.Part1(p1Reader);
        var p1Timing = sw.ElapsedMilliseconds;
        logger.LogInformation("{Day} Part 1: {Answer} ({Elapsed}ms)", day, p1, p1Timing);
        var p2 = await solution.Part2(p2Reader);
        var p2Timing = sw.ElapsedMilliseconds - p1Timing;
        logger.LogInformation("{Day} Part 2: {Answer} ({Elapsed}ms)", day, p2, p2Timing);
    }
}
