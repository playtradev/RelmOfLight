using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class BuffIconHandler : MonoBehaviour
{
    [SerializeField] protected GameObject statusIconPrefab = null;

    protected List<BuffIcon> buffIcons = new List<BuffIcon>();

    [SerializeField] protected List<Transform> iconSlots = new List<Transform>();
    BuffIcon bI;

    private void Awake()
    {
        foreach (Transform iconSlot in iconSlots)
        {
            buffIcons.Add(Instantiate(statusIconPrefab, iconSlot.position, Quaternion.identity, transform).GetComponent<BuffIcon>());
        }
        buffIcons = GetComponentsInChildren<BuffIcon>().ToList();
    }

  /*  public void RefreshIcons(List<BuffDebuffClass> statusList)
    {
        statusList = statusList.Where(r => r.CurrentBuffDebuff.Icon != null && !r.Stop_Co).ToList();
        //Get the bufficons that should no longer be running and stop them
        foreach (BuffIcon ico in buffIcons)
        {
            if (ico.StatusEffect == null) continue;
            for (int i = 0; i < iconSlots.Count; i++)
            {
                if (statusList.Count > i)
                {
                    if (statusList[i].Stat == ico.StatusEffect.StatsToAffect)
                    {
                        //if the bufficon isn't in the right spot move it
                        if (ico.transform.localPosition != iconSlots[i].localPosition && ico.movingPos != iconSlots[i].localPosition)
                        {
                            ico.MoveStatusIcon(iconSlots[i].localPosition);
                        }

                        //return since this ico is present in the list of status list of the player
                        break;
                    }
                }
                if (i + 1 == iconSlots.Count) ico.TerminateStatusIcon();
            }
        }

        //Get the bufficons that need to be created and make them
        for (int i = 0; i < iconSlots.Count && statusList.Count > i; i++)
        {
            if (buffIcons.Where(r => r.StatusEffect != null && r.StatusEffect.StatsToAffect == statusList[i].Stat).FirstOrDefault() == null)
            {
                bI = buffIcons.Where(r => r.StatusEffect == null).FirstOrDefault();
                if (bI != null)
                {
                    bI.InitiateStatusIcon(statusList[i].Effect);
                    if (bI.transform.localPosition != iconSlots[i].localPosition && bI.movingPos != iconSlots[i].localPosition)
                    {
                        bI.MoveStatusIcon(iconSlots[i].localPosition);
                    }
                }
            }
        }
    }*/

    public void RefreshIcons(List<BuffDebuffIconClass> buffDebuffIcons)
    {
        //Get the bufficons that should no longer be running and stop them
        foreach (BuffIcon ico in buffIcons)
        {
            if (ico.BuffDebuffIcon == null) continue;
            for (int i = 0; i < iconSlots.Count; i++)
            {
                if (buffDebuffIcons.Count > i)
                {
                    if (buffDebuffIcons[i].StatsToAffect == ico.BuffDebuffIcon.StatsToAffect)
                    {
                        //if the bufficon isn't in the right spot move it
                        if (ico.transform.localPosition != iconSlots[i].localPosition && ico.movingPos != iconSlots[i].localPosition)
                        {
                            ico.MoveStatusIcon(iconSlots[i].localPosition);
                        }

                        //return since this ico is present in the list of status list of the player
                        break;
                    }
                }
                if (i + 1 == iconSlots.Count)
                {
                    ico.TerminateStatusIcon();
                }
            }
        }

        //Get the bufficons that need to be created and make them
        for (int i = 0; i < iconSlots.Count && buffDebuffIcons.Count > i; i++)
        {
            if (buffIcons.Where(r => r.BuffDebuffIcon != null && r.BuffDebuffIcon.StatsToAffect == buffDebuffIcons[i].StatsToAffect).FirstOrDefault() == null)
            {
                bI = buffIcons.Where(r => r.BuffDebuffIcon == null).FirstOrDefault();
                if(bI != null)
                {
                    bI.InitiateStatusIcon(buffDebuffIcons[i]);
                    if (bI.transform.localPosition != iconSlots[i].localPosition && bI.movingPos != iconSlots[i].localPosition)
                    {
                        bI.MoveStatusIcon(iconSlots[i].localPosition);
                    }
                }
            }
        }
    }
}

