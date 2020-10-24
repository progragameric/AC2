using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.ObjectFactories;

namespace AC2
{
    public class Creator : MonoBehaviour
    {
        public TextAsset inputTemplate;
        // Start is called before the first frame update
        void Start()
        {
            var template = CreateController(inputTemplate.text);
            GeneratePostAnimation(template);
            EditAnimationClip(template);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private Template CreateController(string yml)
        {

            var deserializer = new DeserializerBuilder()
                       .WithTagMapping("!AnimationClip", typeof(AnimationClip))
                       .WithTagMapping("!BlendTree", typeof(BlendTree))
                       .WithTagMapping("!AssetRef", typeof(AssetRef))
                       .WithTagMapping("!Ref", typeof(Ref))
                   .WithNodeDeserializer(inner => new AssetRefNodeDeserializer(inner, new DefaultObjectFactory()), s => s.InsteadOf<ObjectNodeDeserializer>())
                   .Build();
            Template template = deserializer.Deserialize<Template>(yml);
            AssetDatabase.CreateAsset(template.controller, $"{template.saveTo}/{template.name}.controller");

            return template;
        }

        private void GeneratePostAnimation(Template t)
        {
            if (t.clips == null) return;
            foreach (AC2AnimationClip clip in t.clips)
            {
                var basename = Path.GetFileNameWithoutExtension(clip.path);
                var path = basename.Contains("Post") ? clip.path : clip.path.Replace(".anim", "Post.anim");
                AnimationClip postC = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (postC == null)
                {
                    if (!AssetDatabase.CopyAsset(clip.path, path))
                    {
                        throw new Exception($"failed to copy asset to {path}");
                    }
                }
                postC = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                postC.name = basename;
                if (basename.Contains("Post"))
                {

                    foreach (var binding in AnimationUtility.GetCurveBindings(postC))
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(postC, binding);
                        Keyframe[] keys = curve.keys;
                        var lastFrame = keys[keys.Length - 1];
                        AnimationCurve postCurve = new AnimationCurve();
                        Keyframe[] postKeys = new Keyframe[] { lastFrame };

                        postCurve.keys = postKeys;
                        AnimationUtility.SetEditorCurve(postC, binding, postCurve);
                    }
                    var _path = $"{Application.dataPath}/{path}".Replace("/Assets/Assets/Resources/", "/Assets/Resources/");
                    var before = File.ReadAllText(_path);
                    var after = Regex.Replace(before, @"m_StopTime: ([\.0-9]*)", $"m_StopTime: {clip.length}");

                    File.WriteAllText(_path, after);

                }
            }
        }

        private void EditAnimationClip(Template t)
        {
            if (t.clips == null) return;
            foreach (AC2AnimationClip clip in t.clips)
            {
                AnimationClip c = AssetDatabase.LoadAssetAtPath<AnimationClip>(clip.path);
                if (c != null)
                {
                    List<AnimationEvent> evts = new List<AnimationEvent>();
                    foreach (AC2AnimationEvent evt in clip.events)
                    {
                        AnimationEvent e = new AnimationEvent();
                        e.floatParameter = evt.floatParameter;
                        e.functionName = evt.functionName;
                        e.intParameter = evt.intParameter;
                        var objRef = Resources.Load(evt.objectReferenceParameter) as UnityEngine.Object;
                        e.objectReferenceParameter = objRef;
                        e.stringParameter = evt.stringParameter;
                        DataTable dt = new DataTable();
                        var expr = evt.time.Replace("length", "" + c.length);
                        object time = dt.Compute(expr, "");
                        var timeVal = float.Parse(time.ToString());
                        e.time = timeVal;

                        evts.Add(e);
                    }
                    AnimationUtility.SetAnimationEvents(c, evts.ToArray());
                }
                else
                {
                    throw new Exception($"clip not found {clip.path}");
                }
            }
        }
    }
}