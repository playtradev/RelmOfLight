#if UNITY_STANDALONE || UNITY_STEAM || TRAIL
#define UNITY_PC
#endif

#if UNITY_XBOXONE || UNITY_SWITCH
#define UNITY_CONSOLE
#endif


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class BuildPlatformSwitch : MonoBehaviour
{
    public enum PlatformType 
    {
        NotAssigned = 0,
        PC = 100, StandAlone = 101, Steam = 102, Trail = 103, 
        Console = 200, XBoxOne = 201, Switch = 202 
    }
    [Header("When:")]
    public PlatformType[] Platforms = new PlatformType[0];

    public enum PlatformCheckType { IsThePlatform, IsNotThePlatform }
    public PlatformCheckType CheckType = PlatformCheckType.IsThePlatform;
    protected bool Inverted => CheckType == PlatformCheckType.IsNotThePlatform;

    
    [Header("Do:"), Space(10)]
    public BuildPlatformSwitchAction[] Actions = new BuildPlatformSwitchAction[0];





    private void Awake()
    {
        foreach (PlatformType platform in Platforms)
        {
            switch (platform)
            {
                case PlatformType.NotAssigned:
                    Debug.LogError("No platform assigned to platform switch, will execute all actions");
                    break;
                case PlatformType.PC:
#if !UNITY_PC
                    if (!Inverted) return;
#endif
                    break;
                case PlatformType.StandAlone:
#if !UNITY_STANDALONE
                    if (!Inverted) return;
#endif
                    break;
                case PlatformType.Steam:
#if !UNITY_STEAM
                    if (!Inverted) return;
#endif
                    break;
                case PlatformType.Trail:
#if !TRAIL
                    if (!Inverted) return;
#endif
                    break;
                case PlatformType.Console:
#if !UNITY_CONSOLE
                    if (!Inverted) return;
#endif
                    break;
                case PlatformType.XBoxOne:
#if !UNITY_XBOXONE
                    if (!Inverted) return;
#endif
                    break;
                case PlatformType.Switch:
#if !UNITY_SWITCH
                    if (!Inverted) return;
#endif
                    break;
                default:
                    break;
            }
        }


        foreach (BuildPlatformSwitchAction Action in Actions)
        {
            Action.DoAction();
        }
    }

    private void OnValidate()
    {
        foreach (BuildPlatformSwitchAction Action in Actions)
        {
            Action.OnValidate();
        }
    }
}


[System.Serializable]
public class BuildPlatformSwitchAction
{
    [HideInInspector] public string Name = "";

    public enum SwitchActionType
    {
        None,
        SetObjectActive,
    }
    public SwitchActionType ActionType = SwitchActionType.None;


    public delegate void ActionMethod();
    Dictionary<SwitchActionType, ActionMethod> ActionMethods => new Dictionary<SwitchActionType, ActionMethod>
        {
            { SwitchActionType.None, Action_None },
            { SwitchActionType.SetObjectActive, Action_SetObjectActive },
        };
    public void DoAction() => ActionMethods[ActionType]?.Invoke();




    public void OnValidate()
    {
        Name = ActionType.ToString();
    }


    //ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS____ACTIONS

    void Action_None() => Debug.LogError("Attempted to do an action in the platform switch but no action was selected!!!! [Belt]");

    [ConditionalField("ActionType", false, SwitchActionType.SetObjectActive)] [SerializeField] protected GameObject SetObjectActive_Object;
    [ConditionalField("ActionType", false, SwitchActionType.SetObjectActive)] [SerializeField] protected bool SetObjectActive_ActiveState;
    void Action_SetObjectActive() => SetObjectActive_Object?.SetActive(SetObjectActive_ActiveState);
}