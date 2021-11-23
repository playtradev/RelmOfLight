using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICrystalInfo : MonoBehaviour
{
    public TextMeshProUGUI HpText;
    public CharacterInfoScript Charinfo = null;
    private void Update()
    {
        if (Charinfo != null)
        {
            HpText.text = ((int)Charinfo.HealthStats.Health).ToString();
        }
    }
}
