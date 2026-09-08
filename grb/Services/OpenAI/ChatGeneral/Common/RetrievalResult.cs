using GpuRagBenchmark.Services.OpenAI.Ranker.Common;

namespace GpuRagBenchmark.Services.OpenAI.ChatGeneral.Common;

public sealed record RetrievalResult(
    string Context, 
    IReadOnlyList<RankedChunk> RankedChunks,
    IReadOnlyList<RankedChunk> SelectedChunks,
    int VectorTopK,
    int FinalTopK,
    long LatencyMs
); 
