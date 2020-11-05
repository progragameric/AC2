using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;


namespace AC2
{
    public class ScannerNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer inner;
        private readonly IObjectFactory objectFactory;
        // public static readonly Dictionary<object, object> refs = new Dictionary<object, object>();
        public ScannerNodeDeserializer(INodeDeserializer inner, IObjectFactory objectFactory)
        {
            this.objectFactory = objectFactory;
            this.inner = inner;
        }
        public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (typeof(AssetRef) == expectedType)
            {
                if (!inner.Deserialize(parser, typeof(AssetRef), nestedObjectDeserializer, out value))
                    return false;
                var assetRef = ((AssetRef)value);
                value = Resources.Load(assetRef.path);
                return true;
            }

            if (typeof(Ref) == expectedType)
            {
                if (!inner.Deserialize(parser, typeof(Ref), nestedObjectDeserializer, out value))
                    return false;
                value = null;
                return true;
            }

            if (expectedType.Namespace == "UnityEditor.Animations")
            {
                if (!inner.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
                    return false;
                if (value.GetType().GetProperty("name") != null)
                {
                    var key = value.GetType().GetProperty("name").GetValue(value);
                    if ((string)key != "")
                    {
                        CreatorNodeDeserializer.refs.Add(key, value);
                    }
                }
                return true;
            }

            return inner.Deserialize(parser, expectedType, nestedObjectDeserializer, out value);
        }
    }
}