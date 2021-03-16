namespace AC2
{
    public class AnimatorStateMachine
    {
        public string name
        {
            get;
            set;
        }
        public UnityEngine.Vector3 anyStatePosition
        {
            get;
            set;
        }

        public AnimatorStateTransition[] anyStateTransitions
        {
            get;
            set;
        }

        public UnityEngine.StateMachineBehaviour[] behaviours
        {
            get;
            set;
        }

        public AnimatorState defaultState
        {
            get;
            set;
        }

        public UnityEngine.Vector3 entryPosition
        {
            get;
            set;
        }

        public AnimatorTransition[] entryTransitions
        {
            get;
            set;
        }

        public UnityEngine.Vector3 exitPosition
        {
            get;
            set;
        }

        public UnityEngine.Vector3 parentStateMachinePosition
        {
            get;
            set;
        }

        public ChildAnimatorStateMachine[] stateMachines
        {
            get;
            set;
        }

        public ChildAnimatorState[] states
        {
            get;
            set;
        }

        public AnimatorTransition[] transitions
        {
            get;
            set;
        }
    }
}