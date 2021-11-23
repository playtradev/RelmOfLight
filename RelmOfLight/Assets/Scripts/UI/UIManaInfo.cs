using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManaInfo : MonoBehaviour
{
    public TeamSideType Side;
    public TextMeshProUGUI ManaText;

    private void Update()
    {
        ManaText.text = Side == TeamSideType.LeftSideTeam ? ((int)BattleManagerScript.Instance.LeftMana.CurrentMana).ToString() :
            ((int)BattleManagerScript.Instance.RightMana.CurrentMana).ToString();
    }

}
