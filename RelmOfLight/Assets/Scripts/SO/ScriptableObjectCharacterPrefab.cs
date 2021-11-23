using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;
using Spine.Unity;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterPrefab/CharacterBasePrefab")]
public class ScriptableObjectCharacterPrefab : ScriptableObject
{
  //  [HideInInspector, SerializeField] public GameObject CharacterPrefab;

    [Header("General")]
    public AbridgedCharacterInfo AbridgedCharInfo;
    public CharacterClassType CharacterClass;// => AbridgedCharInfo == null ? CharacterClassType.Any : AbridgedCharInfo.ClassType;

    public List<LevelsInfoClass> Levels = new List<LevelsInfoClass>
    {
        new LevelsInfoClass(LevelType.Novice, 0),
        new LevelsInfoClass(LevelType.Defiant, 1000),
        new LevelsInfoClass(LevelType.Heroine, 3600),
        new LevelsInfoClass(LevelType.Goddess, 9850)
    };

    public List<Vector2Int> OccupiedTiles = new List<Vector2Int>();

    [Header("Lore")]
    [TextArea(3, 6)] public string Biography = "NO BIO";

    [Header("Procedural Wave Config")]
    public WaveGenCharInfo WaveGenInfo = new WaveGenCharInfo();
}

[System.Serializable]
public class WaveGenCharInfo
{
    public bool IncludeInProceduralWaves = false;
    [ConditionalField("IncludeInProceduralWaves")][Tooltip("A ranking for the difficulty of the encounter, with the weakest minion being at 0 and the strongest boss being at 100")] public Vector2 MinMaxChallengeRating = new Vector2(0, 15);
    public bool FallsWithinCR(Vector2 crRange, out int[] levelForCR)
    {
        List<int> outLevels = new List<int>();
        float curCR = 0;
        for (int i = 0; i < 10; i++)
        {
            curCR = MinMaxChallengeRating.x + (i * ((MinMaxChallengeRating.y - MinMaxChallengeRating.x) / 9f));
            if (curCR >= crRange.x && curCR <= crRange.y) outLevels.Add(i + 1);
        }
        levelForCR = outLevels.ToArray();
        return outLevels.Count > 0;
    }
    public float DistanceToClosestCRFromPoint(float originCR, out int closestCrLevel)
    {
        closestCrLevel = 1;
        float closestDist = 100000000f;
        for (int i = 0; i < 10; i++)
        {
            if (closestDist > Mathf.Abs(originCR - (MinMaxChallengeRating.x + (i * ((MinMaxChallengeRating.y - MinMaxChallengeRating.x) / 9f)))))
            {
                closestDist = Mathf.Abs(originCR - (MinMaxChallengeRating.x + (i * ((MinMaxChallengeRating.y - MinMaxChallengeRating.x) / 9f))));
                closestCrLevel = i + 1;
            }
        }
        return closestDist;
    }
    [ConditionalField("IncludeInProceduralWaves")] public StageNameType[] StageTags = new StageNameType[] { StageNameType.NotAssigned };
    [ConditionalField("IncludeInProceduralWaves")] public WaveGen_EncounterType WaveNPCType = WaveGen_EncounterType.NotAssigned;
    [ConditionalField("IncludeInProceduralWaves")] [Tooltip("The chance of picking this character out of a pool of other potential characters")] public WaveGen_RarityType Rarity = WaveGen_RarityType.Common;
    public int EncounterChances
    {
        get
        {
            switch (Rarity)
            {
                case WaveGen_RarityType.Common:
                    return 100;
                case WaveGen_RarityType.Uncommon:
                    return 75;
                case WaveGen_RarityType.Rare:
                    return 50;
                case WaveGen_RarityType.VeryRare:
                    return 25;
                case WaveGen_RarityType.ExtremelyRare:
                    return 10;
                default:
                    return 0;
            }
        }
    }


}

/// <summary>
/// A class containing stats about the character that can be used in a lighter fashion than the CharacterInfoScript can be used in
/// </summary>
[System.Serializable]
public class AbridgedCharacterInfo
{
    [Header("General Identity")]
    public string Name;
    public string AddressableLocation;
    public CharacterNameType CharacterID;
    public CharacterClassType ClassType;
    public ElementalType Elemental;

    [SerializeField] public System.DateTime ConfigurationDate;

    [Header("Visuals")]
    public Sprite CharacterIcon;
    public SkeletonDataAsset skeletonDataAsset;
    public bool UseSkins = false;
    public List<SkeletonDataAsset> Skins = new List<SkeletonDataAsset>();
   
    public string initialSkinName;

    [Header("StatsBlock"), Space(5)]
    public float HealthStats_Health;
    public float HealthStats_Armour;
    public float EtherStats_Ether;
    public float DamageStats_BaseDamage;
    public float SpeedStats_BaseSpeed;
    public float SpeedStats_CalculatedSpeed;
    public float SpeedStats_MovementSpeed;
    public Vector2 SpeedStats_WeakBulletSpeedV;
    public float SpeedStats_WeakBulletSpeed;
    public Vector2 SpeedStats_StrongBulletSpeedV;
   
    public DeathBehaviourType Behaviour_DeathBehaviour;
    public float DeathExplosion_Delay;
    public float DeathDisableing_Delay;
    public ParticlesType DeathExplosion_OverrideParticles;

    [Header("Behaviour and Attacks")]
    public List<ScriptableObjectAttackBase> CurrentAttackTypeInfo;
    public ScriptableObjectSkillBase[] SkillLevels = new ScriptableObjectSkillBase[10];
    public ScriptableObjectPassiveSkill[] PassiveSkills;
    public int AIs_Count;

    //Properties
    public int LevelRequiredForSkill(ScriptableObjectSkillBase skill)
    {
        for (int i = 0; i < SkillLevels.Length; i++)
        {
            if (SkillLevels[i] != null && skill.name.Contains(SkillLevels[i].name))
                return i + 1;
        }
        return 0;
    }
    public ScriptableObjectAttackBase[] AllUnlockedAttacks => CurrentAttackTypeInfo.Where(r => LevelRequiredForSkill(r) <= 1/* && LevelRequiredForSkill(r) >= StatsLevels.LowestLevel*/).ToArray();



    public AbridgedCharacterInfo()
    {

    }

    public AbridgedCharacterInfo(GameObject prefab)
    {
        if (prefab == null) return;
        CharacterInfoScript charInfo = prefab.GetComponentInChildren<CharacterInfoScript>();
        SkeletonAnimation skeleton = prefab.GetComponentInChildren<SkeletonAnimation>();
        if (charInfo == null) return;

        ConfigurationDate = System.DateTime.Now;
        AddressableLocation = prefab.name;

        Name = charInfo.Name;
        CharacterID = charInfo.CharacterID;
        HealthStats_Health = charInfo.HealthStats.Health;
        DamageStats_BaseDamage = charInfo.DamageStats.BaseDamage;
      
        SpeedStats_BaseSpeed = charInfo.SpeedStats.BaseSpeed;
        SpeedStats_MovementSpeed = charInfo.SpeedStats.MovementSpeed;
        SpeedStats_CalculatedSpeed = charInfo.SpeedStats.CalculatedSpeed;
        HealthStats_Armour = charInfo.HealthStats.Armour;
        SpeedStats_WeakBulletSpeedV = charInfo.SpeedStats.WeakBulletSpeedV;
        SpeedStats_StrongBulletSpeedV = charInfo.SpeedStats.StrongBulletSpeedV;
        Behaviour_DeathBehaviour = charInfo.Behaviour.DeathBehaviour;
        DeathExplosion_Delay = charInfo.DeathExplosion_Delay;
        DeathDisableing_Delay = charInfo.DeathDisableing_Delay;
        DeathExplosion_OverrideParticles = charInfo.DeathExplosion_OverrideParticles;
        CharacterIcon = charInfo.CharacterIcon;
        ClassType = charInfo.ClassType;
        Elemental = charInfo.Elemental;
        CurrentAttackTypeInfo = charInfo.CurrentAttackTypeInfo;
        SkillLevels = charInfo.SkillLevels;
        PassiveSkills = charInfo.PassiveSkills;
        SpeedStats_WeakBulletSpeed = charInfo.SpeedStats.WeakBulletSpeed;
        skeletonDataAsset = skeleton.skeletonDataAsset;
        initialSkinName = skeleton.initialSkinName;
    }
}