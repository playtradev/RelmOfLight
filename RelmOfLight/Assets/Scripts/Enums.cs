using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{

}

public enum CompareType
{
    MoreThan,
    LessThan,
    IsEqualTo,
    None,
}
public enum SavingSystemVersion
{
    V_0
}

public enum AchievementType
{
    Demo_Test1
}


public enum HintType
{
    Basics_00_ThankYou = 0,
    Basics_01_Controls = 1,
    TUTORIAL_DEFENCE = 2,
    TUTORIAL_DECISIONS = 3,
    TUTORIAL_UPGRADE = 4,
    TUTORIAL_SKILL = 5,
    TUTORIAL_CLASSES = 6
}


public enum LoadingHintType
{
    None,
    Hint_Image,
    Video
}


public enum DefendingActionType
{
    None,
    Normal,
    Reflected,
    Undefendable
}

public enum AudioCooldownType
{
    FrameWait,
    SecondWait
}

public enum BattleTileStateType
{
    NonUsable,
    Empty,
    Occupied,
    Blocked,
    Bound
}


public enum GridForceLogType
{
    Debug,
    Error,
    Warning,
    CodeFlow,
    Catcher,
}

public enum PassiveSkillTargetType
{
    Player,
    Enemy
}


public enum PassiveSkillsValueType
{
    Multiplier,
    Value,
}


public enum PassiveSkillType
{
    Sigils = 60,
    Element = 61,
    HP = 2,
    HP_Regen = 17,
    Armour = 18,
    ArmourType = 71,
    HP_Regen_OnGrid_OnOff = 70,
    Speed_Base = 3,
    Speed_Movement = 4,
    Speed_Attack_Loop = 90,
    Speed_Weak_Bullet = 25,
    Speed_Strong_Bullet = 26,
    Damage_Base = 0,
    Damage_Weak_Damage_Multiplier = 27,
    Damage_Strong_Damage_Multiplier = 30,
    Shield = 19,
    Shield_Regen = 8,
    Shield_Absorbtion_Weak = 20,
    Shield_Absorbtion_Strong = 34,
    Shield_InvulnerabilityTime = 21,
    Shield_Minion_Normal_Chances = 22,
    Shield_Minion_Perfect_Chances = 23,
    Ether = 11,
    Ether_Regen = 24,
    Ether_Regen_OnGrid_OnOff = 80,
    Agility_Chances = 100,
    Luck_Chances = 101,
    CriticalChances_Weak = 28,
    CriticalChances_Strong = 31,
    ScaleCharacterSize = 52,
    WalkingSide = 53,
    CharacterSpawnWait = 54,
    WhiteList = 1001,
    BlackList = 1002,
    AddBulletType = 1000
}


public enum SkipPointStatusType
{
    None,
    SetUp,
    Skipping
}

public enum FungusDialogType
{
    None,
    Dialog,
    Menu
}

public enum CameraShakeType
{
    None,
    Arrival,
    GettingHit,
    Powerfulattack,
    PowerfulAttackHit,
    Octopus_Tentacle
}

public enum DefendDamageType
{
    Old,
    New
}

public enum VibrationType
{
    a,
    b
}

public enum InputDirectionType
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}


public enum ButtonClickStateType
{
    Down,
    Press,
    Up
}


public enum BattleTileType
{
    Base,
    Buff_Health_Instant,
    Buff_Health_OverTime,
    Buff_Armor_ForTime,
    Buff_MovementSpeed_ForTime,
    Buff_Regeneration_ForTime,
    Buff_Stamina_ForTime,
    Buff_StaminaRegeneration_ForTime,
    Buff_AttackSpeed_ForTime,
    Buff_BulletSpeed_ForTime,
    Buff_AttackType_ForTime,
    Buff_Armor_Elemental_Neutral_1_ForTime,
    Buff_Armor_Elemental_Light_1_ForTime,
    Buff_Armor_Elemental_Dark_1_ForTime,
    Buff_Armor_Elemental_Earth_1_ForTime,
    Buff_Armor_Elemental_Lightning_1_ForTime,
    Buff_Armor_Elemental_Water_1_ForTime,
    Buff_Armor_Elemental_Fire_1_ForTime,
    Buff_Armor_Elemental_Ice_1_ForTime,
    Buff_Armor_Elemental_Wind_1_ForTime,
    Buff_Armor_Elemental_Life_1_ForTime,
    Buff_Armor_Elemental_Neutral_2_ForTime,
    Buff_Armor_Elemental_Light_2_ForTime,
    Buff_Armor_Elemental_Dark_2_ForTime,
    Buff_Armor_Elemental_Earth_2_ForTime,
    Buff_Armor_Elemental_Lightning_2_ForTime,
    Buff_Armor_Elemental_Water_2_ForTime,
    Buff_Armor_Elemental_Fire_2_ForTime,
    Buff_Armor_Elemental_Ice_2_ForTime,
    Buff_Armor_Elemental_Wind_2_ForTime,
    Buff_Armor_Elemental_Life_2_ForTime,
    Buff_Armor_Elemental_Neutral_3_ForTime,
    Buff_Armor_Elemental_Light_3_ForTime,
    Buff_Armor_Elemental_Dark_3_ForTime,
    Buff_Armor_Elemental_Earth_3_ForTime,
    Buff_Armor_Elemental_Lightning_3_ForTime,
    Buff_Armor_Elemental_Water_3_ForTime,
    Buff_Armor_Elemental_Fire_3_ForTime,
    Buff_Armor_Elemental_Ice_3_ForTime,
    Buff_Armor_Elemental_Wind_3_ForTime,
    Buff_Armor_Elemental_Life_3_ForTime,
    Debuff_Health_Instant,
    Debuff_Health_OverTime,
    Debuff_Armor_ForTime,
    Debuff_MovementSpeed_ForTime,
    Debuff_Regeneration_ForTime,
    Debuff_Stamina_ForTime,
    Debuff_StaminaRegeneration_ForTime,
    Debuff_AttackSpeed_ForTime,
    Debuff_BulletSpeed_ForTime,
    Debuff_AttackType_ForTime,
    Debuff_Armor_Elemental_Neutral_1_ForTime,
    Debuff_Armor_Elemental_Light_1_ForTime,
    Debuff_Armor_Elemental_Dark_1_ForTime,
    Debuff_Armor_Elemental_Earth_1_ForTime,
    Debuff_Armor_Elemental_Lightning_1_ForTime,
    Debuff_Armor_Elemental_Water_1_ForTime,
    Debuff_Armor_Elemental_Fire_1_ForTime,
    Debuff_Armor_Elemental_Ice_1_ForTime,
    Debuff_Armor_Elemental_Wind_1_ForTime,
    Debuff_Armor_Elemental_Life_1_ForTime,
    Debuff_Armor_Elemental_Neutral_2_ForTime,
    Debuff_Armor_Elemental_Light_2_ForTime,
    Debuff_Armor_Elemental_Dark_2_ForTime,
    Debuff_Armor_Elemental_Earth_2_ForTime,
    Debuff_Armor_Elemental_Lightning_2_ForTime,
    Debuff_Armor_Elemental_Water_2_ForTime,
    Debuff_Armor_Elemental_Fire_2_ForTime,
    Debuff_Armor_Elemental_Ice_2_ForTime,
    Debuff_Armor_Elemental_Wind_2_ForTime,
    Debuff_Armor_Elemental_Life_2_ForTime,
    Debuff_Armor_Elemental_Neutral_3_ForTime,
    Debuff_Armor_Elemental_Light_3_ForTime,
    Debuff_Armor_Elemental_Dark_3_ForTime,
    Debuff_Armor_Elemental_Earth_3_ForTime,
    Debuff_Armor_Elemental_Lightning_3_ForTime,
    Debuff_Armor_Elemental_Water_3_ForTime,
    Debuff_Armor_Elemental_Fire_3_ForTime,
    Debuff_Armor_Elemental_Ice_3_ForTime,
    Debuff_Armor_Elemental_Wind_3_ForTime,
    Debuff_Armor_Elemental_Life_3_ForTime,
    Debuff_Trap_ForTime,
    Debuff_Freeze_ForTime,
    Portal,
    Debuff_Weapon_Elemental_Neutral_ForTime,
    Debuff_Weapon_Elemental_Light_ForTime,
    Debuff_Weapon_Elemental_Dark_ForTime,
    Debuff_Weapon_Elemental_Earth_ForTime,
    Debuff_Weapon_Elemental_Lightning_ForTime,
    Debuff_Weapon_Elemental_Water_ForTime,
    Debuff_Weapon_Elemental_Fire_ForTime,
    Debuff_Weapon_Elemental_Ice_ForTime,
    Debuff_Weapon_Elemental_Wind_ForTime,
    Debuff_Weapon_Elemental_Life_ForTime,
}

public enum ControllerType
{
    Player1,
    Player2,
    Player3,
    Player4,
    Enemy,
    None
}

public enum SwapStateType
{
    Idle,
    Selection,
    StopSelection,
    Swapping
}

public enum TriggerType
{
    TriggerOnceOff,
    TriggerAndReset
}

public enum LightLayersType
{
    Default,
    Background,
    Grid,
    Player,
    Foreground,
    Special,
    Skill
}

public enum InputBehaviourType
{
    AIInput,
    PlayerInput,
    AIDumb,
    None = 1000
}
public enum MovementActionType
{
    Move,
    Teleport,
    Idle,
    CrossMovement,
    UpDown,
    LeftRight,
    None = 1000
}


public enum DeathBehaviourType
{
    Explosion,
    Defeat,
    Reverse_Arrives,
    Defeat_And_Explode,
    Defeat_And_Revive,
    NoAnim,
    None = 99999,
}

public enum SwappableActionType
{
    AIInput,
    PlayerInput,
    Particles,
    Tiles,
    Move,
    Teleport,
    Explosion,
    Defeat,
    Reverse_Arrives,
    Defeat_And_Explode,
    Idle,
    AIDumb,
    CrossMovement,
    Defeat_And_Revive,
    UpDown,
    NoAnim,
    LeftRight
}

public enum UnitBehaviourType
{
    ControlledByPlayer,
    NPC
}


public enum EventManagerVariablesType
{
    stringType,
    IntegerType
}

public enum WalkingSideType
{
    LeftSide,
    RightSide,
    Both
}

public enum BattleFieldAttackType
{
    OnAreaAttack,
    OnTarget,
    OnItSelf,
    OnRandom,
}

public enum CharacterActionType
{
    Move,
    Weak,
    Strong,
    Skill1,
    Skill2,
    Skill3,
    Defence,
    SwitchCharacter,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    GettingHit,
    Staggered,
    InputDefence,
    TutorialSkill1,
    TutorialSkill2,
    None = 999999
}

public enum BattleState
{
    Initialization = 0,
    FungusPuppets = 1,
    Event = 3,
    Battle = 4,
    Pause = 5,
    Menu = 6,
    WaveEnd = 7,
    WinLose = 8,
    Tutorial = 9,
    Intro = 10,
    Previous = 1000,
}

public enum ElementalType
{
    Neutral = 0,
    Earth = 1,
    Water = 2,
    Fire = 3,
    Light = 4,
    Air = 5,
    Dark = 6,
}


public enum ExclusionType 
{
    WhiteList, 
    BlackList,
}


public enum AttackPhasesType
{
    Start,
    Charging,
    Firing,
    Shoot,
    Reset,
    End,
}

public enum RelationshipType
{
    Strangers,
    Acquaintances,
    Friends,
    Sisters
}

public enum CharacterType
{
    c1,
    c2,
    c3,
    c4,
    c5,
    c6,
    c7,
    c8
}

public enum SceneSwitchType
{
    MenuScene,
    BattleScene,
    MenuSceneDemoVer,
    BattleScene_Stage00,
    BattleScene_Stage01,
    BattleScene_Stage02,
    BattleScene_Stage03,
    BattleScene_Stage04,
    BattleScene_Stage05,
    BattleScene_Stage06,
    BattleScene_Stage07,
    BattleScene_Stage08,
    BattleScene_Stage09,
    BattleScene_Stage10,
    StageProfileScene,

}

public enum SpriteAtlasQualityType
{
    High,
    Mid,
    Low,
}

public enum NextCharacterSelectionType
{
    Prev = -1,
    Next = 1,
}


public enum MatchType
{
    PvE,
    PvP,
    PPvE,
    PPvPP,
    PPPPvE,
    AIvsAI
}

public enum CharacterSelectionType
{
    First = 0,
    Second = 1,
    Third = 2,
    Fourth = 3,
    Fifth = 4,
    none = 1000
}


public enum HitParticlesType
{
    Normal,
    Resized,
    None
}

public enum StatsCheckerType
{
    Multiplier,
    Value,
    OnCasterAttack
}

public enum StatusEffectType
{
    Buff = 0,
    Debuff = 1
}


public enum BuffDebuffStatsType
{
    Regen = 1,
    Drain = 5,
    Zombie = 6,
    Bleed = 9,
    AttackChange = 10,
    Legion = 12,
    Invulnerable = 13,
    Rebirth = 14,
    Backfire = 15,
    Rage = 16,
    Bliss = 32,
    SoulClash = 33,

    Sigils = 60,
    Element = 61,


    HP = 2,
    HP_Regen = 17,
    Armour = 18,
    ArmourType = 71,
    HP_Regen_OnGrid_OnOff = 70,

    Speed_Base = 3,
    StopChar = 501,
    Speed_Movement = 4,
   // Speed_Attack_Loop = 90,
    Speed_Weak_Bullet = 25,
    Speed_Strong_Bullet = 26,


    Damage_Base = 0,
    Damage_Weak_Damage_Multiplier = 27,
    Damage_Strong_Damage_Multiplier = 30,


    Shield = 19,
    Shield_Regen = 8,
    Shield_Absorbtion_Weak = 20,
    Shield_Absorbtion_Strong = 34,
    Shield_InvulnerabilityTime = 21,
    Shield_Minion_Normal_Chances = 22,
    Shield_Minion_Perfect_Chances = 23,

    Ether = 11,
    Ether_Regen = 24,
    Ether_Regen_OnGrid_OnOff = 80,


    Agility_Chances = 100,
    Luck_Chances = 101,
    CriticalChances_Weak = 28,
    CriticalChances_Strong = 31,

    Teleport = 51,
    ScaleCharacterSize = 52,
    WalkingSide = 53,
    CharacterSpawnWait = 54,
    BoxColliderSize = 55,
    Disable_CollisionWithTileEffect = 56,
    Tile_Blocked = 7,
    Tile_Free = 58,
    Tile_ChangeSide = 59,

    ActionDisable_WeakAttack = 110,
    ActionDisable_StrongAttack = 111,
    ActionDisable_Skill1 = 112,
    ActionDisable_Skill2 = 113,
    ActionDisable_Mask = 114,
    ActionDisable_Move = 115,
    ActionDisable_Swap = 116,
    Confusion = 120,
    Undead = 121,
    ChancgeColor = 122,
    ChancgeColorWithCurve = 504,
    ForceAI = 123,
    ReplaceWithAI = 505,
    StealAttack = 124,
    RemoveBuffs = 125,
    RemoveDebuffs = 126,
    ShadowForm = 127,
    Stun = 128, //Can't attack, can't move, can't switch
    MeleeAttack = 500,
    KillPoolChar = 502,
    EndWave = 503,
    AggroMultiplier = 506,
    Cursed = 507,
    DeathSentence = 508,
    MovementTime = 509,
    AttackTime = 510,
    FireParticlesToChar = 1000,
}

public enum CharacterContainerType
{
    Player_Enemy,
    Talking
}


public enum BuffDebuffStackType
{
    Refreshable = 0, //Additional BuffDebuffs of the same kind will refresh the cooldown, if they are of higher level, they will also change the level to the new higher one
    Stackable = 1,//Additional BuffDebuffs of the same kind will upgrade the level and refresh the cooldown
}

public enum ImmunityType
{
    None = 0,
    Buffs = 1,
    Debuffs = 2,
}

public enum StatsCheckType
{
    None,
    Health,
    Ether,
    AttackSpeed,
    MovementSpeed,
    BaseSpeed,
    TeamTotalHpPerc,
    BuffDebuff,
    AI,
    CompareHP
}

public enum BulletType
{
    Base,
    Piercing,
    Pooping,
    Homing,
    Unstoppable, 
    IgnoresCollision,
    Grenade,
}

public enum DeathDropTypes
{
    Embue,
    Throw
}


public enum TriggerCheckType
{
    AtAnyPoint,
    Currently
};


public enum ArmourType
{
    Base,
    Blocking
}

public enum ModificableStatsType
{
    WeakAttack_CriticalChance = 0,
    WeakAttack_DamageMultiplier = 1,
    StrongfulAttac_CriticalChance = 2,
    StrongfulAttac_DamageMultiplier = 3,
    HealthStats_Regeneration = 6,
    EtherStats_Regeneration = 9,
    SpeedStats_BaseSpeed = 11,
    SpeedStats_MovementSpeed = 12,
    SpeedStats_WeakBulletSpeed = 15,
    SpeedStats_StrongBulletSpeed = 27,
    DamageStats_BaseDamage = 16,
    ShieldStats_Regeneration = 20,
    HealthStats_Armour = 22,
    ShieldStats_MinionShieldChances = 23,
    ShieldStats_MinionPerfectShieldChances = 24,
    Agility = 28

}

public enum AIAttackSequenceEventType
{
    OnComplete = 0,
    OnHit = 1
}

public enum AIType
{
    VeryDefensive = -20,
    Defensive = -10,
    Neutral = 0,
    Aggressive = 10,
    VeryAggressive = 20,
    Love = 100
}

public enum AggroType
{
    PlayerController,
    Position
}

public enum ElementalWeaknessType
{
    ExtremelyWeak = -3,
    VeryWeak = -2,
    Weak = -1,
    Neutral = 0,
    Resistent = 1,
    VeryResistent = 2,
    ExtremelyResistent = 3
}
public enum FieldOfViewType
{
    NearRange,
    MidRange,
    LongRange
}


public enum BulletLevelType
{
    Level_1,
    Level_2,
    Level_3,
    Level_4,
    Level_5,
    Level_6,
    Level_7,
    Level_8,
    Level_9,
    Level_10


}

public enum AttackAnimPrefixType
{
    Atk1 = 0,
    Atk2 = 1,
    S_Buff = 3,
    S_DeBuff = 4
}


public enum AttackAnimType
{
    Weak_Atk,
    Strong_Atk,
    Buff,
    Debuff,
}

public enum AttackInputType
{
    Weak,
    Strong,
    Skill1,
    Skill2,
    Skill3
}

public enum AttackParticlesInputType
{
    Weak,
    Strong,
    Skill1,
    Skill2,
    Skill3,
    Weak2,
    Weak3,
    Strong2,
    Strong3,
    Cinematic
}

public enum AttackTargetSideType
{
    EnemySide,
    FriendlySide,
    BothSides
}

public enum CharacterAnimationStateType
{

    NoMesh,
    Idle,
    Atk,
    Atk1,
    Atk1_AtkToIdle,
    Atk1_IdleToAtk,
    Atk1_Loop,
    Atk1_Charging,
    Atk2_AtkToIdle,
    Atk2_IdleToAtk,
    Atk2_Charging,
    Buff,
    Debuff,
    GettingHit,
    Defending,
    Paralized,
    Arriving,
    Growing,
    Growing1,
    Growing2,
    DashRight,
    DashLeft,
    DashDown,
    DashUp,
    Selection,
    PowerUp,
    Reverse_Arriving,
    Speaking,
    Victory,
    Defeat,
    Death,
    Idle_Disable_Loop,
    Death_Prep,
    Death_Loop,
    Death_Exit,
    Death_Born,
    Idle_Agressive,
    Idle_AtkToIdle,
    Idle_Charging,
    Idle_IdleToAtk,
    Idle_Loop,
    Defeat_ReverseArrive,
    Dialogue_Confused,
    Dialogue_Disappointed,
    Dialogue_Angry,
    Dialogue_Happy,
    Dialogue_Sad,
    Dialogue_Standard,
    Dialogue_Surprise,
    Dialogue_To_Idle,
    Idle_To_Dialogue,
	JumpTransition_IN,
    JumpTransition_OUT,
    DashUp_Intro,
    DashUp_Loop,
    DashUp_End,
    DashDown_Intro,
    DashDown_Loop,
    DashDown_End,
    DashLeft_Intro,
    DashLeft_Loop,
    DashLeft_End,
    DashRight_Intro,
    DashRight_Loop,
    DashRight_End,
    S_Arriving,
    S_Arriving_Loop,
    S_Arriving_LoopToIdle,
    Defeat_IdleToLoop,
    Defeat_Loop,
    Defeat_LoopToIdle,
    Speaking_IdleToLoop,
    Speaking_Loop,
    Speaking_LoopToIdle,
    Cine_Arriving,
    none = 1000

}



public enum TileActionType
{
    None,
    SingleHit,
    OnDuration,
    OverTime
}

public enum PortalType
{
    In,
    Out
}


public enum WaveNPCTypes
{
    Minion,
    Recruitable,
    Boss,
    None = 1000
}


public enum BaseCharType
{
    None = 10000,
    BaseCharacter = 0,
    Stage09_Boss_Geisha_Script = 10,
    Stage09_Boss_NoFace_Script = 11,
    Stage01_Boss_Script = 12,
    Stage02_Boss_Script = 13,
    Stage03_Boss_Script = 14,
    Stage05_BossMonster_Script = 2,
    Stage05_BossGirl_Script = 3,

}

// 0            ->      None
// 1 to 999     ->      Basic
// 1000 to 1999 ->      Bosses extra(leave 100 slots between bosses)
// 2000 to 2999 ->      Mask Skills(leave 100 slots between masks)
// 3000 to 3499 ->      Buff Status effects Buff
// 3500 to 3999 ->      Debuff Status effects 
// 4000 to 5999 ->      Miscelaneous
// 6000+        ->      Test
public enum ParticlesType   
{
    // 0            -> None
    None = 0,

    // 1 to 999     -> Basic
    CharArrivingSmoke = 1,
    ShieldNormal = 2,
    ShieldTotalDefence = 3,
    Death = 4,
    ReflectedBulletImpact = 5,
    ShieldBigNormal = 6,
    ShieldBigTotalDefence = 7,
    BulletImpactLeft = 8,
    BulletImpactRight = 9,
    DeathSmallEnemy = 10,
    Stun_Explosion = 11,

    //Tiles and blocks
    Tile_Forest = 99,
    Tile_Burning = 100,
    Tile_Burning_Hit = 109,
    Tile_Ice = 101,
    Death_Tile_Ice = 102,
    Death_Tile_Lava = 103,
    Death_Block_Shield = 104,
    Death_Block_Wood = 105,
    Death_Block_Ice = 106,
    Death_Block_Lava = 107,
    Death_Block_Rock = 108,
    Death_Block_PaiRock = 110,
    Tile_Tornado = 111,
    Tile_Kin_Nightsheart = 112,
    Tile_Mushroom_Explosion = 113,
    Tile_Mushroom = 114,
    Tile_Claim_Left = 115,
    Tile_Claim_Right = 116,
    Tile_Air_Tornado = 117,
    Death_Block_Walker = 118,
    Tile_SpeedBullet = 119,

    //200 -> Circles Buff
    Circle_Buff_HP = 200,
    Circle_Buff_Armour = 201,
    Circle_Buff_Ether = 202,
    Circle_Buff_Speed = 203,
    Circle_Buff_Luck = 204,
    Circle_Buff_Damage = 205,
    Circle_Buff_Reflection = 206,

    //250 -> Circles Debuff
    Circle_Debuff_HP = 250,
    Circle_Debuff_Armour = 251,
    Circle_Debuff_Ether = 252,
    Circle_Debuff_Speed = 253,
    Circle_Debuff_Luck = 254,
    Circle_Debuff_Damage = 255,

    // 500 -> Coins
    Coin_LIFE = 500,
    Coin_MATTER = 501,
    Coin_VOID = 502,
    Coin_MIND = 503,
    Coin_SPIRIT = 504,
    Coin_FORCES = 505,



    // 1000 to 1999 ->      Bosses extra(leave 100 slots between bosses)
    Chapter00_CleasTemple_TohoraSea_BossDeathSmoke = 1000,
    Chapter01_TohoraSea_Boss_MoonDrums_Loop = 1100,
    Chapter01_TohoraSea_Boss_MoonDrums_LoopCrumble = 1101,
    Chapter01_TohoraSea_Boss_TeleportationIn = 1102,
    Chapter01_TohoraSea_Boss_TeleportationOut = 1103,
    Chapter01_TohoraSea_Boss_FaceChanging_WarDrums = 1104,
    Chapter01_TohoraSea_Boss_FaceChanging_LifeDrums = 1105,
    Chapter01_TohoraSea_Boss_FaceChanging_MoonDrums = 1106,
    Chapter01_TohoraSea_Boss_CrystalTomb_Effect = 1107,
    Chapter01_TohoraSea_Boss_CrystalTomb_Tile_Explode = 1108,
    Chapter01_TohoraSea_Boss_Tikaka_DeathExplosion = 1109,
    Chapter01_TohoraSea_Boss_Tikaka_Drain = 1110,

    Chapter02_TheBurg_Boss_Mainframe_DeathExplosion = 1200,
    Chapter02_TheBurg_Boss_Mainframe_Sparkles = 1201,


    Chapter03_ForestOfKin_Boss_MoonCry_Effect = 1300,
    Chapter03_ForestOfKin_Boss_KinHowl = 1301,


    Chapter05_AscensoMountain_FlowersSmoke = 1500,
    Chapter05_AscensoMountain_ParticleTransference = 1501,
    Chapter05_AscensoMountain_Boss_QuillaTentacleCreation = 1502,


    Chapter08_MaidenShrine_Boss_Matriarchal_Drain = 1700,
    // 2100 to 2999 ->      Mask Skills(leave 100 slots between masks)
    Skill_Mind_1_Loop = 2100,
    Skill_Mind_2_Loop = 2101,
    Skill_Mind_2_Teleporting = 2102,
    Skill_Mind_3_In = 2103,
    Skill_Mind_3_Loop = 2104,
    Skill_Mind_3_Tentacle = 2105,
    Skill_Might_1_LegionOriginal = 2200,
    Skill_Might_1_LegionClone = 2201,
    Skill_Might_2_Invencible = 2202,

    // 3000 to 3499 -> Buff Status effects Buff
    Status_Buff_Power = 3000,//Atk
    Status_Buff_Regen = 3001,//Health
    Status_Buff_Regen_Hit = 3011,//Health
    Status_Buff_Bliss = 3002,//stamina
    Status_Buff_Haste = 3003,//Speed
    Status_Buff_Armour = 3004,//shield
    Status_Buff_Aim = 3005,
    Status_Buff_Rebirth = 3006,
    Status_Buff_Voice = 3007,
    Status_Buff_Drain = 3008,
    Status_Buff_Piercing = 3009,
    Status_Buff_Push = 3010,
    Status_Buff_LifeDrain = 3012,

    // 3500 to 3999 -> Debuff Status effects 
    Status_Debuff_Bleed = 3500,
    Status_Debuff_SoulCrash = 3501,
    Status_Debuff_Slow = 3502,
    Status_Debuff_Blind = 3503,
    Status_Debuff_Backfire = 3504,
    Status_Debuff_Stop = 3505,
    Status_Debuff_Shatter = 3506,
    Status_Debuff_Powerless = 3507,
    Status_Debuff_Death_Sentence = 3508,
    Status_Debuff_Silence = 3509,
    Status_Debuff_Rage = 3510,
    Status_Debuff_Chain = 3511,
    Status_Debuff_Zombie = 3512,
    Status_Debuff_Loud_Disruption = 3513,


    // 4000 to 5999 -> Miscelaneous
    AI_Status_VeryAgressive = 4000,
    AI_Status_VeryDefensive = 4001,
    AI_Status_Aggressive = 4002,
    AI_Status_Love = 4003,
    AI_Status_Neutral = 4004,
    AI_Status_Defensive = 4005,
    AI_Status_VeryAgressive_Explosion = 4006,
    AI_Status_VeryDefensive_Explosion = 4007,
    AI_Status_Aggressive_Explosion = 4008,
    AI_Status_Love_Explosion = 4009,
    AI_Status_Neutral_Explosion = 4010,
    AI_Status_Defensive_Explosion = 4011,

    UI_BulletGroundMarker = 4050,


    GroundType_Puddle_Swamp = 4100,

    //cutscenes
    AM_Rosa_Feather_Loop = 4200,
    AM_Rosa_Feather_Hit = 4201,

    MS_Joja_MassAttack = 4301,
    MS_Joja_Tracker = 4302,
    MS_Joja_Tracker_Hand = 4303,


    ChargeLoop_Chaos = 5001,
    ChargeLoop_Forces = 5002,
    ChargeLoop_Life = 5003,
    ChargeLoop_Matter = 5004,
    ChargeLoop_Mind = 5005,
    ChargeLoop_Spirit = 5006,
    ChargeExplosion_Chaos = 5007,
    ChargeExplosion_Forces = 5008,
    ChargeExplosion_Life = 5009,
    ChargeExplosion_Matter = 5010,
    ChargeExplosion_Mind = 5011,
    ChargeExplosion_Spirit = 5012,
    ChargeStartLoop_Chaos = 5013,
    ChargeStartLoop_Forces = 5014,
    ChargeStartLoop_Life = 5015,
    ChargeStartLoop_Matter = 5016,
    ChargeStartLoop_Mind = 5017,
    ChargeStartLoop_Spirit = 5018,
    MaskCharOut = 5101,
    MaskCharIn = 5102,

    DonnaMeleeGoIn = 6001,
    DonnaMeleeGoOut = 6002,
    DonnaMeleeBackIn = 6003,
    DonnaMeleeBackOut = 6004,
    DonnaMeleeSlash = 6005,
    DonnaMeleeHit = 6006,


    //Grenades

    Moss_Bear_Grenade = 7000,
    Moss_Bear_Grenade_Explosion = 7001,

    //Stun
    Stun = 8000,

    VOID = 99999,
}


public enum AttackParticlePhaseTypes
{
    Cast,
    Bullet,
    Hit,
    Charging,
    CastActivation,
}

public enum TeamSideType
{
    LeftSideTeam,
    RightSideTeam
}

public enum ItemType
{
    PowerUp_Damage,
    PowerUp_Speed,
    PowerUP_Health,
    PowerUP_FullRecovery,
    PowerUP_Stamina,
    PowerUp_All,
    PowerUp_Shield,
}


public enum FacingType
{
    Left,
    Right
}

public enum TileAttackPhaseType
{
    None,
    TileAttack,
    Cast,
    Bullet
}


public enum RelationshipBetweenElements
{
    Neutral = 0,
    ExtremelyResistent = 1,
    VeryResistent = 2,
    Resistent = 3,
    Weak = 4,
    VeryWeak = 5,
    VeryWeak_1 = 6,
    VeryWeak_2 = 7,
    VeryWeak_3 = 8,
    ExtremelyWeak = 9,
}


public enum PlayerColorType
{
    _FF0D0D,
    _FF0CDC,
    _0C21FF,
    _0CFF1F
}

public enum CharacterClassType
{
    Champion = 0,
    Defender = 1,
    Valiant = 2,
    Spellbinder = 3,
    Any = 1000,
}

public enum BattleFieldIndicatorType
{
    Damage,
    Defend,
    Heal,
    CriticalHit,
    Invulnerable,
    Rebirth,
    Backfire,
    Miss,
    CompleteDefend,
    Buff,
    Debuff,
    Armored,
    Shielded,
    Effective,
    UnEffective,
    Reflect,
    Ether,
    EffectiveText,
    InEffectiveText,
    NoDamage,
    StugDamage,
    Undefendable
}

public enum SavingVersionType
{
    None = 0,
    V0 = 1,
}

public enum AITransitionType
{
    None,
    To_Aggressive,
    To_Defensive,
    ToLove,
    To_Neutral,
    To_VeryAggressive,
    To_VeryDefensive
}


public enum CharacterNameType
{
    #region CharacterNameType
    None = 0,
    //Stage 00 - Clea's Temple
    CleasTemple_Minion_Mountain_Bomber = 1000,
    CleasTemple_Minion_Desert_Protector = 1001,
    CleasTemple_Minion_Forest_Wasper = 1002,
    CleasTemple_Minion_Valley_Sentinel = 1003,
    CleasTemple_Character_Mountain_Bird = 1004,
    CleasTemple_Character_Valley_Donna = 1005,
    CleasTemple_Character_Forest_Koniko = 1006,
    CleasTemple_Character_Desert_Pan = 1007,
    CleasTemple_BossOctopus = 1010,
    CleasTemple_BossOctopus_Head = 1011,
    CleasTemple_BossOctopus_Tentacles = 1012,
    CleasTemple_BossOctopus_Girl = 1013,
    CleasTemple_Boss_CleasHuman = 1014,
    CleasTemple_Character_Forest_KonikoCasual = 1015,
    CleasTemple_Boss_CleasRobot = 1016,
    CleasTemple_Character_Valley_Donna_Golden = 1017,



    //Stage 01 - Tohora
    Tohora_Minion_Mountain_Toka = 1020,
    Tohora_Minion_Desert_Fishylla = 1021,
    Tohora_Minion_Forest_Crabera = 1022,
    Tohora_Minion_Valley_Mothra = 1023,
    Tohora_Character_Valley_Noiti = 1024,
    Tohora_Character_Desert_Mermer = 1025,
    Tohora_Character_Mountain_Kora = 1026,
    Tohora_Character_Forest_Pai = 1027,
    Tohora_Boss_Tikaka = 1028,


    //Stage 02 - The Burg
    TheBurg_Minion_Mountain_IronTron = 1040,
    TheBurg_Minion_Desert_Robotron = 1041,
    TheBurg_Minion_Forest_HappyBot = 1042,
    TheBurg_Minion_Valley_Ted = 1043,
    TheBurg_Character_Valley_Switch = 1044,
    TheBurg_Character_Desert_Loud = 1045,
    TheBurg_Character_Mountain_Flint = 1046,
    TheBurg_Character_Forest_Deedra = 1047,
    TheBurg_Boss_Mainframe = 1048,
    TheBurg_Boss_Mainframe_Minion = 1049,


    //Stage 03 - Forest Of Kin
    ForestOfKin_Minion_Mountain_Moss_Bear = 1060,
    ForestOfKin_Minion_Desert_Wendigoat = 1061,
    ForestOfKin_Minion_Forest_Morthus = 1062,
    ForestOfKin_Minion_Valley_Notark = 1063,
    ForestOfKin_Character_Valley_Elu = 1064,
    ForestOfKin_Character_Desert_Valis = 1065,
    ForestOfKin_Character_Mountain_Seke = 1066,
    ForestOfKin_Character_Forest_Balla = 1067,
    ForestOfKin_Boss_Forest_Kin = 1068,


   

    //Stage 04
    RhodaCoast_Minion_Mountain_Claw = 1100,
    RhodaCoast_Minion_Desert_Bluebster = 1101,
    RhodaCoast_Minion_Forest_Hebi = 1102,
    RhodaCoast_Minion_Valley_Tusk = 1103,
    RhodaCoast_Character_Mountain_Kika = 1104,
    RhodaCoast_Character_Valley_Preta = 1105,
    RhodaCoast_Character_Forest_Camila = 1106,
    RhodaCoast_Character_Desert_Lua = 1107,

    //Stage 05 - Ascenso Mountains
    AscensoMountains_Minion_Mountain_Ballama = 1080,
    AscensoMountains_Minion_Desert_LadyBush = 1081,
    AscensoMountains_Minion_Forest_Furia = 1082,
    AscensoMountains_Minion_Valley_Monkena = 1083,
    AscensoMountains_Character_Valley_Miranda = 1084,
    AscensoMountains_Character_Desert_Rosa = 1085,
    AscensoMountains_Character_Mountain_Alanda = 1086,
    AscensoMountains_Character_Forest_Pina = 1087,
    AscensoMountains_BossGirl_Quilla = 1088,
    AscensoMountains_BossMonster_Pachamama = 1089,
    AscensoMountains_BossGirl_Quilla_Minion1 = 1090,
    AscensoMountains_BossGirl_Quilla_Minion2 = 1091,
    AscensoMountains_BossGirl_Quilla_Minion3 = 1092,
    AscensoMountains_BossGirl_Quilla_Minion4 = 1093,
    AscensoMountains_BossMonster_Pachamama_Minion1 = 1094,
    AscensoMountains_BossMonster_Pachamama_Minion2 = 1095,
    AscensoMountains_BossMonster_Pachamama_Minion3 = 1096,
    AscensoMountains_BossMonster_Pachamama_Minion4 = 1097,
    AscensoMountains_Minion_Forest_Pinata = 1098,
    AscensoMountains_BossGirl_Quilla_Minion = 10900,
    AscensoMountains_BossMonster_Pachamama_Minion = 10940,
    AscensoMountains_Minion_Mountain_Guu = 10990,
    AscensoMountains_Minion_Valley_Juu = 11000,
    

    //Stage 06
    DaikiniPeaks_Minion_Mountain_Tantun = 1120,
    DaikiniPeaks_Minion_Desert_StrangeCaw = 1121,
    DaikiniPeaks_Minion_Forest_Kabuto = 1122,
    DaikiniPeaks_Minion_Valley_Panchitta = 1123,
    DaikiniPeaks_Character_Valley_Thruthsayer = 1124,
    DaikiniPeaks_Character_Desert_Hoe = 1125,
    DaikiniPeaks_Character_Mountain_Bosha = 1126,
    DaikiniPeaks_Character_Forest_TalCass = 1127,
    DaikiniPeaks_Boss_Kala = 1128,


    //Stage 07
    RenagateZone_Minion_Mountain_Brawle = 1140,
    RenagateZone_Minion_Desert_Atlas = 1141,
    RenagateZone_Minion_Forest_Comander_Brina = 1142,
    RenagateZone_Minion_Valley_Corey = 1143,
    RenagateZone_Character_Valley_Navigator = 1144,
    RenagateZone_Character_Desert_Watcher = 1145,
    RenagateZone_Character_Mountain_Blaster = 1146,
    RenagateZone_Character_Forest_Neutralizer = 1147,


    //Stage 08
    MaidenShrine_Minion_Mountain_Takoma = 1160,
    MaidenShrine_Minion_Desert_Kitsune = 1161,
    MaidenShrine_Minion_Forest_Seitaurosu = 1162,
    MaidenShrine_Minion_Valley_Shikame = 1163,
    MaidenShrine_Character_Valley_Dorje = 1164,
    MaidenShrine_Character_Desert_LinSupreme = 1165,
    MaidenShrine_Character_Mountain_Skye = 1166,
    MaidenShrine_Character_Forest_Joja = 1167,
    MaidenShrine_Boss_Geisha = 1168,
    MaidenShrine_Boss_MinionSpear = 1169,
    MaidenShrine_NPC_TheRanger = 1170,
    MaidenShrine_NPC_DragonDojo = 1171,
    MaidenShrine_Minion_Forest_Guard = 1172,
    MaidenShrine_Minion_Forest_Djinn = 1173,
    MaidenShrine_Minion_Mountain_Komainu = 1174,



    //Stage 09
    MukuruCaves_Minion_Mountain_Elphantina = 1180,
    MukuruCaves_Minion_Desert_Globee = 1181,
    MukuruCaves_Minion_Forest_Yukai = 1182,
    MukuruCaves_Minion_Valley_Dyno = 1183,
    MukuruCaves_Character_Valley_Himba = 1184,
    MukuruCaves_Character_Mountain_QueenThema = 1185,
    MukuruCaves_Character_Desert_Eanda = 1186,
    MukuruCaves_Character_Forest_Aje = 1187,
    MukuruCaves_Boss_Jua = 1188,
    MukuruCAves_Boss_Giza = 1189,

    //SmokoLoko
    Potion_Smokoloko = 2000,
    TheBurg_NPC_Grennar = 2001,
    Character_Forest_The_Presence = 2002,
    Character_Trainer_Dummy = 2003,


    //Items
    Grenade_Base = 3001,

    //Structures
    Structure_Barrier_Ice = 5001,
    Structure_Barrier_Fire = 5002,
    Structure_Barrier_Wood = 5003,
    Structure_Barrier_AlandaBlock = 5004,
    Structure_Barrier_CrystalTomb = 5005,
    Structure_Barrier_Rock = 5006,
    Structure_Barrier_PaiFist = 5007,
    Structure_Barrier_PaiRock = 5008,
    Structure_Barrier_Tomb = 5009,
    Structure_Barrier_TotemShooter = 5010,
    Structure_Barrier_AlandaWalker = 5011,
    Structure_Barrier_Plague = 5012,



    //Testing Chars
    Testing_01 = 7000,
    Testing_02 = 7001,
    Testing_03 = 7002,
    Testing_04 = 7003,
    Testing_05 = 7004,
    Testing_06 = 7005,
    Testing_07 = 7006,
    Testing_08 = 7007,
    Testing_09 = 7008,
    Testing_10 = 7009,
    Testing_11 = 7010,
    Testing_12 = 7011,
    Testing_13 = 7012,

    CrystalLeft = 7013,
    CrystalRight = 7014,
    //Dummy
    DummyChar = 10001,
    DummyMinion = 10002
    #endregion

}

public enum WavePhaseType
{
    Combat,
    Event,
}

public enum InputAxisType
{
    Left_Move_Horizontal,
    Right_Move_Horizontal,
    Left_Move_Vertical,
    Right_Move_Vertical
}

public enum InputActionType
{
    None = 0,
    Weak,
    Strong,
    Skill1,
    Skill2,
    Skill3,
    Defend,
    Defend_Stop,
    Move_Up,
    Move_Down,
    Move_Left,
    Move_Right,
}


public enum InputButtonType
{
    A,
    B,
    X,
    Y,
    Left,
    Right,
    Up,
    Down,
    ZL,
    L,
    ZR,
    R,
    Plus,
    Minus,
    Home,
    Capture,
    Left_SL,
    Right_SL,
    Left_SR,
    Right_SR,
    Left_Stick,
    Right_Stick,
    Left_Move_Horizontal,
    Right_Move_Horizontal,
    Left_Move_Vertical,
    Right_Move_Vertical,
    KeyboardDown,
    KeyboardUp,
    KeyboardRight, 
    KeyboardLeft,

    //MENU NAV ACTIONS:
    Menu_Vertical = 1000,
    Menu_Horizontal = 1001,
    Menu_Up = 1002,
    Menu_Down = 1003,
    Menu_Left = 1004,
    Menu_Right = 1005,
    Menu_Start = 1006,
    Menu_Back = 1007,
    Menu_Confirm = 1008,
    Menu_Primary_Action = 1009,
    Menu_Secondary_Action = 1010,
    Menu_Tab_Primary_Left = 1011,
    Menu_Tab_Primary_Right = 1012,
    Menu_Tab_Secondary_Left = 1013,
    Menu_Tab_Secondary_Right = 1014,
    Menu_Exit = 1015,
    Menu_Confirm_Mouse = 1016,
    Menu_Skip = 1017,

    None = 99999,
}

public enum ActionCategoryType
{
    previous = 0,
    mapEnabler_Game = 1,
    mapEnabler_Menu = 2,
}


public enum WaveEventCheckType
{
    CharStatsCheckInPerc,
    CharDied,
    KillsNumber

}


public enum SpecialAttackStatus
{
    None,
    Start,
    Move,
    Stop
}

public enum DeathProcessStage
{
    None,
    Start,
    LoopDying,
    End
}

public enum AudioSourceType
{
    Any,
    Music,
    Ambience,
    Ui,
    Game,
}

public enum AudioBus
{
    Music = 1000,
    LowPrio = 0,
    MidPrio = 50,
    HighPrio = 100,
    NonSilenced = 500,
}

public enum MovementCurveType
{
    Space_Time,
    Speed_Time
}

public enum MenuNavigationType
{
    Unassigned = 0,
    None = 0,
    Relative = 1,
    Cursor = 2,
    DirectButton = 3,
    PlayerNavBox = 4,
    MouseInput = 5,
}

public enum LevelType
{
    Novice,
    Defiant,
    Heroine,
    Goddess
}

public enum PowerUpColorTypes
{
    White = 0,
    Red = 1,
    Blue = 2,
    Purple = 3,
    Green = 4,
    Orange = 5,
    Black = 6,
    DarkRed = 7,
    DarkBlue = 8,
    DarkPurple = 9,
    DarkGreen = 10,
    DarkOrange = 11,
}

public enum EncounterState
{
    Hidden,
    Encountered,
    Recruited
}

public enum EncounterStateMinion
{
    Hidden,
    Encountered_Innitial,
    Encountered
}



public enum MaskTypes
{
    None = 0,
    Mask_Tikaka = 1,
    Mask_MainFrame = 2,
    Mask_Kin = 3,
    Mask_Octress = 4,
    Mask_Anova = 5,
    Mask_Kala = 6,
    Mask_Geisha = 7,
    Mask_Xbox = 8,
    Mask_GizaAndJua = 9,
    Stage9 = 10,
}

public enum ComboType
{
    None,
    Attack,
    Defence,
    Kill,
}

public enum StageType
{
    Story = 0,
    Pvp = 1,
    Arena = 2,
}
public enum StageUnlockType
{
    locked,
    unlocking,
    unlocked,
};


public enum OptionBoxAnimType
{
    Active,
    AlreadySelected,
    Hidden
}


public enum CameraMovementType
{
    OnWorldPosition,
    OnCharacter,
    OnPlayer,
    OnTile
}

public enum CharacterEventTriggerType
{
    None = 0,
    Health = 1,
    Forme = 2,
    Death = 3,

    //CHAREVENT SHIT
    CharEvent_HasHappened = 100,
}

public enum CharacterEventActionType
{
    None,
    Forme_Change,
    Health_Set,
}

public enum StageNameType
{
    NotAssigned = 0,
    Stage00_CleasTemple = 1,
    Stage01_Tohora = 2,
    Stage02_TheBurg = 3,
    Stage03_ForestOfKin = 4,
    Stage04_RhodaCoast = 5,
    Stage05_AscensoMountains = 6,
    Stage06_DaikiniPeaks = 7,
    Stage07_NOTINUSE = 8,
    Stage08_MaidenShrine = 9,
    Stage09_MukuruCaves = 10,
    Any = 1000,
}

public enum WaveGen_RarityType
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    VeryRare = 3,
    ExtremelyRare = 4,
}

public enum WaveGen_EncounterType
{
    NotAssigned = 0,
    Minion = 1,
    Recruitable = 2,
    Boss = 3,
    Any = 1000,
}

public enum CharacterEvolutionStatType
{
    NONE = 0,

    HP = 1,
    HPRegen = 2,

    Armour = 3,
    ArmourShieldRegen = 4,

    Ether = 5,
    EtherRegen = 12,

    SpeedMovement = 6,
    Agility = 9,

    SigilDropBonus = 7,
    CriticalChances = 8,

    DamageWeak = 10,
    DamageStrong = 11,
}