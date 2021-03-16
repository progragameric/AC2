using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;


namespace AC2
{
    public class ScannerNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer inner;
        private readonly IObjectFactory objectFactory;
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
                value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetRef.path);
                return true;
            }

            if (typeof(Ref) == expectedType)
            {
                if (!inner.Deserialize(parser, typeof(Ref), nestedObjectDeserializer, out value))
                    return false;
                value = null;
                return true;
            }

            if (expectedType.Namespace == "AC2")
            {
                if (!inner.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
                    return false;

                if (value.GetType().GetProperty("name") != null)
                {
                  
                    var key = value.GetType().GetProperty("name").GetValue(value);
                    
                    if ((string)key != "" && key != null && !CreatorNodeDeserializer.refs.ContainsKey(key))
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