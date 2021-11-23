using UnityEngine;
using MyBox;

[System.Serializable]
public class ShakeEffect
{
    [HideInInspector] public string Name = "";

    public string ShakeID = "";

    [Header("Move Point Calculation")]
    [Tooltip("The amount of sway the bias has on the direction of the shake")][Range(0f, 1f)] public float biasWeight = 0.5f;
    [Tooltip("A direction vector that biases the shake toward a certain angle. [(0,1) is up, (1,0) is right]")] [SerializeField] protected Vector2 directionalBias = Vector2.zero;
    public Vector2 DirectionalBias
    {
        get
        {
            return directionalBias.normalized;
        }
    }
    [Tooltip("How far the shake extends, with 1 being the normalized default")] public float intensity = 1f;
    [Tooltip("How the intensity of the moves changes over the duration")] public AnimationCurve IntensityCurve = AnimationCurve.Linear(1f, 1f, 0f, 0f);
    [Tooltip("How much, as a percentage, the shake can deviate from it's intensity toward 0f")] [Range(0f, 1f)] public float variance = 0.5f;

    public Vector2 RandomShakePoint
    {
        get
        {
            Vector2 res = DirectionalBias != Vector2.zero ? Vector2.ClampMagnitude(((Random.insideUnitCircle.normalized * (1f - biasWeight)) + (DirectionalBias * (1f + biasWeight))).normalized, 1f) : Vector2.ClampMagnitude(Random.insideUnitCircle.normalized, 1f);
            res *= UnityEngine.Random.Range(1f - variance, 1f) * intensity;
            res = new Vector2(res.x, res.y) * (Random.Range(0, 10) >= 5 ? 1 : -1);
            return res;
        }
    }


    [Header("Move Type")]
    [Tooltip("You already know what it is")] public float defaultDuration = 1f;
    [Tooltip("How many moves the camera makes in a second")] public float Frequency = 5f;
    [Tooltip("How the frequency of the moves changes over the duration")] public AnimationCurve FrequencyCurve = AnimationCurve.Constant(0f, 1f, 1f);
    [Tooltip("The travel progress curve the camera will make toward each individual shake point")] public AnimationCurve shakeMoveCurve = AnimationCurve.Linear(0f,0f,1f,1f);

    [Header("Post Pro-lone")]
    [Tooltip("Does the shake apply rainbow edges?")] public bool UseChromaticAbberation = false;
    [ConditionalField("UseChromaticAbberation")][Tooltip("How much chromatic abberation should be applied as per the inensity curve")] public float chromaticAbberation = 0f;


    public Vector2 Lerp(Vector2 startPoint, Vector2 endPoint, float point)
    {
        if (shakeMoveCurve == null) shakeMoveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        return Vector2.Lerp(startPoint, endPoint, shakeMoveCurve.Evaluate(point));
    }

    public void OnValidate()
    {
        Name = ShakeID != "" ? ShakeID : "UNIDENTIFIED SHAKE";
        if (shakeMoveCurve == null) shakeMoveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        Frequency = Frequency == 0 ? 1 : Frequency;
    }
}