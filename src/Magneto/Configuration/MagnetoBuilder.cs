using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Magneto.Configuration
{
	/// <summary>
	/// A class for fluently configuring Magneto decorator, cache key creator, and cache stores.
	/// </summary>
	public class MagnetoBuilder
	{
		readonly IServiceCollection _services;

		/// <summary>
		/// Creates a new instance, wrapping the given <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
		public MagnetoBuilder(IServiceCollection services) => _services = services ?? throw new ArgumentNullException(nameof(services));

		/// <summary>
		/// Adds a singleton <see cref="IDecorator"/> of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of decorator to add.</typeparam>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithDecorator<T>() where T : class, IDecorator
		{
			_services.AddSingleton<IDecorator, T>();

			return this;
		}

		/// <summary>
		/// Configures the delegate Magneto uses for creating cache keys.
		/// </summary>
		/// <param name="createKey">The method for creating cache keys.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithCacheKeyCreator(Func<string, object, string> createKey)
		{
			CachedQuery.UseKeyCreator(createKey);

			return this;
		}

		/// <summary>
		/// Adds a singleton <see cref="ICacheStore{MemoryCacheEntryOptions}"/> of type <see cref="MemoryCacheStore"/>.
		/// </summary>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithMemoryCacheStore()
		{
			return WithCacheStore<MemoryCacheEntryOptions, MemoryCacheStore>();
		}


		/// <summary>
		/// Adds a singleton <see cref="ICacheStore{DistributedCacheEntryOptions}"/> of type
		/// <see cref="DistributedCacheStore"/>. Also adds a singleton <see cref="IStringSerializer"/>
		/// of type <see cref="SystemTextStringSerializer"/>, for use by the cache store.
		/// </summary>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithDistributedCacheStore()
		{
			return WithDistributedCacheStore<SystemTextStringSerializer>();
		}

		/// <summary>
		/// Adds a singleton <see cref="ICacheStore{DistributedCacheEntryOptions}"/> of type
		/// <see cref="DistributedCacheStore"/>. Also adds a singleton <see cref="IStringSerializer"/>
		/// of the specified type, for use by the cache store.
		/// </summary>
		/// <typeparam name="T">The type of string serializer to add.</typeparam>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithDistributedCacheStore<T>() where T : class, IStringSerializer
		{
			_services.AddSingleton<IStringSerializer, T>();
			return WithCacheStore<DistributedCacheEntryOptions, DistributedCacheStore>();
		}

		/// <summary>
		/// Adds a singleton <see cref="ICacheStore{DistributedCacheEntryOptions}"/> of type
		/// <see cref="DistributedCacheStore"/>. Also adds the given <paramref name="stringSerializer"/>
		/// as a singleton <see cref="IStringSerializer"/>, for use by the cache store.
		/// </summary>
		/// <param name="stringSerializer">The string serializer to use.</param>
		/// <typeparam name="T">The type of <paramref name="stringSerializer"/>.</typeparam>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithDistributedCacheStore<T>(T stringSerializer) where T : class, IStringSerializer
		{
			if (stringSerializer == null) throw new ArgumentNullException(nameof(stringSerializer));

			_services.AddSingleton<IStringSerializer>(stringSerializer);
			return WithCacheStore<DistributedCacheEntryOptions, DistributedCacheStore>();
		}

		/// <summary>
		/// Adds a singleton <see cref="ICacheStore{DistributedCacheEntryOptions}"/> of type
		/// <see cref="DistributedCacheStore"/>. Also adds a singleton <see cref="IStringSerializer"/>
		/// created by <paramref name="stringSerializerFactory"/>, for use by the cache store.
		/// </summary>
		/// <param name="stringSerializerFactory">The factory for creating the string serializer.</param>
		/// <typeparam name="T">The type of string serializer created by <paramref name="stringSerializerFactory"/>.</typeparam>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithDistributedCacheStore<T>(Func<IServiceProvider, T> stringSerializerFactory) where T : class, IStringSerializer
		{
			if (stringSerializerFactory == null) throw new ArgumentNullException(nameof(stringSerializerFactory));

			_services.AddSingleton<IStringSerializer>(stringSerializerFactory);
			return WithCacheStore<DistributedCacheEntryOptions, DistributedCacheStore>();
		}

		/// <summary>
		/// Adds a singleton <see cref="ICacheStore{TCacheEntryOptions}"/> of type <typeparamref name="TImplementation"/>.
		/// </summary>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options.</typeparam>
		/// <typeparam name="TImplementation">The type of the cache store.</typeparam>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithCacheStore<TCacheEntryOptions, TImplementation>() where TImplementation : class, ICacheStore<TCacheEntryOptions>
		{
			_services.AddSingleton<ICacheStore<TCacheEntryOptions>, TImplementation>();

			return this;
		}

		/// <summary>
		/// Adds the given <paramref name="cacheStore"/> as a singleton <see cref="ICacheStore{TCacheEntryOptions}"/>.
		/// </summary>
		/// <param name="cacheStore">The cache store to use.</param>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options.</typeparam>
		/// <typeparam name="TImplementation">The type of the cache store.</typeparam>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithCacheStore<TCacheEntryOptions, TImplementation>(TImplementation cacheStore)
			where TImplementation : class, ICacheStore<TCacheEntryOptions>
		{
			if (cacheStore == null) throw new ArgumentNullException(nameof(cacheStore));

			_services.AddSingleton<ICacheStore<TCacheEntryOptions>>(cacheStore);

			return this;
		}

		/// <summary>
		/// Adds a singleton <see cref="ICacheStore{TCacheEntryOptions}"/> created by <paramref name="cacheStoreFactory"/>.
		/// </summary>
		/// <param name="cacheStoreFactory">The factory for creating the cache store.</param>
		/// <typeparam name="TCacheEntryOptions">The type of cache entry options.</typeparam>
		/// <typeparam name="TImplementation">The type of the cache store created by <paramref name="cacheStoreFactory"/>.</typeparam>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public MagnetoBuilder WithCacheStore<TCacheEntryOptions, TImplementation>(Func<IServiceProvider, TImplementation> cacheStoreFactory)
			where TImplementation : class, ICacheStore<TCacheEntryOptions>
		{
			if (cacheStoreFactory == null) throw new ArgumentNullException(nameof(cacheStoreFactory));

			_services.AddSingleton<ICacheStore<TCacheEntryOptions>>(cacheStoreFactory);

			return this;
		}
	}
}
