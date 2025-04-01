using System;
using Magneto.Configuration;

namespace Magneto.Core;

/// <inheritdoc />
public class CacheKey : IKey
{
	string? _key;
	string? _prefix;
	object? _varyBy;

	internal static Func<string?, object?, string> CreateKey = CachedQuery.DefaultKeyCreator;

	/// <summary>
	/// Create a new instance of <see cref="CacheKey"/> with the given key prefix and an optional callback for creating the key.
	/// </summary>
	/// <param name="prefix">The prefix to use for generated keys.</param>
	public CacheKey(string? prefix) => Prefix = prefix;

	/// <summary>
	/// Configures this instance using the given <see cref="Action{T}"/>.
	/// </summary>
	/// <param name="configure">An action for configuring this instance.</param>
	/// <returns>This same instance, after being configured by <paramref name="configure"/>.</returns>
	public CacheKey Configure(Action<CacheKey> configure)
	{
		ArgumentNullException.ThrowIfNull(configure);
		configure(this);
		return this;
	}

	/// <summary>
	/// The key that is generated from a combination of the <see cref="Prefix"/> and <see cref="VaryBy"/>.
	/// </summary>
	public string Value => _key ??= CreateKey(Prefix, VaryBy);

	/// <inheritdoc />
	public string? Prefix
	{
		get => _prefix;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException($"{nameof(Prefix)} cannot be null or whitespace");
			_prefix = value;
			_key = null;
		}
	}

	/// <inheritdoc />
	public object? VaryBy
	{
		get => _varyBy;
		set
		{
			_varyBy = value;
			_key = null;
		}
	}
}
