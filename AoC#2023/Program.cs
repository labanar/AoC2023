// See https://aka.ms/new-console-template for more information
using AoC_2023;
using AoC_2023.Solutions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
builder.Services.AddSingleton<Solver>();

var app = builder.Build();
var runner = app.Services.GetRequiredService<Solver>();
await app.StartAsync();

var day = 2;
await runner.Solve(day);
await runner.SolvePipelines(day);

await app.StopAsync();


internal class Solver(AdventClient client, ILogger<Solver> logger, IServiceProvider serviceProvider)
{
    public async Task Solve(int day)
    {
        var solution = serviceProvider.GetRequiredKeyedService<ISolution>(day);
        var input = await client.DownloadInputFileAsync(day);
        var part1 = solution.Part1(input);
        logger.LogInformation("{Day} Part 1: {Answer}", day, part1);
        var part2 = solution.Part2(input);
        logger.LogInformation("{Day} Part 2: {Answer}", day, part2);
    }

    public async Task SolvePipelines(int day)
    {
        var solution = serviceProvider.GetRequiredKeyedService<IPiplinesSolution>(day);
        var p1 = await solution.Part1(await client.GetInputAsPipeReader(day));
        var p2 = await solution.Part2(await client.GetInputAsPipeReader(day));
        logger.LogInformation("{Day} Part 1: {Answer}", day, p1);
        logger.LogInformation("{Day} Part 1: {Answer}", day, p2);
    }
}
