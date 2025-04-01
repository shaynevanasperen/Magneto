using System.Text.Json;

namespace Magneto.Configuration;

/// <summary>
/// An implementation of <see cref="IStringSerializer"/> backed by <see cref="JsonSerializer"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="SystemTextStringSerializer"/>.
/// </remarks>
/// <param name="options">Optional settings to use when serializing/deserializing objects.</param>
public class SystemTextStringSerializer(JsonSerializerOptions? options = null) : IStringSerializer
{
	JsonSerializerOptions JsonSerializerOptions { get; } = options ?? new JsonSerializerOptions();

	/// <inheritdoc cref="IStringSerializer.Serialize"/>
	public string Serialize(object value) => JsonSerializer.Serialize(value, JsonSerializerOptions);

	/// <inheritdoc cref="IStringSerializer.Deserialize{T}"/>
	public T Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, JsonSerializerOptions)!;
}
