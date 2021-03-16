namespace AC2
{
    public class AnimatorStateTransition
    {
        public bool canTransitionToSelf
        {
            get;
            set;
        }

        public float duration
        {
            get;
            set;
        }

        public float exitTime
        {
            get;
            set;
        }

        public bool hasExitTime
        {
            get;
            set;
        }

        public bool hasFixedDuration
        {
            get;
            set;
        }

        public UnityEditor.Animations.TransitionInterruptionSource interruptionSource
        {
            get;
            set;
        }

        public float offset
        {
            get;
            set;
        }

        public bool orderedInterruption
        {
            get;
            set;
        }

        public AnimatorCondition[] conditions
        {
            get;
            set;
        }

        public AnimatorState destinationState
        {
            get;
            set;
        }
        public AnimatorStateMachine destinationStateMachine
        {
            get;
            set;
        }

        public bool isExit
        {
            get;
            set;
        }

        public bool mute
        {
            get;
            set;
        }

        public bool solo
        {
            get;
            set;
        }

        public UnityEngine.HideFlags hideFlags
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }
    }
}