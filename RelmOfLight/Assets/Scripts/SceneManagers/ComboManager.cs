using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UI;
using System.Linq;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    public delegate void FungusEventTriggerAction(string blockName);
    public static event FungusEventTriggerAction OnFungusEventTrigger;

    public delegate void ComboTriggered();
    public static event ComboTriggered OnComboTriggered;

    [Header("Combo Type Details")]
    public ComboTypeInfoClass[] comboTypeInformation = new ComboTypeInfoClass[3];
    public ComboTypeInfoClass GetComboTypeInfo(ComboType comboType)
    {
        return comboTypeInformation.Where(r => r.comboType == comboType).FirstOrDefault();
    }

    [Header("Groupings")]
    [SerializeField] protected bool useComboGroupings = false;
    [SerializeField] protected List<ComboGroupInspectorClass> comboGroups = new List<ComboGroupInspectorClass>();


    //Runtime
    [HideInInspector] public List<PlayerComboInfoGroupClass> comboInfo = new List<PlayerComboInfoGroupClass>();


    protected  Dictionary<ComboType, int> comboHighScores = new Dictionary<ComboType, int>
    {
        { ComboType.Attack, 0 },
        { ComboType.Defence, 0 },
        { ComboType.Kill, 0 },
    };

    public int GetHighestAchievedCombo(ComboType type)
    {
        int high = comboHighScores[type];
        foreach (PlayerComboInfoGroupClass combI in comboInfo)
        {
            if(high < combI.comboInfo[type].ComboCount)
            {
                high = combI.comboInfo[type].ComboCount;
            }
        }
        comboHighScores[type] = high;
        return high;
    }


    private void Awake()
    {
        Instance = this;
    }

    protected void TriggerComboForPlayer(int playerIndex, ComboType combo, bool hit, Vector3 displayTarget = new Vector3())
    {
        StartCoroutine(TriggerComboForPlayer_Co(playerIndex, combo, hit, displayTarget));
    }

    protected IEnumerator TriggerComboForPlayer_Co(int playerIndex, ComboType combo, bool hit, Vector3 displayTarget = new Vector3())
    {
        int comboNum = comboInfo.Where(r => r.ContainsPlayer(playerIndex)).FirstOrDefault().TriggerCombo(combo, hit);
        OnComboTriggered?.Invoke();
        if (displayTarget == new Vector3())
        {
            yield break;
        }

        ComboTypeInfoClass comboTypeInfo = GetComboTypeInfo(combo);

        string text = "";
        Color displayColor = Color.white;
        bool finalColorFound = false;
        int lastThresholdVal = 0;
        for (int i = 0; i < comboTypeInfo.comboThresholds.Length; i++)
        {
            if (comboTypeInfo.comboThresholds[i].val <= comboNum)
            {
                lastThresholdVal = comboTypeInfo.comboThresholds[i].val;
                displayColor = comboTypeInfo.comboThresholds[i].color;
            }
            else if (!finalColorFound)
            {
                finalColorFound = true;
                displayColor = Color.Lerp(displayColor, comboTypeInfo.comboThresholds[i].color, (float)(comboNum - lastThresholdVal) / (float)(comboTypeInfo.comboThresholds[i].val - lastThresholdVal));
            }
            if (comboNum == comboTypeInfo.comboThresholds[i].val)
            {
                text = comboTypeInfo.comboThresholds[i].Text;
                if(comboTypeInfo.comboThresholds[i].fungusBlockToFire != "")
                {
                    if (comboTypeInfo.comboThresholds[i].fireBlockOnceOff) comboTypeInfo.comboThresholds[i].blockFired = true;
                    if (!comboTypeInfo.comboThresholds[i].blockFired) OnFungusEventTrigger?.Invoke(comboTypeInfo.comboThresholds[i].fungusBlockToFire);
                }
            }
        }

        float animTime;
        UIBattleFieldManager.Instance.DisplayComboStyleSplasher(comboTypeInfo.displayCount && comboTypeInfo.startingNumber <= comboNum ? comboTypeInfo.prefix + comboNum.ToString() + comboTypeInfo.suffix : "", displayTarget, 1f + Mathf.Clamp(((float)combo) / (float)comboTypeInfo.maxIntensityCombo, 0f, 1f), displayColor, false, out animTime);

        if (text == "") yield break;

        yield return new WaitForSecondsRealtime(animTime * 0.6f);

        UIBattleFieldManager.Instance.DisplayComboStyleSplasher(text, displayTarget + new Vector3(0f,0.3f,0f), 1f + Mathf.Clamp(((float)combo) / (float)comboTypeInfo.maxIntensityCombo, 0f, 1f), displayColor, true, out animTime);
    }


    public void TriggerComboInfoClass(ComboInfoClass combo)
    {
        StartCoroutine(combo.comboCountDown);
    }




    private void OnValidate()
    {
        foreach(ComboTypeInfoClass info in comboTypeInformation)
        {
            info.Name = info.comboType.ToString();
            foreach (ComboThresholdInfoClass combT in info.comboThresholds)
            {
                combT.Name = combT.val.ToString() + (combT.texts.Length == 0 ? " No Texts. Will return ''" : "");
            }
        }

    }
}


[System.Serializable]
public class ComboGroupInspectorClass
{
    public string Name;
    public int[] playerIndexes = new int[0];
}

[System.Serializable]
public class ComboTypeInfoClass
{
    [HideInInspector] public string Name;
    public ComboType comboType;
    [Tooltip("The point at which the combo reaches its maximum size when displayed, note that the combo CAN continue increasing past this point")]
    public int maxIntensityCombo = 25;
    [Tooltip("The amount of time a combo must be retriggered (shot landing, shield used, etc.) in to avoid it resetting to 0")]
    public float resetTimer = 5f;
    public bool displayCount = true;
    [ConditionalField("displayCount")] public string prefix = "x";
    [ConditionalField("displayCount")] public string suffix = "";
    [ConditionalField("displayCount")] public int startingNumber = 1;
    public ComboThresholdInfoClass[] comboThresholds = new ComboThresholdInfoClass[0];

}


[System.Serializable]
public class ComboThresholdInfoClass
{
    [HideInInspector] public string Name;
    public int val;
    public string[] texts;
    public Color color;
    public string fungusBlockToFire = "";
    public bool fireBlockOnceOff = false;
    [HideInInspector] public bool blockFired = false;

    public string Text
    {
        get
        {
            if (texts.Length == 0) return "";
            return texts[Random.Range(0, texts.Length)];
        }
    }
}

[System.Serializable]
public class PlayerComboInfoGroupClass
{
    public List<int> playerIndexes = new List<int>();
    public Dictionary<ComboType, ComboInfoClass> comboInfo = new Dictionary<ComboType, ComboInfoClass>
    {
        { ComboType.Attack, new ComboInfoClass(ComboManager.Instance.GetComboTypeInfo(ComboType.Attack).resetTimer) },
        { ComboType.Defence, new ComboInfoClass(ComboManager.Instance.GetComboTypeInfo(ComboType.Defence).resetTimer) },
        { ComboType.Kill, new ComboInfoClass(ComboManager.Instance.GetComboTypeInfo(ComboType.Kill).resetTimer) },
    };

    public PlayerComboInfoGroupClass(int[] playerIndex)
    {
        playerIndexes = playerIndex.ToList();
        comboInfo = new Dictionary<ComboType, ComboInfoClass>
        {
            { ComboType.Attack, new ComboInfoClass(ComboManager.Instance.GetComboTypeInfo(ComboType.Attack).resetTimer) },
            { ComboType.Defence, new ComboInfoClass(ComboManager.Instance.GetComboTypeInfo(ComboType.Defence).resetTimer) },
            { ComboType.Kill, new ComboInfoClass(ComboManager.Instance.GetComboTypeInfo(ComboType.Kill).resetTimer) },
        };
    }

    public bool ContainsPlayer(int playerIndex)
    {
        if (playerIndexes.Contains(playerIndex)) return true;
        return false;
    }

    public int TriggerCombo(ComboType combo, bool hit)
    {
        comboInfo[combo].TriggerCombo(hit);
        return comboInfo[combo].ComboCount;
    }
}

[System.Serializable]
public class ComboInfoClass
{
    protected int comboCount = 0;
    public int maxComboAchieved = 0;
    public int ComboCount
    {
        get
        {
            return comboCount;
        }
        set
        { 
            comboCount = value;
            maxComboAchieved = comboCount > maxComboAchieved ? comboCount : maxComboAchieved;
        }
    }
    protected float resetTime = 0f;
    public float timeRemaining = 0f;
    public IEnumerator comboCountDown = null;

    public ComboInfoClass(float resetTiming)
    {
        resetTime = resetTiming;
        ComboCount = 0;
        timeRemaining = 0f;
        comboCountDown = null;
    }


    public void TriggerCombo(bool hit)
    {
        bool timeBelowZero = timeRemaining <= 0f;
        ComboCount = hit ? ComboCount + 1 : 0;
       // Debug.Log("<b>" + ComboCount.ToString() + "</b> with seconds remaining: " + timeRemaining.ToString());
        timeRemaining = hit ? resetTime : 0f;
        if (timeBelowZero && hit)
        {
            comboCountDown = CountComboDown();
            ComboManager.Instance.TriggerComboInfoClass(this);
        }
    }

    IEnumerator CountComboDown()
    {
        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.unscaledDeltaTime;
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets);
        }
        ComboCount = 0;
    }
}