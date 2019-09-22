using Newtonsoft.Json;

namespace Magneto.Microsoft
{
	/// <summary>
	/// An implementation of <see cref="IStringSerializer"/> backed by <see cref="JsonConvert"/>.
	/// </summary>
	public class JsonConvertStringSerializer : IStringSerializer
	{
		readonly JsonSerializerSettings _settings;

		/// <summary>
		/// Creates a new instance of <see cref="JsonConvertStringSerializer"/>.
		/// </summary>
		/// <param name="settings">Optional settings to use when serializing/deserializing objects.</param>
		public JsonConvertStringSerializer(JsonSerializerSettings settings = null) => _settings = settings;

		/// <inheritdoc cref="IStringSerializer.Serialize"/>
		public string Serialize(object value) => JsonConvert.SerializeObject(value, _settings);

		/// <inheritdoc cref="IStringSerializer.Deserialize{T}"/>
		public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, _settings);
	}
}
