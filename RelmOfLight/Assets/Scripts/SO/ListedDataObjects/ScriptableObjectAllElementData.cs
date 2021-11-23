using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

[CreateAssetMenu(fileName = "New Element Data Profile", menuName = "ScriptableObjects/Elements/ElementDataProfile")]
public class ScriptableObjectAllElementData : ScriptableObject
{
    public ScriptableObjectElementData[] ElementDatas = new ScriptableObjectElementData[0];
    public ScriptableObjectElementData GetElementData(ElementalType element) => ElementDatas.Where(r => r.Type == element).FirstOrDefault();
    public ScriptableObjectElementData[] GetElementDatas(ElementalType[] elements)
    {
        List<ScriptableObjectElementData> res = new List<ScriptableObjectElementData>();
        ScriptableObjectElementData curElementData;
        foreach (ElementalType element in elements)
        {
            curElementData = GetElementData(element);
            if(curElementData != null && res.Where(r => r.Type == element).FirstOrDefault() == null)
            {
                res.Add(curElementData);
            }
        }
        return res.ToArray();
    }

    //Gets elements that have a relation with the input, NOTE: DOES NOT RETURN THE INPUT ELEMENT'S RELATIONS
    public ScriptableObjectElementData[] GetRelatingElements(ElementalType[] types, out ScriptableObjectElementData[] strongAgainstInput, out ScriptableObjectElementData[] weakAgainstInput)
    {
        List<ScriptableObjectElementData> weakElements = new List<ScriptableObjectElementData>();
        List<ScriptableObjectElementData> strongElements = new List<ScriptableObjectElementData>();

        foreach (ElementalType type in types)
        {
            foreach (ScriptableObjectElementData data in ElementDatas)
            {
                ElementDataRelationInfoClass validRelation = data.Relations.Where(r => r.Element == type).FirstOrDefault();
                if (validRelation != null && strongElements.Where(r => r.Type == validRelation.Element).FirstOrDefault() == null && weakElements.Where(r => r.Type == validRelation.Element).FirstOrDefault() == null)
                {
                    if(validRelation.AttackDamageMultiplier > 1f)
                    {
                        weakElements.Add(data);
                    }
                    else
                    {
                        strongElements.Add(data);
                    }
                }
            }
        }

        strongAgainstInput = weakElements.ToArray();
        weakAgainstInput = strongElements.ToArray();
        return weakAgainstInput.Concat(strongAgainstInput).ToArray();
    }


    //public bool TestCalculations = false;
    //[ConditionalField("TestCalculations")] public ElementalType[] testAttacker = new ElementalType[0];
    //[ConditionalField("TestCalculations")] public ElementalType[] testDefender = new ElementalType[0];
    //[ConditionalField("TestCalculations")] public bool testCompound = false;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.KeypadPlus) && TestCalculations) Debug.LogError("ELEMENTAL DAMAGE MULTIPLIER TEST: " + AttackDamageMultiplier(testAttacker, testDefender, testCompound).ToString() + "x");    
    //}


    public ElementDataRelationInfoClass[] RelationsForElements(ElementalType[] elements)
    {
        List<ElementDataRelationInfoClass> elementsRelations = new List<ElementDataRelationInfoClass>();

        foreach (ElementalType element in elements)
        {
            ScriptableObjectElementData elementInfo = ElementDatas.Where(r => r.Type == element).FirstOrDefault();
            foreach (ElementDataRelationInfoClass elementRelation in elementInfo.Relations)
            {
                if(elementsRelations.Where(r => r.Element == elementRelation.Element).FirstOrDefault() == null)
                {
                    elementsRelations.Add(elementRelation);
                }
            }
        }

        return elementsRelations.ToArray();
    }

    public float AttackDamageMultiplier(ElementalType[] attackerElements, ElementalType[] defenderElements, bool compound = false)
    {
        float highestIncrease = 1f;
        float lowestDecrease = 1f;
        float overallMultiplier = 1f;

        foreach (ElementDataRelationInfoClass AttackerRelation in RelationsForElements(attackerElements))
        {
            if (defenderElements.Contains(AttackerRelation.Element))
            {
                if (compound)
                {
                    overallMultiplier *= AttackerRelation.AttackDamageMultiplier;
                }

                if(AttackerRelation.AttackDamageMultiplier > 1f) highestIncrease = highestIncrease > AttackerRelation.AttackDamageMultiplier ? highestIncrease : AttackerRelation.AttackDamageMultiplier;
                else if(AttackerRelation.AttackDamageMultiplier < 1f) lowestDecrease = lowestDecrease < AttackerRelation.AttackDamageMultiplier ? lowestDecrease : AttackerRelation.AttackDamageMultiplier;
            }
        }

        if (!compound)
        {
            overallMultiplier = highestIncrease * lowestDecrease;
        }

        return overallMultiplier;
    }

    public Dictionary<ElementalType, Color> _ElementColors = null;
    public Dictionary<ElementalType, Color> ElementColors
    {
        get
        {
            if(_ElementColors == null)
            {
                _ElementColors = new Dictionary<ElementalType, Color>();
                for (int i = 0; i < ElementDatas.Length; i++)
                {
                    if (ElementDatas != null)
                        _ElementColors.Add(ElementDatas[i].Type, ElementDatas[i].color);
                }
            }
            return _ElementColors;
        }
    }
}
