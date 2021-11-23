using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Profiles/CharLevelScalingHueing", fileName = "data")]
public class ScriptableObjectCharLevelScaleHueProfile : ScriptableObject
{
    [Header("Defaults")]
    [SerializeField] protected CharLevelHueScaleInfo Level1 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level2 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level3 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level4 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level5 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level6 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level7 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level8 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level9 = CharLevelHueScaleInfo.normal;
    [SerializeField] protected CharLevelHueScaleInfo Level10 = CharLevelHueScaleInfo.normal;

    CharLevelHueScaleInfo[] levelHueScalingInfo = null;
    public CharLevelHueScaleInfo[] LevelHueScaleInfo
    {
        get
        {
            return new CharLevelHueScaleInfo[]
            {
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            Level6,
            Level7,
            Level8,
            Level9,
            Level10,
            };
        }
    }
}
