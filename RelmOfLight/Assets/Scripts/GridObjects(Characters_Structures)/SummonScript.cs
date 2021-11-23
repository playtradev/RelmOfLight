using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonScript : MonoBehaviour
{
    protected BaseCharacter charTracking = null;
    public bool countingDown = false;

    public void Init(float duration = 0f)
    {
        charTracking = GetComponent<BaseCharacter>();
        if (charTracking == null)
        {
            Debug.LogError("SUMMON NOT CODED RIGHT, BLAME BELTAIN");
            return;
        }
        foreach (ParticleHelperScript item in GetComponentsInChildren<ParticleHelperScript>(true))
        {
            item.SetStopAction(ParticleSystemStopAction.None);
        }
        charTracking.CharInfo.IsSummon = true;
        countingDown = false;
        charTracking.CurrentCharIsDeadEvent += TrackedCharDies;
        if (duration > 0f) StartCoroutine(Countdown(duration));
    }

    protected IEnumerator Countdown(float duration)
    {
        countingDown = true;
        while (0f < duration && countingDown)
        {
            duration -= Time.deltaTime;
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || BattleManagerScript.Instance.isSkillHappening.Value);
        }
        countingDown = false;
        if (charTracking.CharInfo.Behaviour.MovementActionN == MovementActionType.Idle || charTracking.CharInfo.Behaviour.MovementActionN == MovementActionType.None)
        {
            GridManagerScript.Instance.GetBattleTile(charTracking.CharInfo.CurrentTilePos + (charTracking.CharInfo.Side == TeamSideType.LeftSideTeam ? new Vector2Int(0, -1) : new Vector2Int(0, 1))).TileHeatMapPoints += 3;
        }
        charTracking.HittedByList = new List<HitInfoClass>();
        charTracking.CharInfo.Health = -5;
    }

    public virtual void TrackedCharDies(BaseCharacter character)
    {

    }

    protected void OnDisable()
    {
        StopAllCoroutines();
        charTracking.CharInfo.IsSummon = false;
        charTracking.CurrentCharIsDeadEvent -= TrackedCharDies;
        Destroy(this);
    }


}