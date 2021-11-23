using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlaytraGamesLtd;

[System.Serializable]
public class AttackSequence
{
    [HideInInspector] public string Name = "";
    public bool rename = true;
    [ConditionalField("rename")] public string newName = "";
    public void GenerateName()
    {
         Name = rename ? newName : groupedAttacks.Count.ToString() + " grouped attack" + (groupedAttacks.Count > 1 ? "s" : "");
    }


    public List<ScriptableObjectAttackBase> groupedAttacks = new List<ScriptableObjectAttackBase>();

    public TriggerType triggerType = TriggerType.TriggerAndReset;
    [HideInInspector] public bool triggered = false;

    public StatsCheckType StatToCheck = StatsCheckType.Health;
    protected bool noCheck
    {
        get
        {
            if(StatToCheck == StatsCheckType.None && triggerType == TriggerType.TriggerAndReset)
            {
                Debug.LogError("Cannot have no check and a 'trigger and reset' triggertype, defaulting to a 'TriggerOnceOff'");
                triggerType = TriggerType.TriggerOnceOff;
            }
            return StatToCheck == StatsCheckType.None;
        }
    }
    [ConditionalField("StatToCheck", true, StatsCheckType.None)] public ValueCheckerType ValueChecker = ValueCheckerType.LessThan;
    [ConditionalField("ValueChecker", true, ValueCheckerType.Between)] public float PercToCheck = 100f;
    [ConditionalField("ValueChecker", false, ValueCheckerType.Between)] public Vector2 InBetween = new Vector2(60,40);
    [Range(0, 100)] public int Chances = 100;
    protected bool chanceChecked = false;

    protected float curCheckValPerc = 0f;

    protected bool PercCompare
    {
        get
        {
            if (noCheck)
            {
                return true;
            }

            bool value = false;

            switch (ValueChecker)
            {
                case ValueCheckerType.LessThan:
                    value = PercToCheck > curCheckValPerc;
                    break;
                case ValueCheckerType.EqualTo:
                    value = PercToCheck == curCheckValPerc;
                    break;
                case ValueCheckerType.MoreThan:
                    value = PercToCheck < curCheckValPerc;
                    break;
                case ValueCheckerType.Between:
                    value = InBetween.x <= curCheckValPerc && InBetween.x >= curCheckValPerc;
                    break;
            }

            //Reset triggered value if the value check fails
            if (!value)
            {
                triggered = triggerType == TriggerType.TriggerAndReset ? false : triggered;
                chanceChecked = triggerType == TriggerType.TriggerAndReset ? false : chanceChecked;
            }
            return value;
        }
    }

    protected bool PassesChanceCheck
    {
        get
        {
            if (chanceChecked) return false;
            chanceChecked = true;
            return Random.Range(0, 100) < Chances;
        }
    }

    //Reset the trigger if the value has changed in the opposite direction and also return whether or not it can be triggered
    public bool CheckTrigger(CharacterInfoScript charInfo)
    {
        switch (StatToCheck)
        {
            case StatsCheckType.None:
                break;
            case StatsCheckType.Health:
                curCheckValPerc = charInfo.HealthPerc;
                break;
            case StatsCheckType.MovementSpeed:
                curCheckValPerc = charInfo.SpeedStats.MovementSpeed;
                break;
            case StatsCheckType.BaseSpeed:
                Debug.LogError("Cannot Compare this Value");
                break;
            case StatsCheckType.TeamTotalHpPerc:
                Debug.LogError("Cannot Compare this Value");
                break;
        }

        if (PercCompare)
        {
            if (PassesChanceCheck && !triggered)
            {
                return true;
            }
        }

        return false;
    }

    public ScriptableObjectAttackBase[] GetAttackSequence()
    {
        triggered = true;
        return groupedAttacks.ToArray();
    }

}
