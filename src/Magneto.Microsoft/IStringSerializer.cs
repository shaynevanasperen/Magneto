namespace Magneto.Microsoft
{
	/// <summary>
	/// An abstraction for serializing and deserializing objects to and from strings.
	/// </summary>
	public interface IStringSerializer
	{
		/// <summary>
		/// Serializes the given <paramref name="value"/> to a string.
		/// </summary>
		/// <param name="value">The value to be serialized.</param>
		/// <returns>The string representation of the given value.</returns>
		string Serialize(object value);

		/// <summary>
		/// Deserializes the given string as an instance of <typeparamref name="T"/>.
		/// </summary>
		/// <param name="value">The string representation for an object of type <typeparamref name="T"/>.</param>
		/// <typeparam name="T">The type of object to materialize.</typeparam>
		/// <returns>The deserialized object.</returns>
		T Deserialize<T>(string value);
	}
}
