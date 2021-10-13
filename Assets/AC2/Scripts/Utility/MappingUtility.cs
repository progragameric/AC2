using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace AC2
{
    public class MappingUtility
    {

        public static void MapAnimatorControllerLayer(AnimatorControllerLayer from, UnityEditor.Animations.AnimatorControllerLayer to)
        {
            // avatarMask
            to.avatarMask = from.avatarMask;
            // blendingMode
            to.blendingMode = (UnityEditor.Animations.AnimatorLayerBlendingMode)from.blendingMode;
            // defaultWeight
            to.defaultWeight = from.defaultWeight;
            // iKPass
            to.iKPass = from.iKPass;
            // name
            to.name = from.name;
            // stateMachine - skip
            // syncedfromAffectsTiming
            to.syncedLayerAffectsTiming = from.syncedLayerAffectsTiming;
            // syncedfromIndex
            to.syncedLayerIndex = from.syncedLayerIndex;
        }

        public static void MapAnimatorStateTransition(AnimatorStateTransition from, UnityEditor.Animations.AnimatorStateTransition to)
        {
            // canTransitionToSelf
            to.canTransitionToSelf = from.canTransitionToSelf;
            // duration 
            to.duration = from.duration;
            // exitTime
            to.exitTime = from.exitTime;
            // hasExitTime
            to.hasExitTime = from.hasExitTime;
            // hasFixedDuration
            to.hasFixedDuration = from.hasFixedDuration;
            // interruptionDuration
            to.interruptionSource = (UnityEditor.Animations.TransitionInterruptionSource)from.interruptionSource;
            // offset
            to.offset = from.offset;
            // orderedInterruption
            to.orderedInterruption = from.orderedInterruption;
            // conditions
            if (from.conditions != null)
            {
                foreach (AnimatorCondition fromCondition in from.conditions)
                {
                    to.AddCondition(fromCondition.mode, fromCondition.threshold, fromCondition.parameter);
                }
            }
        }

        public static void MapAnimatorTransition(AnimatorTransition from, UnityEditor.Animations.AnimatorTransition to)
        {
            // destinationState - done
            // destinationStateMachine - done
            // isExit
            to.isExit = from.isExit;
            // mute
            to.mute = from.mute;
            // solo
            to.solo = from.solo;
            // hideFlags
            to.hideFlags = from.hideFlags;
            // name
            to.name = from.name;
            // conditions
            if (from.conditions != null)
            {
                foreach (AnimatorCondition fromCondition in from.conditions)
                {
                    to.AddCondition(fromCondition.mode, fromCondition.threshold, fromCondition.parameter);
                }
            }
        }

        public static void MapAnimatorState(AnimatorState from, UnityEditor.Animations.AnimatorState to)
        {
            // behaviours
            if (from.behavaviours == null)
            {
                to.behavaviours = new UnityEngine.StateMachineBehaviour[] { };
            }
            else
            {
                to.behaviours = from.behaviours;
            }
            // cycleOffset
            to.cycleOffset = from.cycleOffset;
            // cycleOffsetParameter
            to.cycleOffsetParameter = from.cycleOffsetParameter;
            // iKOnFeet
            to.iKOnFeet = from.iKOnFeet;
            // mirror
            to.mirror = from.mirror;
            // mirrorParameter
            to.mirrorParameter = from.mirrorParameter;
            // mirrorParameterActive
            to.mirrorParameterActive = from.mirrorParameterActive;
            // motion
            to.motion = from.motion;

            // nameHash - skip
            // speed
            to.speed = from.speed;
            // speedParameter
            to.speedParameter = from.speedParameter;
            // speedParameterActive
            to.speedParameterActive = from.speedParameterActive;
            // tag
            to.tag = from.tag;
            // timeParameter
            to.timeParameter = from.timeParameter;
            // timeParameterActive
            to.timeParameterActive = from.timeParameterActive;
            // transitions - done
            // writeDefaultValues
            to.writeDefaultValues = from.writeDefaultValues;
        }
    }
}