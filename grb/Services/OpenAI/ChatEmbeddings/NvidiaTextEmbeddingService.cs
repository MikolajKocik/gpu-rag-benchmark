using grb.Services.OpenAI.ChatGeneral.Common;
using Microsoft.Extensions.Options;
using OpenAI;

namespace grb.Services.OpenAI.ChatEmbeddings;

public sealed class NvidiaTextEmbeddingService(
    OpenAIClient openAIClient,
    IOptions<NvidiaOptions> options
) : BaseTextEmbeddingService(openAIClient.GetEmbeddingClient(options.Value.Embedding));



