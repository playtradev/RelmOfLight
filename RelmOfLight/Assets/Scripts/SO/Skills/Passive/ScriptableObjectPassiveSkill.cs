using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PassiveSkill")]
public class ScriptableObjectPassiveSkill : ScriptableObjectSkillBase
{
    //[SerializeField, HideInInspector] protected int Level;
    //public override int MinLevel { get => Level; set => Level = value; }
    //public override int MaxLevel { get => 10; }

    [SerializeField] public PassiveSkillClass[] PassiveSkills;
}


[System.Serializable]
public class PassiveSkillClass
{
    public string Name;
    public PassiveSkillTargetType PassiveSkillTarget;
    public PassiveSkillType PassiveSkill;
    public PassiveSkillsValueType ValueType;
    public float Value;
    public bool BoolValue = false;
    public ArmourType ArmourT;
    public bool BaseCurrentValue = true;
    public ElementalType Elemental;
    public List<BuffDebuffStatsType> BuffDebuffImmunities = new List<BuffDebuffStatsType>();
    public BulletTypeModifierClass BulletModifier;
    public bool Show = false;

    public PassiveSkillClass()
    {

    }
}
