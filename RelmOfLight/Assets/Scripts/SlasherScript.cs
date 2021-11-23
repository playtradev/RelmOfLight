using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlasherScript : MonoBehaviour
{
    public TextMeshProUGUI Damage_Heal;
    public TextMeshProUGUI Critical;
    public TextMeshProUGUI Buff_Debuff;

    public Animator Anim;

    public void SetupIndicator(string txt, int value, BattleFieldIndicatorMaterialClass bfim, bool Buff = false)
    {
        if(Buff)
        {
            Buff_Debuff.text = txt;
            Buff_Debuff.fontMaterial = bfim.Mat;
            Buff_Debuff.UpdateFontAsset();
        }
        else
        {
            Damage_Heal.text = txt;
            Damage_Heal.fontMaterial = bfim.Mat;
            Damage_Heal.UpdateFontAsset();
        }
        Anim.SetInteger("State", value);
    }
}
