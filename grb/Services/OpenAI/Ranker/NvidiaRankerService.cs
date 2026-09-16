using System.Text.Json.Serialization;
using grb.Services.OpenAI.ChatGeneral.Common;
using grb.Services.OpenAI.Ranker.Common;
using Microsoft.Extensions.Options;

namespace grb.Services.OpenAI.Ranker;

public sealed class NvidiaRankerService(
    HttpClient httpClient,
    IOptions<NvidiaOptions> options
) : IChunkRanker
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _modelName = options.Value.Ranker;

    public async Task<IReadOnlyList<RankedChunk>> RankAsync(
        string query,
        IReadOnlyList<string> chunks,
        CancellationToken ct)
    {
        if (chunks.Count == 0) return Array.Empty<RankedChunk>();

        // NIM format
        var requestBody = new
        {
            model = _modelName,
            query = query,
            passages = chunks.Select(c => new { text = c }).ToArray()
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("ranking", requestBody, ct);
        response.EnsureSuccessStatusCode();

        NvidiaRankingResponse? result =
            await response.Content.ReadFromJsonAsync<NvidiaRankingResponse>(cancellationToken: ct);

        var rankedChunks = new List<RankedChunk>();
        if (result?.Rankings != null)
        {
            foreach (RankingItem ranking in result.Rankings)
            {
                rankedChunks.Add(new RankedChunk(
                    chunks[ranking.Index],
                    ranking.Logit,
                    "nvidia"
                ));
            }
        }

        // Zwracamy posortowane malejąco po logitach (score)
        return rankedChunks.OrderByDescending(r => r.Score).ToList();
    }

    private sealed class NvidiaRankingResponse
    {
        [JsonPropertyName("rankings")]
        public List<RankingItem>? Rankings { get; set; }
    }

    private sealed class RankingItem
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("logit")]
        public float Logit { get; set; }
    }
}
