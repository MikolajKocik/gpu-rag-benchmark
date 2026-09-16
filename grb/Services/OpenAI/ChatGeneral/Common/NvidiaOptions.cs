namespace grb.Services.OpenAI.ChatGeneral.Common;

public class NvidiaOptions
{
    public string BaseUrl { get; set; } = "https://integrate.api.nvidia.com/v1/";
    public string ApiKey { get; set; }
    public string Embedding { get; set; } = "Nemotron-3-Embed-1B";
    public string Ranker { get; set; } = "llama-nemotron-rerank-vl-1b-v2";
}
