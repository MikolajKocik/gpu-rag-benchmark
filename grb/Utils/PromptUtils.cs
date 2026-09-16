using Microsoft.SemanticKernel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace grb.Utils;

public static class PromptUtils
{
    public static string LoadPrompt(int? version = 1)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Prompts", $"AnswerFromDocuments.{version}.yaml");
        string yaml = File.ReadAllText(path);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var config = deserializer.Deserialize<PromptTemplateConfig>(yaml);
        return config.Template;
    }
}
