using grb.Services.OpenAI.Ranker.Common;

namespace grb.Services.OpenAI.Ranker;

public interface IChunkRanker
{
    Task<IReadOnlyList<RankedChunk>> RankAsync(
        string query,
        IReadOnlyList<string> chunks,
        CancellationToken cancellationToken
    );
}
