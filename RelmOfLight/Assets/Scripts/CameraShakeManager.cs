using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

//[RequireComponent(typeof(Camera))]
public class CameraShakeManager : MonoBehaviour
{
    [Header("General")]
    public ShakeEffect[] ShakeEffectList = new ShakeEffect[0];

    protected float startingOrth = 0f;
    public float OrthAdjustment
    {
        get
        {
            return cam.orthographicSize / startingOrth;
        }
    }
    protected Camera cam = null;

    [Header("Testing")]
    public bool TestingEnabled = false;
    [ConditionalField("TestingEnabled")] public KeyCode TestingButton = KeyCode.Alpha9;
    [ConditionalField("TestingEnabled")] public KeyCode ResetButton = KeyCode.Alpha0;
    [ConditionalField("TestingEnabled")] public string ShakeToTest = "ENTER_TEST_NAME_HERE";
    [ConditionalField("TestingEnabled")] [Tooltip("Leave 0 to test at default duration of ShakeData")] public float TestOverrideDuration = 0f;



    private void Awake()
    {
        cam = GetComponent<Camera>();
        startingOrth = cam.orthographicSize;
    }

    private void Update()
    {
        if (TestingEnabled && Input.GetKeyDown(TestingButton)) PlayShake(ShakeToTest, TestOverrideDuration);
        if (TestingEnabled && Input.GetKeyDown(ResetButton)) HaltShaking();
        //if (Input.GetKeyDown(KeyCode.Alpha0)) HaltShaking();
    }

    public void PlayShake(ShakeEffect shake, float duration = 0f, System.Func<bool> conditions = null)
    {
        if (shake == null) return;

        duration = duration == 0f ? shake.defaultDuration : duration;

        activeShakeEffects.Add(new ShakeEffectProcessorClass(duration, shake, conditions == null ? () => true : conditions, this));

        if (activeShakeEffects.Count == 1)
        {
            StartCoroutine("ShakeHandlerCo");
        }
    }

    public bool PlayShake(string ID, float duration = 0f, System.Func<bool> conditions = null)
    {
        ShakeEffect shake = ShakeEffectList.Where(r => r.ShakeID == ID).FirstOrDefault();
        if(shake == null)
        {
            Debug.LogError("SHAKE EFFECT WITH ID: '" + ID + "' NOT FOUND, ABORTING");
            return false;
        }

        PlayShake(shake, duration, conditions);

        return true;
    }

    public bool HaltShaking()
    {
        if (activeShakeEffects.Count > 0)
        {
            activeShakeEffects.Clear();
            return true;
        }
        return false;
    }

    List<ShakeEffectProcessorClass> activeShakeEffects = new List<ShakeEffectProcessorClass>();
    IEnumerator ShakeHandlerCo()
    {
        Vector3 anchorPos = transform.localPosition;
        Vector2 lastOut = Vector2.zero;
        float lastIntensity = 0f;
       // bool first = true;

        while(activeShakeEffects.Count > 0)
        {
            transform.localPosition = anchorPos + (new Vector3(lastOut.x, lastOut.y, 0f) * OrthAdjustment);
            float vibrationAdjustedIntensity = Mathf.Clamp(lastOut.normalized.magnitude * 1.8f, 0, 1f);
            CameraManagerScript.PostProlone.SetChromaticIntensity(Mathf.Clamp(lastIntensity, 0f, 1f));

            //first = true;
            lastOut = Vector2.zero;
            lastIntensity = 0f;

            foreach (ShakeEffectProcessorClass shakeEffect in activeShakeEffects.ToArray())
            {
                if (shakeEffect.Expired)
                {
                    activeShakeEffects.Remove(shakeEffect);
                    continue;
                }

                lastOut += shakeEffect.Output;
                lastIntensity += shakeEffect.effect.UseChromaticAbberation ? shakeEffect.IntensityOut * shakeEffect.effect.chromaticAbberation : 0f;
                //lastOut *= first ? 1f : 0.5f;
                //first = false;
            }

            yield return BattleManagerScript.Instance.WaitFixedUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        }
        transform.localPosition = anchorPos;
    }

    private void OnValidate()
    {
        foreach (ShakeEffect shakeE in ShakeEffectList)
        {
            shakeE.OnValidate();
        }
    }
}

public class ShakeEffectProcessorClass
{
    public Vector2 Output = new Vector2();
    public float IntensityOut = 0f;
    public bool Expired = false;

    float moveCoTimeRemaining = 0f;
    public ShakeEffect effect = null;
    public MonoBehaviour handler = null;

    public ShakeEffectProcessorClass(float _duration, ShakeEffect _effect, System.Func<bool> conditions, MonoBehaviour _handler = null)
    {
        if(handler == null && _effect.Frequency == 0f)
        {
            Expired = true;
            return;
        }

        Expired = false;
        effect = _effect;
        moveCoTimeRemaining = 0f;
        handler = _handler;
        handler.StartCoroutine(ShakeCo(_duration, conditions));
    }


    IEnumerator ShakeCo(float duration, System.Func<bool> conditions)
    {
        IntensityOut = 0f;
        float timeLeft = duration;
        while(timeLeft != 0f && conditions())
        {
            timeLeft = Mathf.Clamp(timeLeft - (BattleManagerScript.Instance.FixedDeltaTime), 0f, 99f);
            IntensityOut = effect.IntensityCurve.Evaluate(1f - (timeLeft / duration));
            if (moveCoTimeRemaining == 0f)
            {
                moveCoTimeRemaining = 1f/(effect.Frequency * effect.FrequencyCurve.Evaluate(1f - (timeLeft/duration)));
                handler.StartCoroutine(MoveCo(1f - (timeLeft / duration)));
				yield return null;
            }
        }
        IntensityOut = 0f;
        Expired = true;
    }


    IEnumerator MoveCo(float progress)
    {
        float moveCoDuration = moveCoTimeRemaining;
        Vector2 startPoint = Output;
        Vector2 endPoint = effect.RandomShakePoint * effect.IntensityCurve.Evaluate(progress);
        while (moveCoTimeRemaining != 0f)
        {
            moveCoTimeRemaining = Mathf.Clamp(moveCoTimeRemaining - (BattleManagerScript.Instance.FixedDeltaTime), 0f, 99f);
            Output = effect.Lerp(startPoint, endPoint, 1f - (moveCoTimeRemaining / moveCoDuration));
			yield return null;
        }
    }
} 
