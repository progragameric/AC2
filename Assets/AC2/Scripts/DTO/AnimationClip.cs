namespace AC2
{
    public class AnimationClip
    {
        public string name
        {
            get;
            set;
        }

        public string path
        {
            get;
            set;
        }
        public float length
        {
            get;
            set;
        }
        public AnimationEvent[] events
        {
            get;
            set;
        }
    }
}