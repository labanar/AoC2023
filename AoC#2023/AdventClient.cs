using Microsoft.Extensions.Options;
using System.IO.Pipelines;

namespace AoC_2023
{
    public class AdventClientOptions
    {
        public string BaseUrl { get; init; } = "https://adventofcode.com/";
        public required string SessionCookie { get; init; }
        public required int Year { get; init; }
    }

    public class AdventClient
    {
        private readonly int _year;
        private readonly HttpClient _client;

        public AdventClient(HttpClient client, IOptions<AdventClientOptions> adventClientOptions)
        {
            _client = client;
            _year = adventClientOptions.Value.Year;
        }

        public async Task<string> DownloadInputFileAsync(int day) =>
            await _client.GetStringAsync($"/{_year}/day/{day}/input");

        public async Task<PipeReader> GetInputAsPipeReader(int day)
        {
            var response = await _client.GetAsync($"/{_year}/day/{day}/input", HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            return PipeReader.Create(stream);
        }

        public async Task<bool> SubmitAnswerAsync(int day, int part, string answer)
        {
            var content = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("level", part.ToString()),
                    new KeyValuePair<string, string>("answer", answer)
                });

            using var response = await _client.PostAsync($"/{_year}/day/{day}/answer", content);
            return await IsAnswerCorrect(response);
        }

        private async Task<bool> IsAnswerCorrect(HttpResponseMessage response)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            if (content.Contains("That's not the right answer"))
                return false;

            return true;
        }
    }
}
