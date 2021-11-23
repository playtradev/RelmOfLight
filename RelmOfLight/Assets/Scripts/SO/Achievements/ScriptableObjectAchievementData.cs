using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Achievement Data", menuName = "ScriptableObjects/AchievementData")]
public class ScriptableObjectAchievementData : ScriptableObject
{
    public AchievementType AchievementT;

    public string DisplayName = "New Achievement";
    [TextArea(2,3)] public string Description = "";

    public Sprite Icon = null;
    
    public bool Stackable = true;
    [ConditionalField("Stackable")] public int StackLimit = 99;

    [Space(20)]
    public string ID = "";

    private void OnValidate()
    {
        ID = DisplayName.ToUpper().Replace(" ", "_");
        StackLimit = Stackable ? StackLimit : 1;
    }
}
