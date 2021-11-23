using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharactersEvolution")]
public class ScriptableObjectCharactersEvolution : ScriptableObject
{
    /// <summary>
    /// Only used in some parts of the code, needs full implementation with the old system
    /// </summary>
    public int NumberOfLevels = 10;
    public int MaxLevelDiamondPatternLength => (NumberOfLevels - 1) * LevelUpElementTypes.Length;
    ElementalType[] _LevelUpElementTypes = new ElementalType[0];
    public ElementalType[] LevelUpElementTypes
    {
        get
        {
            if (_LevelUpElementTypes.Length == 0)
            {
                List<ElementalType> thaElements = new List<ElementalType>();
                System.Array values = System.Enum.GetValues(typeof(ElementalType));
                for (int i = 0; i < values.Length; i++)
                {
                    if ((ElementalType)(values.GetValue(i)) == ElementalType.Neutral)
                        continue;
                    thaElements.Add((ElementalType)(values.GetValue(i)));
                }
                _LevelUpElementTypes = thaElements.ToArray();
            }
            return _LevelUpElementTypes;
        }
    }

    public ElementalType CharEvoStatType_To_ElementalType(CharacterEvolutionStatType stat)
    {
        if (stat == CharacterEvolutionStatType.HP || stat == CharacterEvolutionStatType.HPRegen)
            return ElementalType.Earth;
        if (stat == CharacterEvolutionStatType.Armour || stat == CharacterEvolutionStatType.ArmourShieldRegen)
            return ElementalType.Water;
        if (stat == CharacterEvolutionStatType.DamageWeak || stat == CharacterEvolutionStatType.DamageStrong)
            return ElementalType.Fire;
        if (stat == CharacterEvolutionStatType.Ether || stat == CharacterEvolutionStatType.EtherRegen)
            return ElementalType.Light;
        if (stat == CharacterEvolutionStatType.SpeedMovement || stat == CharacterEvolutionStatType.Agility)
            return ElementalType.Air;
        if (stat == CharacterEvolutionStatType.CriticalChances || stat == CharacterEvolutionStatType.SigilDropBonus)
            return ElementalType.Dark;
        Debug.LogError("WASN'T ABLE TO MATCH STAT WITH AN ELEMENT");
        return ElementalType.Neutral;
    }

    [Tooltip("How significant each combo length is towards the player character's fulfillment progress")] public ComboPointWeightPairings[] ComboPointWeightPairs = new ComboPointWeightPairings[] { new ComboPointWeightPairings(2, 1), new ComboPointWeightPairings(3, 3) };
    [System.Serializable]
    public struct ComboPointWeightPairings
    {
        public int ComboLength;
        public int PointWeight;
        public ComboPointWeightPairings(int ComboLength, int PointWeight)
        {
            this.ComboLength = ComboLength;
            this.PointWeight = PointWeight;
        }
    }
    [Tooltip("Set the general multiplier curve for attributes based on how well they fulfilled their comboing of attribute level ups. THIS SHOULD BE EXPONENTIAL TO REWARD MIN MAXING.\n\nSTART AT 0, AND TREAT IT AS 1 (0 -> 0.1 is the same as x1 -> x1.1)")] public AnimationCurve ComboFulfillmentMultiplierCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    /// <summary>
    /// Get the percentage of the characters progression to achieving a perfectly combo'd character between 0-1
    /// </summary>
    public float GetComboFulfillmentProgress(CharacterEvolutionStatType stat, Vector2Int[] levelDiamondPatternCombos) => GetComboFulfillmentProgress(CharEvoStatType_To_ElementalType(stat), levelDiamondPatternCombos);
    /// <summary>
    /// Get the percentage of the characters progression to achieving a perfectly combo'd character between 0-1
    /// </summary>
    public float GetComboFulfillmentProgress(ElementalType element, Vector2Int[] levelDiamondPatternCombos)
    {
        float res = 0f;
        ComboPointWeightPairings temp_CWP;
        for (int i = 0; i < levelDiamondPatternCombos.Length; i++)
        {
            if (levelDiamondPatternCombos[i].x != (int)element) continue;
            temp_CWP = ComboPointWeightPairs.GridFight_Where_Struct_FirstOrDefault(r => r.ComboLength == levelDiamondPatternCombos[i].y);
            res += temp_CWP.ComboLength;
        }
        return Mathf.Min(res / MaxLevelFulfillmentPts, 1f);
    }
    /// <summary>
    /// The long fucking line here just means "the value if they player got all levels of a stat in perfect combination eg. 9 levels -> 3x3 combos"
    /// </summary>
    public float MaxLevelFulfillmentPts 
    {
        get
        {
            if(_MaxLevelFulfillmentPts == -1f)
                _MaxLevelFulfillmentPts = (((float)(NumberOfLevels) - 1f) / (float)(ComboPointWeightPairs.OrderByDescending(r => r.ComboLength).First().ComboLength)) * ComboPointWeightPairs.OrderByDescending(r => r.ComboLength).First().PointWeight;
            return _MaxLevelFulfillmentPts;
        }
    }
    float _MaxLevelFulfillmentPts = -1f;


    //COST____COST____COST____COST____COST____COST____COST____COST____COST
    public float[] CostMultiplier = new float[]
    {
        1,1,1,1,1,1,1,1,1,1
    };

    public EvolutionCostBaseSettingsClass GetEvolutionCostBaseSettingsFor(ElementalType elementCurrencyTye)
    {
        switch (elementCurrencyTye)
        {
            case ElementalType.Earth:
                return HP;
            case ElementalType.Water:
                return Armour;
            case ElementalType.Fire:
                return Damage;
            case ElementalType.Light:
                return Ether;
            case ElementalType.Air:
                return Speed;
            case ElementalType.Dark:
                return Grace;
            case ElementalType.Neutral:
                return Luck;
            default:
                Debug.LogError("Cannot get the cost for the element currency type: " + elementCurrencyTye.ToString() + "... ABORTING");
                return null;
        }
    }
    public EvolutionCostBaseSettingsClass HP;
    public EvolutionCostBaseSettingsClass Armour;
    public EvolutionCostBaseSettingsClass Ether;
    public EvolutionCostBaseSettingsClass Speed;
    public EvolutionCostBaseSettingsClass Luck;
    public EvolutionCostBaseSettingsClass Grace;
    public EvolutionCostBaseSettingsClass Damage;

    public int CostForLevel(ElementalType currency, int level, CharacterClassType classType) 
    {
        if (level < 2) return 0;

        EvolutionCostBaseSettingsClass evoCostBase = GetEvolutionCostBaseSettingsFor(currency);

        float cost = evoCostBase.BaseCost[(int)classType];

        for (int i = 1; i < level && i < CostMultiplier.Length; i++)
            cost *= CostMultiplier[i];

        return Mathf.FloorToInt(cost);
    }
    //

    //STATS EVOLUTION CURVE___STATS EVOLUTION CURVE___STATS EVOLUTION CURVE___STATS EVOLUTION CURVE
    public float[] BaseStaseCurveFor(CharacterEvolutionStatType type)
    {
        switch (type)
        {
            case CharacterEvolutionStatType.HP:
                return BaseHPStaseCurve;
            case CharacterEvolutionStatType.HPRegen:
                return BaseHPRegenStaseCurve;
            case CharacterEvolutionStatType.Armour:
                return BaseArmourStaseCurve;
            case CharacterEvolutionStatType.ArmourShieldRegen:
                return BaseArmourShieldRegenStaseCurve;
            case CharacterEvolutionStatType.Ether:
                return BaseEtherStaseCurve;
            case CharacterEvolutionStatType.EtherRegen:
                return BaseEtherRegenStaseCurve;
            case CharacterEvolutionStatType.SpeedMovement:
                return BaseSpeedMovementStaseCurve;
            case CharacterEvolutionStatType.SigilDropBonus :
                return BaseLuckSigilDropBonusStaseCurve;
            case CharacterEvolutionStatType.CriticalChances:
                return BaseLuckCCStaseCurve;
            case CharacterEvolutionStatType.Agility:
                return BaseSpeedAgilityStaseCurve;
            case CharacterEvolutionStatType.DamageWeak:
                return BaseDamageWeakStaseCurve;
            case CharacterEvolutionStatType.DamageStrong:
                return BaseDamageStrongStaseCurve;
            default:
                return new float[]
                {
                    100,1,1,1,1,1,1,1,1,1
                };
        }
    }
    public float[] BaseHPStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseHPRegenStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseArmourStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseArmourShieldRegenStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseEtherStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseSpeedMovementStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseLuckCCStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseLuckSigilDropBonusStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };

    public float[] BaseDamageWeakStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseEtherRegenStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseSpeedAgilityStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    public float[] BaseDamageStrongStaseCurve = new float[]
    {
        100,1,1,1,1,1,1,1,1,1
    };
    //

    //CHARACTER VARIANCES___CHARACTER VARIANCES___CHARACTER VARIANCES___CHARACTER VARIANCES
    public List<CharacterEvolutionClass> Characters = new List<CharacterEvolutionClass>();

    CharacterEvolutionClass charEvo;
    public void Set_StatValue_AtLevelForCharacter(ref float startingVal, CharacterEvolutionStatType statType, CharacterNameType charID, int level, Vector2Int[] comboProfile) => startingVal = StatValue_AtLevelForCharacter(startingVal, statType, charID, level, comboProfile);
    public void Set_StatValue_AtLevelForCharacter(ref Vector2 startingVal, CharacterEvolutionStatType statType, CharacterNameType charID, int level, Vector2Int[] comboProfile) => startingVal = StatValue_AtLevelForCharacter(startingVal, statType, charID, level, comboProfile);
    public float StatValue_AtLevelForCharacter(float startingVal, CharacterEvolutionStatType statType, CharacterNameType charID, int level, Vector2Int[] comboProfile) => StatValue_AtLevelForCharacter(Vector2.one * startingVal, statType, charID, level, comboProfile).x;
    public Vector2 StatValue_AtLevelForCharacter(Vector2 startingVal, CharacterEvolutionStatType statType, CharacterNameType charID, int level, Vector2Int[] comboProfile)
    {
        if (charEvo == null || charEvo.CharacterName != charID)
            charEvo = Characters.Where(r => r.CharacterName == charID).FirstOrDefault();
        if (charEvo == null)
            charEvo = new CharacterEvolutionClass();

        for (int i = 1; i < level; i++)
            startingVal += (startingVal / 100) * (BaseStaseCurveFor(statType)[i] + (BaseStaseCurveFor(statType)[i] / 100) * charEvo.LevelFor(statType)[i]);

        return startingVal * (1f + (ComboFulfillmentMultiplierCurve == null ? 0f : ComboFulfillmentMultiplierCurve.Evaluate(GetComboFulfillmentProgress(statType, comboProfile))));
    }
    //
 
}

[System.Serializable]
public class CharacterEvolutionClass
{
    public CharacterNameType CharacterName;
    
    public bool Show = false; //Inspector Stuff

    //LEVEL VARIANCES FROM THE DEFAULT EVO
    public int[] LevelFor(CharacterEvolutionStatType type)
    {
        switch (type)
        {
            case CharacterEvolutionStatType.HP:
                return HPLevel;
            case CharacterEvolutionStatType.HPRegen:
                return HPRegenLevel;
            case CharacterEvolutionStatType.Armour:
                return ArmourLevel;
            case CharacterEvolutionStatType.ArmourShieldRegen:
                return ShieldRegenLevel;
            case CharacterEvolutionStatType.Ether:
                return EtherLevel;
            case CharacterEvolutionStatType.EtherRegen:
                return EtherRegenLevel;
            case CharacterEvolutionStatType.SpeedMovement:
                return SpeedMovementLevel;
            case CharacterEvolutionStatType.SigilDropBonus :
                return SigilDropBonusLevel;
            case CharacterEvolutionStatType.CriticalChances:
                return CritCLevel;
            case CharacterEvolutionStatType.Agility:
                return AgilityLevel;
            case CharacterEvolutionStatType.DamageWeak:
                return DamageWeakLevel;
            case CharacterEvolutionStatType.DamageStrong:
                return DamageStrongLevel;
            default:
                return new int[]
                {
                    0,0,0,0,0,0,0,0,0,0
                };
        }
    }

    public int[] HPLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] ArmourLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] EtherLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] SpeedMovementLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] CritCLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] DamageWeakLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] HPRegenLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] ShieldRegenLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] EtherRegenLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] SigilDropBonusLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] AgilityLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    public int[] DamageStrongLevel = new int[]
    {
        0,0,0,0,0,0,0,0,0,0
    };
    //

    public CharacterEvolutionClass()
    {

    }
}

[System.Serializable]
public class EvolutionCostBaseSettingsClass
{
    public int[] BaseCost = new int[4];
}