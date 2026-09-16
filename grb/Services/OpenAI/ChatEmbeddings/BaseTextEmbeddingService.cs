using System.ClientModel;
using OpenAI.Embeddings;

namespace grb.Services.OpenAI.ChatEmbeddings;

public abstract class BaseTextEmbeddingService(EmbeddingClient embeddingClient) : ITextEmbeddingService
{
    protected readonly EmbeddingClient _embeddingClient = embeddingClient;

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
