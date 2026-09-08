using GpuRagBenchmark.Services.OpenAI.ChatGeneral.Common;

namespace GpuRagBenchmark.Services.OpenAI.ChatGeneral;

public interface IChatService
{
    Task<string> AskAsync(string request, string context, CancellationToken cancellationToken);
    Task<RetrievalResult> RetrieveDocumentAsync(string request, CancellationToken cancellationToken);
}
