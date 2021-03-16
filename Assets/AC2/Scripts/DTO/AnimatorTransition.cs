namespace AC2
{
    public class AnimatorTransition
    {
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