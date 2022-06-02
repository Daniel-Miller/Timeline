using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using Timeline.Utilities;

namespace Sample.Persistence
{
    public class Serializer
    {
        static readonly Dictionary<string, DynamicContractResolver> ContractResolvers = new Dictionary<string, DynamicContractResolver>();

        public DynamicContractResolver GetContractResolver(string exclusions)
        {
            lock (this)
            {
                if (!ContractResolvers.ContainsKey(exclusions))
                    ContractResolvers.Add(exclusions, new DynamicContractResolver(exclusions.Split(new char[] { ',' })));
            }

            return ContractResolvers[exclusions];
        }

        public T Deserialize<T>(string value, Type type)
        {
            return (T)JsonConvert.DeserializeObject(value, type);
        }

        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Exclude the aggregate identifier/version and the event time/user from the serialized event. These
        /// properties are stored in their own discrete columns in the Command table, so we don't need them
        /// duplicated in the CommandData column.
        /// </summary>
        public string Serialize(object command, string[] exclusions)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = GetContractResolver(string.Join(",", exclusions)),
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(command, settings);
        }
        
        /// <summary>
        /// Returns the assembly-qualified class name without the version, culture, and public key token.
        /// </summary>
        public string GetClassName(Type type)
        {
            return $"{type.FullName}, {Assembly.GetAssembly(type).GetName().Name}";
        }
    }

    public sealed class DynamicContractResolver : DefaultContractResolver
    {
        private readonly string[] _exclusions;

        public DynamicContractResolver(string[] exclusions)
        {
            _exclusions = exclusions;
        }

        /// <summary>
        /// Exclude properties that we don't want in the serialized JSON output, and sort properties
        /// alphabetically.
        /// </summary>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            properties = properties
                .Where(p => !_exclusions.Contains(p.PropertyName))
                .OrderBy(p => p.PropertyName)
                .ToList();

            return properties;
        }
    }
}
