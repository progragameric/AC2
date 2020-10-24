using UnityEditor.Animations;

namespace AC2
{
    public class Template
    {
        public string name;

        public string saveTo;
        public AnimatorController controller;

        public AC2AnimationClip[] clips;

        public AnimatorStateMachine stateMachines;

        public AnimatorState states;

    }
}