using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Quarks.IDictionaryExtensions;

namespace Data.Operations
{
	public static class DataFactory
	{
		static readonly IDictionary<Type, IDataQueryCache> _dataQueryCaches = new Dictionary<Type, IDataQueryCache>();
		public static void SetDataQueryCache<TContext>(IDataQueryCache dataQueryCache)
		{
			_dataQueryCaches.AddOrUpdate(typeof(TContext), dataQueryCache);
		}

		static IDataQueryCache _dataQueryCache = new DataQueryCache(new MemoryCacheDefaultCacheStore());
		public static void SetDataQueryCache(IDataQueryCache dataQueryCache)
		{
			_dataQueryCache = dataQueryCache;
		}

		static IData _createData<TContext>()
		{
			return new Data(_dataQueryCaches.GetValueOrDefault(typeof(TContext), _dataQueryCache));
		}

		static readonly IDictionary<Type, Func<object, object>> _dataFactories = new Dictionary<Type, Func<object, object>>();
		public static void SetFactory<TContext>(Func<TContext, IData<TContext>> dataFactory)
		{
			_dataFactories.AddOrUpdate(typeof(TContext), x => dataFactory((TContext)x));
		}

		public static IData<TContext> Data<TContext>(this TContext context)
		{
			return _dataFactories
				.GetValueOrDefault(typeof(TContext), _createData<TContext>)
				.Invoke(context) as IData<TContext>;
		}

		static readonly IDictionary<Type, Func<IData, object, object>> _dataConstructorDelegates = new Dictionary<Type, Func<IData, object, object>>();
		static object _createData<TContext>(object context)
		{
			return _dataConstructorDelegates
				.GetValueOrDefault(context.GetType(), () => _createDataConstructorDelegate(context.GetType()))
				.Invoke(_createData<TContext>(), context);
		}

		static Func<IData, object, object> _createDataConstructorDelegate(Type contexType)
		{
			var dataType = typeof(Data<>).MakeGenericType(contexType);
			var constructorInfo = dataType.GetConstructors()[0];
			var method = new DynamicMethod("", typeof(object), new[] { typeof(IData), typeof(object) }, true);
			var gen = method.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_1); // Load second argument
			var variable = gen.DeclareLocal(contexType); // Declare local variable
			gen.Emit(contexType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, contexType); // Cast or unbox second argument
			gen.Emit(OpCodes.Stloc, variable); // Assign result of cast or unbox to local variable
			gen.Emit(OpCodes.Ldarg_0); // Load first argument
			gen.Emit(OpCodes.Ldloc_0); // Load local variable (the second argument casted or unboxed to contextType)
			gen.Emit(OpCodes.Newobj, constructorInfo); // Call constructor
			gen.Emit(OpCodes.Ret); // Return
			return (Func<IData, object, object>)method.CreateDelegate(typeof(Func<IData, object, object>));
		}
	}
}