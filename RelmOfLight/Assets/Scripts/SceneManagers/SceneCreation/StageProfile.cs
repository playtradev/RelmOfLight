using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

[System.Serializable]
[CreateAssetMenu(fileName = "New_Stage_Profile", menuName = "ScriptableObjects/Stage/Stage Profile")]
public class StageProfile : StageProfile_Base
{
    [HideInInspector, SerializeField] public string BattleInfoManagerName;
    [HideInInspector, SerializeField] public string BaseEnvironmentName;
    [HideInInspector, SerializeField] public string BattleManagerName;
    [HideInInspector, SerializeField] public string UI_BattleName;
    [HideInInspector, SerializeField] public string EventManagerName;
    [HideInInspector, SerializeField] public string WaveManagerName;
    [HideInInspector, SerializeField] public string IndicatorCanvasName;
    [HideInInspector, SerializeField] public string AudioManagerName;
}

[System.Serializable]
public class RewardsRating
{
    public float ValueToAchieve = 1f;
    public bool UseMaximumRewardSystem = true;
    [ConditionalField("UseMaximumRewardSystem")] public float MaximumReward = 500f;

    public RewardsRating()
    {
        ValueToAchieve = 1f;
        UseMaximumRewardSystem = true;
        MaximumReward = 500f;
    }
}