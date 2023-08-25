using System.Text.Json.Serialization;
using VP.Selenium.Contracts.Models;

namespace VP.Selenium.Contracts
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(LogEntityMessageMessageModel))]
    [JsonSerializable(typeof(LogEntityMessageModel))]

    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
