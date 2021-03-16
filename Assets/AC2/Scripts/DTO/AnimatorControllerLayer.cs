namespace AC2
{
    public class AnimatorControllerLayer
    {
        public UnityEngine.AvatarMask avatarMask
        {
            get;
            set;
        }

        public UnityEditor.Animations.AnimatorLayerBlendingMode blendingMode
        {
            get;
            set;
        }

        public float defaultWeight
        {
            get;
            set;
        }

        public bool iKPass
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
        public AnimatorStateMachine stateMachine
        {
            get;
            set;
        }

        public bool syncedLayerAffectsTiming
        {
            get;
            set;
        }
        public int syncedLayerIndex
        {
            get;
            set;
        }
    }
}