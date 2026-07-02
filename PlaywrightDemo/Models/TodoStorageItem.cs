using System.Text.Json.Serialization;

namespace PlaywrightDemo.Models;

public sealed record TodoStorageItem(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("completed")] bool Completed);
