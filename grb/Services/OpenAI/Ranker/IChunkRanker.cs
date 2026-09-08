using GpuRagBenchmark.Services.OpenAI.Ranker.Common;

namespace GpuRagBenchmark.Services.OpenAI.Ranker;

public interface IChunkRanker
{
    Task<IReadOnlyList<RankedChunk>> RankAsync(
        string query,
        IReadOnlyList<string> chunks,
        CancellationToken cancellationToken
    );
}
