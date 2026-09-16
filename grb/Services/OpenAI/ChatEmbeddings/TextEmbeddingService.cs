using System.ClientModel;
using Azure.AI.OpenAI;
using grb.Services.OpenAI.ChatGeneral.Common;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace grb.Services.OpenAI.ChatEmbeddings;

public sealed class TextEmbeddingService : ITextEmbeddingService
{
    private readonly EmbeddingClient _embeddingClient;
    public TextEmbeddingService(
        AzureOpenAIClient openAIClient,
        IOptions<AzureOpenAIOptions> options)
    {
        _embeddingClient = openAIClient.GetEmbeddingClient(
            options.Value.EmbeddingDeploymentName);
    }

    public async Task<float[]> GetEmbeddingAsync(string input, CancellationToken cancellationToken)
    {
        ClientResult<OpenAIEmbedding> response = await _embeddingClient.GenerateEmbeddingAsync(
            input,
            null,
            cancellationToken
        );

        return response.Value.ToFloats().ToArray();
    }
}
