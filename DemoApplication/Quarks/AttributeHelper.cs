using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Quarks
{
	/// <summary>
	/// Provides easy methods for obtaining attributes, and improves performance by keeping retrieved
	/// attributes in a dictionary cache.
	/// <see cref="http://lotsacode.wordpress.com/2011/04/27/c-reflectionhelper"/>
	/// </summary>
	static class AttributeHelper
	{
		static readonly Dictionary<object, List<Attribute>> _attributeCache = new Dictionary<object, List<Attribute>>();

		internal static Dictionary<object, List<Attribute>> AttributeCache
		{
			get { return _attributeCache; }
		}

		internal static List<Attribute> GetTypeAttributes(Type type)
		{
			return lockAndGetAttributes(type, tp => ((Type)tp).GetCustomAttributes(true));
		}

		internal static List<Attribute> GetTypeAttributes<TType>()
		{
			return GetTypeAttributes(typeof(TType));
		}

		internal static List<TAttributeType> GetTypeAttributes<TType, TAttributeType>(Func<TAttributeType, bool> predicate = null)
		{
			return GetTypeAttributes<TType>().where<Attribute, TAttributeType>().Where(attr => predicate == null || predicate(attr)).ToList();
		}

		internal static TAttributeType GetTypeAttribute<TType, TAttributeType>(Func<TAttributeType, bool> predicate = null)
		{
			return GetTypeAttributes<TType, TAttributeType>(predicate).FirstOrDefault();
		}

		internal static TAttributeType GetTypeAttribute<TAttributeType>(Type type)
		{
			return GetTypeAttributes(type).where<Attribute, TAttributeType>().FirstOrDefault();
		}

		internal static bool HasTypeAttribute<TType, TAttributeType>(Func<TAttributeType, bool> predicate = null)
		{
			return GetTypeAttribute<TType, TAttributeType>(predicate) != null;
		}

		internal static bool HasTypeAttribute<TAttributeType>(Type type)
		{
			return GetTypeAttribute<TAttributeType>(type) != null;
		}

		internal static List<Attribute> GetMemberAttributes<TType>(Expression<Func<TType, object>> action)
		{
			return GetMemberAttributes(getMember(action));
		}

		internal static List<TAttributeType> GetMemberAttributes<TType, TAttributeType>(Expression<Func<TType, object>> action, Func<TAttributeType, bool> predicate = null)
			where TAttributeType : Attribute
		{
			return GetMemberAttributes(getMember(action), predicate);
		}

		internal static TAttributeType GetMemberAttribute<TType, TAttributeType>(Expression<Func<TType, object>> action, Func<TAttributeType, bool> predicate = null)
			where TAttributeType : Attribute
		{
			return GetMemberAttribute(getMember(action), predicate);
		}

		internal static bool HasMemberAttribute<TType, TAttributeType>(Expression<Func<TType, object>> action, Func<TAttributeType, bool> predicate = null)
			where TAttributeType : Attribute
		{
			return GetMemberAttribute(getMember(action), predicate) != null;
		}

		internal static List<Attribute> GetMemberAttributes(this MemberInfo memberInfo)
		{
			return lockAndGetAttributes(memberInfo, mi => ((MemberInfo)mi).GetCustomAttributes(true));
		}

		internal static List<TAttributeType> GetMemberAttributes<TAttributeType>(this MemberInfo memberInfo, Func<TAttributeType, bool> predicate = null)
			where TAttributeType : Attribute
		{
			return GetMemberAttributes(memberInfo).where<Attribute, TAttributeType>().Where(attr => predicate == null || predicate(attr)).ToList();
		}

		internal static TAttributeType GetMemberAttribute<TAttributeType>(this MemberInfo memberInfo, Func<TAttributeType, bool> predicate = null)
			where TAttributeType : Attribute
		{
			return GetMemberAttributes(memberInfo, predicate).FirstOrDefault();
		}

		internal static bool HasMemberAttribute<TAttributeType>(this MemberInfo memberInfo, Func<TAttributeType, bool> predicate = null)
			where TAttributeType : Attribute
		{
			return memberInfo.GetMemberAttribute(predicate) != null;
		}

		static IEnumerable<TType> where<X, TType>(this IEnumerable<X> list)
		{
			return list.Where(item => item is TType).Cast<TType>();
		}

		static List<Attribute> lockAndGetAttributes(object key, Func<object, object[]> retrieveValue)
		{
			return lockAndGet(_attributeCache, key, mi => retrieveValue(mi).Cast<Attribute>().ToList());
		}

		static TValue lockAndGet<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> retrieveValue)
		{
			var value = default(TValue);
			lock (dictionary)
				if (dictionary.TryGetValue(key, out value))
					return value;

			value = retrieveValue(key);

			lock (dictionary)
				if (dictionary.ContainsKey(key) == false)
					dictionary.Add(key, value);
				return value;
		}

		static MemberInfo getMember<T>(Expression<Func<T, object>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression != null)
				return memberExpression.Member;

			var unaryExpression = expression.Body as UnaryExpression;
			if (unaryExpression != null)
			{
				memberExpression = unaryExpression.Operand as MemberExpression;
				if (memberExpression != null)
					return memberExpression.Member;

				var methodCall = unaryExpression.Operand as MethodCallExpression;
				if (methodCall != null)
					return methodCall.Method;
			}

			return null;
		}
	}
}