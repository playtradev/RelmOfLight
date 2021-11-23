using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

public class BattleInfoManagerScript : MonoBehaviour
{
    public static BattleInfoManagerScript Instance;
   
    public List<TeamSideInformationClass> PlayerBattleInfo = new List<TeamSideInformationClass>();


    private void Awake()
    {
        Instance = this;
    }

    private void OnValidate()
    {
        foreach (TeamSideInformationClass item in PlayerBattleInfo)
        {
            item.Name = item.CharacterName.ToString().Split('_').Last();
        }
    }

}



[System.Serializable]
public class TeamSideInformationClass
{
    [HideInInspector] public string Name;

    public string CharName
    {
        get
        {
            return CharacterName.ToString().Split('_').Last();
        }
    }

    public CharacterNameType CharacterName;
    public TeamSideType Team;
    public List<ControllerType> PlayerController = new List<ControllerType>();

    public List<CharacterActionType> CharActionlist = new List<CharacterActionType>();
    public CharacterSelectionType CharacterSelection;

    public bool OverrideMask = false;
    [ConditionalField("OverrideMask", false)] public ScriptableObjectSkillMask Mask;

  
    public FacingType DefaultFacing
    {
        get
        {
            return Team == TeamSideType.LeftSideTeam ? FacingType.Right : FacingType.Left;
        }
    }


    public TeamSideInformationClass()
    {
        CharActionlist = new List<CharacterActionType> {
            CharacterActionType.Defence,
            CharacterActionType.Move,
            CharacterActionType.Weak,
            CharacterActionType.Strong,
            CharacterActionType.MoveUp,
            CharacterActionType.MoveDown,
            CharacterActionType.MoveLeft,
            CharacterActionType.MoveRight,
            CharacterActionType.SwitchCharacter,
            CharacterActionType.Skill1,
            CharacterActionType.Skill2,
            CharacterActionType.Skill3,
        };
    }

    public TeamSideInformationClass(List<ControllerType> playercontroller, CharacterNameType charactername, TeamSideType side, LevelType charaterlevel)
    {
        PlayerController = playercontroller;
        Team = side;
        CharacterName = charactername;
        CharActionlist = new List<CharacterActionType> {
            CharacterActionType.Defence,
            CharacterActionType.Move,
            CharacterActionType.Weak,
            CharacterActionType.Strong,
            CharacterActionType.MoveUp,
            CharacterActionType.MoveDown,
            CharacterActionType.MoveLeft,
            CharacterActionType.MoveRight,
            CharacterActionType.SwitchCharacter,
            CharacterActionType.Skill1,
            CharacterActionType.Skill2,
            CharacterActionType.Skill3,
        };
       // CharaterLevel = charaterlevel;
    }

}


[System.Serializable]
public class ElementalResistenceClass
{

    public ElementalWeaknessType ElementalWeakness
    {
        get
        {
            return _ElementalWeakness;
        }
        set
        {
            _ElementalWeakness = value;
        }
    }

    public ElementalType Elemental;
    public ElementalWeaknessType _ElementalWeakness;

    public ElementalResistenceClass()
    {

    }
    public ElementalResistenceClass(ElementalType elemental, ElementalWeaknessType elementalWeakness)
    {
        Elemental = elemental;
        ElementalWeakness = elementalWeakness;
    }
}

[System.Serializable]
public class CharactersRelationshipClass
{
    public RelationshipType Relationship;
    public CharacterNameType CharacterName;

    public CharactersRelationshipClass()
    {
    }

    public CharactersRelationshipClass(RelationshipType relationship, CharacterNameType characterName)
    {
        Relationship = relationship;
        CharacterName = characterName;
    }
}

