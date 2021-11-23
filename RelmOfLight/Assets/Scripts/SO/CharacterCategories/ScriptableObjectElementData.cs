using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element Data", menuName = "ScriptableObjects/Elements/Data")]
public class ScriptableObjectElementData : ScriptableObject
{
    public ElementalType Type = ElementalType.Neutral;
    public Sprite Icon = null;
    public Sprite SigilIcon = null;
    //public Sprite Icon_Unbounded = null;
    //public Sprite Icon_Unbounded_Shaded = null;
    public Color color = Color.white;
    public ElementDataRelationInfoClass[] Relations = new ElementDataRelationInfoClass[0];

    private void OnValidate()
    {
        foreach (ElementDataRelationInfoClass relation in Relations) relation.OnValidate();
    }
}

[System.Serializable]
public class ElementDataRelationInfoClass
{
    [HideInInspector] public string Name = "";
    public ElementalType Element;
    [Range(0f, 3f)]public float AttackDamageMultiplier = 1f;

    public ElementDataRelationInfoClass()
    {
        AttackDamageMultiplier = 1f;
    }

    public void OnValidate()
    {
        AttackDamageMultiplier = Mathf.Round(AttackDamageMultiplier * 10f) / 10f;
        Name = Element.ToString() + "  ||  x" + AttackDamageMultiplier.ToString();
    }
}
