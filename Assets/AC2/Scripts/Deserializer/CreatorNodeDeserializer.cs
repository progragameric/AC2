using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace AC2 {
  public class CreatorNodeDeserializer: INodeDeserializer {
    private readonly INodeDeserializer inner;
    private readonly IObjectFactory objectFactory;
    public static Dictionary <object, object> refs = new Dictionary <object, object> ();
    public CreatorNodeDeserializer(INodeDeserializer inner, IObjectFactory objectFactory) {
      this.objectFactory = objectFactory;
      this.inner = inner;
    }
    public bool Deserialize(IParser parser, Type expectedType, Func <IParser, Type, object> nestedObjectDeserializer, out object value) {
      if (typeof (AssetRef) == expectedType) {
        if (!inner.Deserialize(parser, typeof (AssetRef), nestedObjectDeserializer, out value))
          return false;

        var assetRef = ((AssetRef) value);
        value = AssetDatabase.LoadAssetAtPath <UnityEngine.Object> (assetRef.path);
        return true;
      }

      if (typeof (Ref) == expectedType) {
        if (!inner.Deserialize(parser, typeof (Ref), nestedObjectDeserializer, out value))
          return false;

        var refVal = ((Ref) value);
        if (refs.TryGetValue(refVal.key, out object obj)) {
          value = obj;
          return true;
        } else {
          value = null;
          if (value == null) {
            Debug.Log("[AC2] Failed to find reference: " + refVal.key);
          }
          return false;
        }
      }
      
      return inner.Deserialize(parser, expectedType, nestedObjectDeserializer, out value);
    }
  }
}