using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnitManagementScript : MonoBehaviour
{


    public bool Test = false;
    public BaseCharacter CharOwner;
    public Color SelectionIndicatorColorUnselected;

    public TextMeshPro HpBarGroup;
    public void SetHP()
    {
        HpBarGroup.text = ((int)CharOwner.CharInfo.HealthStats.Health) + "/" + ((int)CharOwner.CharInfo.HealthStats.B_Health);
    }

    public Vector3 IndicatorContainer_DefaultLocalPosition;
    public Transform HpBarContainer;
    public Transform VitalityContainer;
    public BuffIconHandler buffIconHandler = null;

    public Transform[] BattleTransforms => new Transform[] { VitalityContainer, buffIconHandler.transform };



    //Used to set the unit info for the facing,side,unitbehaviour, tag and AI
    public void SetUnit()
    {
        if(CharOwner.InfoScript.Facing == FacingType.Right)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            HpBarContainer.eulerAngles = Vector3.zero;
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
        gameObject.tag = CharOwner.InfoScript.Side.ToString();
    }

    public void EnableBattleBars(bool hp = true)
    {
        HpBarContainer.gameObject.SetActive(hp);
    }


    public bool IsCharControllableByPlayers(List<ControllerType> controllers)
    {
        if (CharOwner.ReferenceCharacter == null) return false;

        foreach (ControllerType item in controllers)
        {
            if(!CharOwner.ReferenceCharacter.CharInfo.PlayerController.Contains(item))
            {
                return false;
            }
        }

        return true;
    }
}
