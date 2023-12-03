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
builder.Services.AddKeyedSingleton<ISolution, Day3>(3);
builder.Services.AddSingleton<Runner>();

var app = builder.Build();
var runner = app.Services.GetRequiredService<Runner>();
await app.StartAsync();

var day = 3;
await runner.RunAsync(day);

await app.StopAsync();


internal class Runner(AdventClient client, ILogger<Runner> logger, IServiceProvider serviceProvider)
{
    public async Task RunAsync(int day)
    {
        var solution = serviceProvider.GetRequiredKeyedService<ISolution>(day);
        var input = await client.DownloadInputFileAsync(day);
        var part1 = solution.Part1(input);
        logger.LogInformation("Part 1: {Answer}", part1);
        var part2 = solution.Part2(input);
        logger.LogInformation("Part 2: {Answer}", part2);
    }
}
