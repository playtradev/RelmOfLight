using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileTargetScript : MonoBehaviour
{
    public Vector2Int Pos;
    public Gradient BaseGradient;
    public SpriteRenderer Indicator;
    public void StartTarget(float duration)
    {
        StartCoroutine(TargetAnim(duration));
    }
    
    private IEnumerator TargetAnim(float duration)
    {
        float timer = 0;
        Vector3 scalingValue = new Vector3(0.5f, 0.5f, 0.5f);
        while (timer < 1)
        {
			yield return null;

            while (BattleManagerScript.Instance != null)
            {
                yield return new WaitForEndOfFrame();
            }

            Indicator.color = BaseGradient.Evaluate(timer);
            timer += BattleManagerScript.Instance.FixedDeltaTime / duration;
            scalingValue += new Vector3(0.5f, 0.5f, 0.5f) / (duration / BattleManagerScript.Instance.FixedDeltaTime);
            Indicator.transform.localScale = scalingValue;
        }

        gameObject.SetActive(false);
    }

    public void StartLoopOnCharging()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void StopLoopOnCharging()
    {

    }
}
