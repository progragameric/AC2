namespace AC2 {
  public class AnimatorState {
    public string name {
      get;
      set;
    }
    public UnityEngine.StateMachineBehaviour[] behaviours {
      get;
      set;
    }

    public float cycleOffset {
      get;
      set;
    }

    public string cycleOffsetParameter {
      get;
      set;
    }

    public bool cycleOffsetParameterActive {
      get;
      set;
    }

    public bool iKOnFeet {
      get;
      set;
    }

    public bool mirror {
      get;
      set;
    }

    public string mirrorParameter {
      get;
      set;
    }

    public bool mirrorParameterActive {
      get;
      set;
    }

    public UnityEngine.Motion motion {
      get;
      set;
    }

    public string nameHash {
      get;
      set;
    }

    public float speed {
      get;
      set;
    }

    public string speedParameter {
      get;
      set;
    }

    public bool speedParameterActive {
      get;
      set;
    }

    public string tag {
      get;
      set;
    }

    public string timeParameter {
      get;
      set;
    }

    public bool timeParameterActive {
      get;
      set;
    }

    public AnimatorStateTransition[] transitions {
      get;
      set;
    }
    public bool writeDefaultValues {
      get;
      set;
    }
  }
}