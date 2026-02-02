using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyDoc.Infrastructure.Data.DataSeed;

internal class JsonDataLoader
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private static readonly Assembly _assembly = typeof(DataSeeder).Assembly;

    private static readonly string _rootNameSpace = typeof(DataSeeder).Assembly.GetName().Name ?? "";
    public async static Task<List<T>> LoadJsonAsync<T>(string fileName)
    {
        var resourceName = $"{_rootNameSpace}.Data.DataSeed.{fileName}";

        await using var stream = _assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

        return JsonSerializer.Deserialize<List<T>>(stream, _jsonOptions) ?? new();
    }
}
