using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Mergen.Core.QueryProcessing
{
    public class PropertyCache
    {
        private readonly ConcurrentDictionary<Type, PropertyInfo[]> _typeProperties;

        public PropertyCache()
        {
            _typeProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();
        }

        public PropertyInfo[] GetProperties<T>()
        {
            return GetProperties(typeof(T));
        }

        public PropertyInfo[] GetProperties(Type type)
        {
            if (!_typeProperties.TryGetValue(type, out var propertyInfos))
            {
                propertyInfos = type.GetProperties();
                _typeProperties.TryAdd(type, propertyInfos);
            }

            return propertyInfos;
        }
    }
}