using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class AdjustableStat
{
    public float statValue;
    protected List<StatAdjuster> statAjusters = new List<StatAdjuster>();
    public int statAdjusterCount
    {
        get
        {
            RefreshAdjustments();
            return statAjusters.Count;
        }
    }
    public float Val
    {
        get
        {
            return StatAdjusting.AdjustStatMultiple(statAjusters, statValue, out statAjusters);
        }
        set
        {
            statValue = value;
        }
    }

    public void RefreshAdjustments()
    {
        StatAdjusting.AdjustStatMultiple(statAjusters, statValue, out statAjusters);
    }

    public string AddAdjustment(StatAdjuster adjustment)
    {
        statAjusters.Add(adjustment);
        return adjustment.ID;
    }

    public bool RemoveAdjustment(string saID)
    {
        StatAdjuster toRemove = statAjusters.Where(r => r.ID == saID).FirstOrDefault();
        if (toRemove == null) return false;
        return RemoveAdjustment(toRemove);
    }

    public bool RemoveAdjustment(StatAdjuster adjustment)
    {
        if (!statAjusters.Contains(adjustment)) return false;
        statAjusters.Remove(adjustment);
        return true;
    }

    public AdjustableStat()
    {
        statValue = 0f;
        statAjusters = new List<StatAdjuster>();
    }

    public AdjustableStat(float _value)
    {
        statValue = _value;
        statAjusters = new List<StatAdjuster>();
    }
}

[System.Serializable]
public class StatAdjuster
{
    protected string uniqueID = "";
    public string ID
    {
        get
        {
            if(uniqueID == "")
            {
                uniqueID = "saID#" + Random.Range(0, 9999999999).ToString();
            }
            return uniqueID;
        }
    }

    [Header("Change")]
    [SerializeField] protected Operation operation = Operation.None;
    [SerializeField] protected float value = 0f;

    [Header("Timing")]
    public bool ApplyIndefinitely = false;
    [ConditionalField("ApplyIndefinitely", true)][SerializeField] protected float duration = 0f;
    protected float startTime = 0f;
    protected bool timeUnscaled = false;

    public StatAdjuster(Operation op, float val, float dur, bool applyForever = false, bool useUnscaledTime = false)
    {
        timeUnscaled = useUnscaledTime;
        operation = op;
        value = val;
        duration = dur;
        startTime = timeUnscaled ? Time.unscaledTime : Time.time;
        ApplyIndefinitely = applyForever;
    }

    public bool ApplyOperation(float inVal, out float outVal)
    {
        if (((timeUnscaled ? Time.unscaledTime : Time.time) - startTime > duration && !ApplyIndefinitely) || operation == Operation.None)
        {
            outVal = inVal;
            return false;
        }

        outVal = operation == Operation.Addition ? inVal + value : inVal * value;

        return true;
    }
}


public class StatAdjusting
{
    public static float AdjustStatMultiple(List<StatAdjuster> adjusters, float value, out List<StatAdjuster> adjusterList)
    {
        foreach (StatAdjuster adjuster in adjusters.ToArray())
        {
            if (!adjuster.ApplyOperation(value, out value))
            {
                adjusters.Remove(adjuster);
            }
        }

        adjusterList = adjusters;

        return value;
    }
}

public enum Operation
{
    None = 0,
    Addition = 1,
    Multiplication = 2,
}