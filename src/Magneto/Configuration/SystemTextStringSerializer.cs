using System.Text.Json;

namespace Magneto.Configuration
{
	/// <summary>
	/// An implementation of <see cref="IStringSerializer"/> backed by <see cref="JsonSerializer"/>.
	/// </summary>
	public class SystemTextStringSerializer : IStringSerializer
	{
		JsonSerializerOptions JsonSerializerOptions { get; }

		/// <summary>
		/// Creates a new instance of <see cref="SystemTextStringSerializer"/>.
		/// </summary>
		/// <param name="options">Optional settings to use when serializing/deserializing objects.</param>
		public SystemTextStringSerializer(JsonSerializerOptions options = null) =>
			JsonSerializerOptions = options ?? new JsonSerializerOptions();

		/// <inheritdoc cref="IStringSerializer.Serialize"/>
		public string Serialize(object value) => JsonSerializer.Serialize(value, JsonSerializerOptions);

		/// <inheritdoc cref="IStringSerializer.Deserialize{T}"/>
		public T Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, JsonSerializerOptions);
	}
}
