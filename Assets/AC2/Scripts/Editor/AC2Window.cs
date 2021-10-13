using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Data;

using System.Text.RegularExpressions;

using UnityEditor.Animations;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.ObjectFactories;
using Newtonsoft.Json;

namespace AC2 {
  public class AC2Window: EditorWindow {
    private DefaultAsset templateDir = null;
    public Dictionary <string, UnityEditor.Animations.AnimatorStateMachine>stateMachines = new Dictionary <string, UnityEditor.Animations.AnimatorStateMachine>();
    public Dictionary <string, UnityEditor.Animations.AnimatorState>states = new Dictionary <string, UnityEditor.Animations.AnimatorState>();

    [MenuItem("Window/AC2")]
    static void Init() {
      AC2Window window = (AC2Window) EditorWindow.GetWindow(typeof (AC2Window), true, "AC2");
      window.Show();
    }

    void OnEnable() {
      templateDir = AssetDatabase.LoadAssetAtPath <DefaultAsset>(
        EditorPrefs.GetString("AC2.templateDir"));
    }

    void OnGUI() {
      templateDir = (DefaultAsset) EditorGUILayout.ObjectField(
        "Select Folder",
        templateDir,
        typeof (DefaultAsset),
        false);

      if (GUILayout.Button(new GUIContent("Generate"), new GUILayoutOption[] {
          GUILayout.ExpandWidth(true)
        })) {
        Start();
      }

      if (templateDir != null) {
        EditorGUILayout.HelpBox(
          "The template directory path: " + AssetDatabase.GetAssetPath(templateDir),
          MessageType.Info,
          true);
        EditorPrefs.SetString("AC2.templateDir", AssetDatabase.GetAssetPath(templateDir));
      } else {
        EditorGUILayout.HelpBox(
          "Please choose the template directory",
          MessageType.Warning,
          true);
      }
    }
    void Start() {

      string[] guids = AssetDatabase.FindAssets("", new [] {
        AssetDatabase.GetAssetPath(templateDir)
      });

      foreach(string guid in guids) {
        stateMachines = new Dictionary <string, UnityEditor.Animations.AnimatorStateMachine>();
        states = new Dictionary <string, UnityEditor.Animations.AnimatorState>();
        TextAsset inputTemplate = AssetDatabase.LoadAssetAtPath <TextAsset>(AssetDatabase.GUIDToAssetPath(guid));

        var template = ScanTemplate(inputTemplate.text);

        GeneratePostAnimation(template);
        EditAnimationClip(template);
        template = ScanTemplate(inputTemplate.text);
        CreateController(template);
        stateMachines.Clear();
        states.Clear();
        Debug.Log("[AC2] Finish generating: " + AssetDatabase.GUIDToAssetPath(guid));
      }

    }

    private Template ScanTemplate(string yml) {
      var scannerBuilder = new DeserializerBuilder()
        .WithTagMapping("!AnimationClip", typeof (UnityEngine.AnimationClip))
        .WithTagMapping("!BlendTree", typeof (UnityEditor.Animations.BlendTree))
        .WithTagMapping("!AssetRef", typeof (AssetRef))
        .WithTagMapping("!Ref", typeof (Ref));

      scannerBuilder.
      WithNodeDeserializer(inner => new ScannerNodeDeserializer(inner, new DefaultObjectFactory()), s => s.InsteadOf <ObjectNodeDeserializer>())
        .Build().Deserialize <Template>(yml);

      var builderBuilder = new DeserializerBuilder()
        .WithTagMapping("!AnimationClip", typeof (UnityEngine.AnimationClip))
        .WithTagMapping("!BlendTree", typeof (UnityEditor.Animations.BlendTree))
        .WithTagMapping("!AssetRef", typeof (AssetRef))
        .WithTagMapping("!Ref", typeof (Ref));

      Template template = builderBuilder
        .WithNodeDeserializer(inner => new CreatorNodeDeserializer(inner, new DefaultObjectFactory()), s => s.InsteadOf <ObjectNodeDeserializer>())
        .Build().Deserialize <Template>(yml);

      
      return template;
    }

    public void CreateController(Template template) {
      var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{template.saveTo}/{template.name}.controller");
      if (template.controller.parameters != null) {
        foreach(AnimatorControllerParameter parameter in template.controller.parameters) {
          controller.AddParameter(parameter.name, parameter.type);
        }
      }
      controller.RemoveLayer(0);

      for (int i = 0; i <template.controller.layers.Length; i++) {
        AnimatorControllerLayer layer = template.controller.layers[i];
        controller.AddLayer(layer.name);

        UnityEditor.Animations.AnimatorControllerLayer cLayer = controller.layers[i];
        MappingUtility.MapAnimatorControllerLayer(layer, cLayer);

        var rootStateMachine = layer.stateMachine;
        var cRootStateMachine = controller.layers[i].stateMachine;
        WalkStateMachineFirst(rootStateMachine, cRootStateMachine);
      }

      for (int i = 0; i <template.controller.layers.Length; i++) {
        AnimatorControllerLayer layer = template.controller.layers[i];
        var rootStateMachine = layer.stateMachine;
        var cRootStateMachine = controller.layers[i].stateMachine;
        WalkStateMachine(controller, rootStateMachine, cRootStateMachine, null);
      }
    }

    public void WalkStateMachineFirst(AnimatorStateMachine fromRootStateMachine, UnityEditor.Animations.AnimatorStateMachine toRootStateMachine) {
      if (fromRootStateMachine.stateMachines != null) {
        foreach(ChildAnimatorStateMachine fromChildStateMachine in fromRootStateMachine.stateMachines) {
          AnimatorStateMachine fromStateMachine = fromChildStateMachine.stateMachine;

          var toStateMachine = toRootStateMachine.AddStateMachine(fromStateMachine.name);

          stateMachines.Add(toStateMachine.name, toStateMachine);

          if (fromStateMachine.states != null) {
            foreach(ChildAnimatorState fromChildState in fromStateMachine.states) {
              AnimatorState fromState = fromChildState.state;
              UnityEditor.Animations.AnimatorState toState = toStateMachine.AddState(fromState.name);

              states.Add(toState.name, toState);
            }
          }
          WalkStateMachineFirst(fromStateMachine, toStateMachine);
        }
      }
    }

    public void WalkStateMachine(UnityEditor.Animations.AnimatorController controller, AnimatorStateMachine fromRootStateMachine, UnityEditor.Animations.AnimatorStateMachine toRootStateMachine, UnityEditor.Animations.AnimatorStateMachine parentStateMachine) {

      // anyStatePosition
      toRootStateMachine.anyStatePosition = fromRootStateMachine.anyStatePosition;
      // anyStateTransitions
      if (fromRootStateMachine.anyStateTransitions != null) {
        foreach(AnimatorStateTransition fromAnyStateTransition in fromRootStateMachine.anyStateTransitions) {
          AnimatorStateMachine destinationStateMachine = fromAnyStateTransition.destinationStateMachine;
          AnimatorState destinationState = fromAnyStateTransition.destinationState;
          if (destinationStateMachine != null) {
            {
              if (stateMachines.TryGetValue(destinationStateMachine.name, out UnityEditor.Animations.AnimatorStateMachine stateMachine)) {
                var toAnyStateTransition = toRootStateMachine.AddAnyStateTransition(stateMachine);
                MappingUtility.MapAnimatorStateTransition(fromAnyStateTransition, toAnyStateTransition);
              }
            }
          }
          if (destinationState != null) {
            {
              if (states.TryGetValue(destinationState.name, out UnityEditor.Animations.AnimatorState state)) {
                var toAnyStateTransition = toRootStateMachine.AddAnyStateTransition(state);
                MappingUtility.MapAnimatorStateTransition(fromAnyStateTransition, toAnyStateTransition);
              }
            }
          }
        }
      }
      if (fromRootStateMachine.behaviours != null) {
        // behavaviours
        foreach(StateMachineBehaviour fromBehaviour in fromRootStateMachine.behaviours) {
          toRootStateMachine.AddStateMachineBehaviour(fromBehaviour.GetType());
        }
      }
      //defaultState
      if (fromRootStateMachine.defaultState != null) {
        if (states.TryGetValue(fromRootStateMachine.defaultState.name, out UnityEditor.Animations.AnimatorState state)) {
          toRootStateMachine.defaultState = state;
        }
      }
      // entryPosition
      toRootStateMachine.entryPosition = fromRootStateMachine.entryPosition;
      // entryTransitions
      if (fromRootStateMachine.entryTransitions != null) {
        foreach(AnimatorTransition fromEntryTransition in fromRootStateMachine.entryTransitions) {
          AnimatorStateMachine destinationStateMachine = fromEntryTransition.destinationStateMachine;
          AnimatorState destinationState = fromEntryTransition.destinationState;
          if (destinationStateMachine != null) {
            if (stateMachines.TryGetValue(destinationStateMachine.name, out UnityEditor.Animations.AnimatorStateMachine stateMachine)) {
              var toEntryTransition = toRootStateMachine.AddEntryTransition(stateMachine);
              MappingUtility.MapAnimatorTransition(fromEntryTransition, toEntryTransition);

              foreach(AnimatorCondition fromCondition in fromEntryTransition.conditions) {
                toEntryTransition.AddCondition(fromCondition.mode, fromCondition.threshold, fromCondition.parameter);
              }
            }
          }

          if (destinationState != null) {
            {
              if (states.TryGetValue(destinationState.name, out UnityEditor.Animations.AnimatorState state)) {
                var toEntryTransition = toRootStateMachine.AddEntryTransition(state);
                MappingUtility.MapAnimatorTransition(fromEntryTransition, toEntryTransition);
                if (fromEntryTransition.conditions != null) {
                  foreach(AnimatorCondition fromCondition in fromEntryTransition.conditions) {
                    toEntryTransition.AddCondition(fromCondition.mode, fromCondition.threshold, fromCondition.parameter);
                  }
                }
              }
            }
          }
        }
      }
      // exitPosition
      toRootStateMachine.exitPosition = fromRootStateMachine.exitPosition;
      // parentStateMachinePosition
      toRootStateMachine.parentStateMachinePosition = fromRootStateMachine.parentStateMachinePosition;
      // transitions

      if (parentStateMachine != null && fromRootStateMachine.transitions != null) {

        foreach(AnimatorTransition fromTransition in fromRootStateMachine.transitions) {

          AnimatorStateMachine fromDestinationStateMachine = fromTransition.destinationStateMachine;

          AnimatorState fromDestinationState = fromTransition.destinationState;

          if (
            fromDestinationStateMachine != null) {

            if (stateMachines.TryGetValue(fromDestinationStateMachine.name, out UnityEditor.Animations.AnimatorStateMachine destinationStateMachine)) {

              var toTransition = parentStateMachine.AddStateMachineTransition(toRootStateMachine, destinationStateMachine);

              MappingUtility.MapAnimatorTransition(fromTransition, toTransition);

            }
          }
          if (fromDestinationState != null) {

            if (states.TryGetValue(fromDestinationState.name, out UnityEditor.Animations.AnimatorState destinationState)) {
              var toTransition = parentStateMachine.AddStateMachineTransition(toRootStateMachine, destinationState);
              MappingUtility.MapAnimatorTransition(fromTransition, toTransition);
              if (fromTransition.conditions != null) {
                foreach(AnimatorCondition fromCondition in fromTransition.conditions) {
                  toTransition.AddCondition(fromCondition.mode, fromCondition.threshold, fromCondition.parameter);
                }
              }

            }

          }
        }
      }
      // states
      if (fromRootStateMachine.states != null) {
        foreach(ChildAnimatorState childState in fromRootStateMachine.states) {
          AnimatorState fromState = childState.state;
    
          if (states.TryGetValue(fromState.name, out UnityEditor.Animations.AnimatorState toState)) {
            MappingUtility.MapAnimatorState(fromState, toState);
            if(toState.motion is UnityEditor.Animations.BlendTree) {
              AssetDatabase.AddObjectToAsset(toState.motion, controller);
            }
            if (fromState.transitions != null) {
              // transitions
              foreach(AnimatorStateTransition fromStateTransition in fromState.transitions) {
                AnimatorStateMachine destinationStateMachine = fromStateTransition.destinationStateMachine;
                AnimatorState destinationState = fromStateTransition.destinationState;
                if (destinationStateMachine != null) {
                  {
                    if (stateMachines.TryGetValue(destinationStateMachine.name, out UnityEditor.Animations.AnimatorStateMachine stateMachine)) {
                      var toStateTransition = toState.AddTransition(stateMachine);
                      MappingUtility.MapAnimatorStateTransition(fromStateTransition, toStateTransition);

                    }
                  }
                } {
                  if (destinationState != null) {
                    {
                      if (states.TryGetValue(destinationState.name, out UnityEditor.Animations.AnimatorState state)) {
                        var toStateTransition = toState.AddTransition(state);
                        MappingUtility.MapAnimatorStateTransition(fromStateTransition, toStateTransition);

                      }
                    }
                  }
                }
                if (fromStateTransition.isExit) {
                  var toExitTransition = toState.AddExitTransition();
                  MappingUtility.MapAnimatorStateTransition(fromStateTransition, toExitTransition);

                }
              }
            }
          }
        }
        // stateMachines
      }
      if (fromRootStateMachine.stateMachines != null) {
        foreach(ChildAnimatorStateMachine fromChildStateMachine in fromRootStateMachine.stateMachines) {
          AnimatorStateMachine fromStateMachine = fromChildStateMachine.stateMachine;

          if (stateMachines.TryGetValue(fromStateMachine.name, out UnityEditor.Animations.AnimatorStateMachine obj)) {
            WalkStateMachine(controller, fromStateMachine, obj, toRootStateMachine);
          }

        }
      }

    }

    private void GeneratePostAnimation(Template t) {
      if (t.clips == null) return;
      foreach(AnimationClip clip in t.clips) {
        var basename = Path.GetFileNameWithoutExtension(clip.path);

        if (basename.Contains("Post")) {
          var path = clip.path;
          if (!AssetDatabase.CopyAsset(path.Replace("Post", ""), path)) {
            Debug.Log("[AC2] Failed to create: " + path);
          }
        
          var postC = AssetDatabase.LoadAssetAtPath <UnityEngine.AnimationClip>(path);
  
          foreach(var binding in AnimationUtility.GetCurveBindings(postC)) {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(postC, binding);
            Keyframe[] keys = curve.keys;
            var lastFrame = keys[keys.Length - 1];
            AnimationCurve postCurve = new AnimationCurve();
            postCurve.AddKey(0, lastFrame.value);
            postCurve.AddKey(clip.length, lastFrame.value);
            AnimationUtility.SetEditorCurve(postC, binding, postCurve);
          }
          var _path =  $"{Application.dataPath}/{path}".Replace("/Assets/Assets/Resources/", "/Assets/Resources/");
         
          var before = File.ReadAllText(_path);

          var after = Regex.Replace(before, @"m_StopTime: ([\.0-9]*)", $"m_StopTime: {clip.length}");

          File.WriteAllText(_path, after);

        }
      }
    }

    private void EditAnimationClip(Template t) {
      if (t.clips == null) return;
      foreach(AnimationClip clip in t.clips) {
        UnityEngine.AnimationClip c = AssetDatabase.LoadAssetAtPath <UnityEngine.AnimationClip>(clip.path);
        if (c != null) {
          List <UnityEngine.AnimationEvent>evts = new List <UnityEngine.AnimationEvent>();
          foreach(AnimationEvent evt in clip.events) {
            UnityEngine.AnimationEvent e = new UnityEngine.AnimationEvent();
            e.floatParameter = evt.floatParameter;
            e.functionName = evt.functionName;
            e.intParameter = evt.intParameter;
            var objRef = AssetDatabase.LoadAssetAtPath <UnityEngine.Object>(evt.objectReferenceParameter);
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
        } else {
            Debug.Log("[AC2] Failed to load: " + clip.path);
        }
      }
    }

  }

}