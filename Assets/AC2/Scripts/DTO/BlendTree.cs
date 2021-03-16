namespace AC2
{
    public class BlendTree
    {
        public string blendParameter
        {
            get;
            set;
        }
        public string blenderParameterY
        {
            get;
            set;
        }
        public UnityEditor.Animations.BlendTreeType blendType
        {
            get;
            set;
        }

        public UnityEditor.Animations.ChildMotion[] children
        {
            get;
            set;
        }
        public float maxThreshold
        {
            get;
            set;
        }

        public float minThreshold
        {
            get;
            set;
        }
        public bool useAutomaticThresholds
        {
            get;
            set;
        }

    }
}