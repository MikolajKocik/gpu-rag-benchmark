using Azure.AI.OpenAI;
using grb.Services.OpenAI.ChatGeneral.Common;
using Microsoft.Extensions.Options;

namespace grb.Services.OpenAI.ChatEmbeddings;

public sealed class TextEmbeddingService(
    AzureOpenAIClient openAIClient,
    IOptions<AzureOpenAIOptions> options
) : BaseTextEmbeddingService(openAIClient.GetEmbeddingClient(options.Value.EmbeddingDeploymentName));



