using MyBox;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkillMask")]
public class ScriptableObjectSkillMask : ScriptableObject
{
    [Header("ID")]
    public string DisplayName = "MaskName";
    public MaskTypes Type = MaskTypes.None;

    [Header("Image Displays")]
    public Sprite Portrait = null;
    public Sprite Profile = null;
    public Sprite Icon = null;

    [Header("General")]
    public CharacterNameType BossToSpawn;
    public ElementalType[] ElementalTypes;

    public bool IsEffectOnCaster = false;
    [ConditionalField("IsEffectOnCaster", false)] public ScriptableObjectAttackEffect EffectOnCaster;

    public int CurrentMaskSigils = 0;

    public MaskSkillInfoClass[] Skills;
    public float ScaleMultiplier = 3;
    public Vector3 PositionOffset;
    public float DelayAfterAttack = 0;
    public int MaskPoints = 25;

    [Header("Phase 1"), Header("Skill Infos")]
    public float SkillCharFadeInOutDuration = 0.2f;
    [Range(0, 10)] public float DimingDelay = 0;
    public float LightDimValue = 0;
    public float LightDimTime = 0.3f;
    [Header("Phase 2")]
    [Range(0, 10)] public float CameraMovementDelayOut = 0;
    [Range(0, 10)] public float DelayParticlesPlayerDisappearing = 0f;
    [Range(0, 10)] public float DelayPlayerDisappearing = 0.01f;
    [Range(0, 10)] public float DelayMaskBossAppearing = 0.2f;
    [Header("Phase 3")]
    [Range(0, 10)] public float WaitBeforeMaskStartsAttack = 1;
    [Range(0, 10)] public float WaitAfterMaskAttacked = 0.2f;
    [Header("Phase 4")]
    [Range(0, 10)] public float WaitBossToDisappear = 0.3f;
    [Header("Phase 5")]
    [Range(0, 10)] public float CameraMovementDelayIn = 0;
    [Range(0, 10)] public float WaitCharsToAppear = 0;
    [Range(0, 10)] public float WaitForEndSound = 0.5f;
    [Range(0, 10)] public float FinalWaitBeforeComingBackToPlay = 1;

    [Header("Skill Camera and Zoom Infos")]
    public AnimationCurve SkillAnimCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1,1) });
    [Range(0, 10)] public float SkillZoom = 4.24f;
    [Tooltip("Duration of the transition out")] [Range(0, 3)] public float SkillZoomOutDuration = 0.5f;
    [Tooltip("Duration of the transition out")] [Range(0, 3)] public float SkillZoomInDuration = 0.5f;
    public Vector3 BossPostionRespectCam = new Vector3(-5, 0, 8.5f);



    public StatChange statChangeAtLevel(int level)
    {
        return StatChanges.Where(r => r.LevelOfChange == level).FirstOrDefault();
    }
    public StatChange[] StatChanges;

    private void OnValidate()
    {
        for(int i = 0; i < Skills.Length; i++)
        {
            if (Skills[i] == null) continue;
            Skills[i].levelUnlocked = Skills[i].levelUnlocked == 0 ? (i + 1) : Skills[i].levelUnlocked;
            Skills[i].Name = Skills[i].DisplayName;
        }
        for (int i = 0; i < StatChanges.Length; i++)
        {
            StatChanges[i].LevelOfChange = i + 1;
            StatChanges[i].Name = "LEVEL " + StatChanges[i].LevelOfChange.ToString() + " STAT CHANGES";
        }

        Skills.OrderBy(a => a.levelUnlocked);
        //if (BattleCry != null && BattleCry.audioBus != null)
        //{
        //    BattleCry.AudioBusName = BattleCry.audioBus.name;

        //}
    }
}

[System.Serializable]
public class StatChange
{
    [HideInInspector] public string Name = "";
    [HideInInspector] public int LevelOfChange = 0;
    public int SigilToLevelUp = 0;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float HPChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float HPRegenChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float ArmourChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float ShieldRegenChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float EtherChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float EtherRegenChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float MovementSpeedChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float AgilityChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float SigilDropBonus = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float WeakCriticalChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float StrongCriticalChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float WeakMultiplierChange = 0f;
    [Tooltip("A percentage change in either the possitive or negative direction, where -1 is -100% and 1 is +100%")] [Range(-1f, 1f)] public float StrongMultiplierChange = 0f;
}

[System.Serializable]
public class MaskSkillInfoClass
{
    [HideInInspector] public string Name = "";
    public string GeneratedName
    {
        get
        {
            return "LEVEL " + levelUnlocked.ToString().ToUpper() + " || " + DisplayName.ToUpper();
        }
    }

    public string DisplayName = "SkillName";
    [TextArea(2,5)] public string Description = "SKill Description";
    public Sprite Icon = null;
    public ScriptableObjectAttackBase Attack;

   [HideInInspector] [Range(1, 4)] public int levelUnlocked = 0;

    public MaskSkillInfoClass()
    {
        Name = "SkillName";
        Description = "SKill Description";
        Attack = null;
        levelUnlocked = 0;
    }
}
