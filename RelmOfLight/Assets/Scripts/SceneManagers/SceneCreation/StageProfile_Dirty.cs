using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

[System.Serializable]
[CreateAssetMenu(fileName = "New_Stage_Profile", menuName = "ScriptableObjects/Stage/Stage Profile Dirty")]
public class StageProfile_Dirty : StageProfile_Base
{
    [HideInInspector, SerializeField] public GameObject Rewired;
    [HideInInspector, SerializeField] public GameObject BattleInfoManager;
    [HideInInspector, SerializeField] public GameObject BaseEnvironment;
    [HideInInspector, SerializeField] public GameObject BattleManager;
    [HideInInspector, SerializeField] public GameObject UI_Battle;
    [HideInInspector, SerializeField] public GameObject EventManager;
    [HideInInspector, SerializeField] public GameObject Wave;
    [HideInInspector, SerializeField] public GameObject IndicatorCanvas;
    [HideInInspector, SerializeField] public GameObject AudioManager;
    //______________________________________________________________________________________________________________
}



public class StageProfile_Base : ScriptableObject
{
    [Header("General")]
    [HideInInspector, SerializeField] public string Name;
    [HideInInspector, SerializeField] public string AddressableBaseName;
    [HideInInspector, SerializeField] public string ID = "S0_XMPL";
    [HideInInspector, SerializeField] public string sceneName = "BattleScene_StageXX";
    [HideInInspector, SerializeField] public StageNameType stageNameType = StageNameType.NotAssigned;
    [HideInInspector, SerializeField] public StageType type;
    //  [HideInInspector, SerializeField] public Sprite Thumbnail;
    [TextArea(5, 15), HideInInspector, SerializeField] public string Description;

    [HideInInspector, SerializeField] public string[] RequiredCompletedStages = new string[0];


    [Header("From Menu Actions")]
    public bool LoadDirectly = false;
    [Tooltip("Force set the characters required by this stage without overriding the squad layout defaults")] public bool ForceSetOverrideStorySquad = true;
    [ConditionalField("ForceSetOverrideStorySquad")] public CharacterNameType[] OverrideStorySquadToSet = new CharacterNameType[] { CharacterNameType.CleasTemple_Character_Valley_Donna, CharacterNameType.None, CharacterNameType.None, CharacterNameType.None };
    [Tooltip("Force set the characters required by this stage without overriding the squad layout defaults")] public bool ForceSetOverrideReplaySquad = true;
    [ConditionalField("ForceSetOverrideReplaySquad")] public CharacterNameType[] OverrideReplaySquadToSet = new CharacterNameType[] { CharacterNameType.None, CharacterNameType.None, CharacterNameType.None, CharacterNameType.None };
    [Tooltip("Force set the characters required by this stage and override the squad layout defaults")] public bool ForceSetSquad = false;
    [ConditionalField("ForceSetSquad")] public CharacterNameType[] SquadToSet = new CharacterNameType[4];

    [Header("Resolution Stuff")]
    [SerializeField] public RewardsRating bestAccuracyRating = new RewardsRating();
    [SerializeField] public RewardsRating bestReflexRating = new RewardsRating();
    [SerializeField] public RewardsRating bestDamageRating = new RewardsRating();


    //STAGE OBJECTS___STAGE OBJECTS___STAGE OBJECTS___STAGE OBJECTS___STAGE OBJECTS___STAGE OBJECTS___STAGE OBJECTS
    [HideInInspector] public bool Addressables = true;

    [Space(5)]
    [Header("Analytics")]
    [SerializeField] public bool trackPhases = false;
}