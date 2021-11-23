using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

[RequireComponent(typeof(Volume))]
public class PostProcessingManager : MonoBehaviour
{
    protected Volume Vol;

    ChromaticAberration _Chroma;
    ChromaticAberration Chroma
    {
        get
        {
            if(_Chroma == null)
                Vol.profile.TryGet(out _Chroma);
            return _Chroma;
        }
    }
    [SerializeField] protected float ChromaticResetSpeedMultiplier = 0.25f;
    float DefaultChroma = 0f;

    DepthOfField _Dof;
    DepthOfField Dof
    {
        get
        {
            if(_Dof == null)
                Vol.profile.TryGet(out _Dof);
            return _Dof;
        }
    }

    MotionBlur _MotionBlur;
    MotionBlur MotionBlur
    {
        get
        {
            if(_MotionBlur == null)
                Vol.profile.TryGet(out _MotionBlur);
            return _MotionBlur;
        }
    }

    ColorAdjustments _ColorAdjust;
    ColorAdjustments ColorAdjust
    {
        get
        {
            if(_ColorAdjust == null)
                Vol.profile.TryGet(out _ColorAdjust);
            return _ColorAdjust;
        }
    }
    ColorAdjustments _colorAdjust_Default;
    [SerializeField] protected PostProRecolorationSettings DefaultColorSettings;
    ColorAdjustments _colorAdjust_GameOverDesaturation;
    [SerializeField] protected PostProRecolorationSettings GameOverDesaturationSettings;

    protected Dictionary<ColorAdjustmentType, ColorAdjustments> ColorAdjustmentProfiles => new Dictionary<ColorAdjustmentType, ColorAdjustments>()
    {
        { ColorAdjustmentType.Default, _colorAdjust_Default },
        { ColorAdjustmentType.GameOverDesaturation, _colorAdjust_GameOverDesaturation },
    };


    private void Awake()
    {
        Vol = GetComponent<Volume>();

        DefaultChroma = Chroma.intensity.value;

        _colorAdjust_Default = DefaultColorSettings.GetColorAdjustment();
        _colorAdjust_GameOverDesaturation = GameOverDesaturationSettings.GetColorAdjustment();

        SettingsManager.Instance?.ApplySettings();
    }


    #region Blurs

    public void EnableBlur(bool state)
    {
        MotionBlur.active = state;
    }

    public void SetBlurry(bool state)
    {
        Dof.active = state;
    }

    #endregion


    #region Chromatic Abberation

    public void SetChromaticIntensity(float intensity, bool useMax = true)
    {
        Chroma.intensity.value = Mathf.Clamp(useMax ? Mathf.Max(intensity, Chroma.intensity.value) : intensity, 0f, 1f);
        if(ChromaCoResetter == null) 
            ResetChroma();
    }

    public void ResetChroma()
    {
        if (ChromaCoResetter != null) StopCoroutine(ChromaCoResetter);
        ChromaCoResetter = ResetChromaCo();
        StartCoroutine(ChromaCoResetter);
    }

    IEnumerator ChromaCoResetter = null;
    public IEnumerator ResetChromaCo()
    {
        while (Chroma.intensity.value != DefaultChroma)
        {
            yield return null;
            SetChromaticIntensity(Mathf.Max(Chroma.intensity.value - (Time.deltaTime * ChromaticResetSpeedMultiplier), DefaultChroma), false);
        }
        ChromaCoResetter = null;
    }

    /// <param name="playerID">As in the name of the curve player, not a player of the game</param>
    public void PlayChromaticAbberationCurve(AnimationCurve intensityCurve, float duration, string playerID = "")
    {
        if(playerID != "" && ChromaticAbberationCurvePlayers.ContainsKey(playerID))
        {
            StopChromaticAbberationCurve(playerID);
        }
        ChromaticAbberationCurvePlayers_KeyCount += playerID == "" ? 1 : 0;
        playerID = playerID == "" ? "Unnamed_" + ChromaticAbberationCurvePlayers_KeyCount.ToString() : playerID;
        ChromaticAbberationCurvePlayers.Add(
                playerID,
                PlayChromaticAbberationCurveCo(intensityCurve, duration, playerID)
            );
        StartCoroutine(ChromaticAbberationCurvePlayers[playerID]);
    }

    void StopChromaticAbberationCurve(string playerID)
    {
        StopCoroutine(ChromaticAbberationCurvePlayers[playerID]);
        ChromaticAbberationCurvePlayers.Remove(playerID);
    }

    public void ClearAllChromaticAbberationCurvesPlayers()
    {
        string[] keys = ChromaticAbberationCurvePlayers.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
            StopChromaticAbberationCurve(keys[i]);
    }

    int ChromaticAbberationCurvePlayers_KeyCount = 0;
    Dictionary<string, IEnumerator> ChromaticAbberationCurvePlayers = new Dictionary<string, IEnumerator>();
    IEnumerator PlayChromaticAbberationCurveCo(AnimationCurve intensityCurve, float duration, string ID)
    {
        float timeLeft = duration;
        while (true)
        {
            timeLeft = Mathf.Max(0f, timeLeft - Time.unscaledDeltaTime);
            SetChromaticIntensity(intensityCurve.Evaluate(duration == 0f ? 1f : (1f - (timeLeft / duration))));
            if (timeLeft == 0f) break;
            yield return null;
        }
        StopChromaticAbberationCurve(ID);
    }

    #endregion


    #region Color Adjustment

    public enum ColorAdjustmentType { Default, GameOverDesaturation }
    ColorAdjustmentType lastAdjustmentType = ColorAdjustmentType.Default;
    public void SetColorAdjustment(int index) => SetColorAdjustment((ColorAdjustmentType)index);
    public void SetColorAdjustment(ColorAdjustmentType type)
    {
        if (type == lastAdjustmentType) 
            return;
        lastAdjustmentType = type;

        if (ColorAdjustmentLerper != null) StopCoroutine(ColorAdjustmentLerper);
        ColorAdjustmentLerper = ColorAdjustmentLerpCo();
        StartCoroutine(ColorAdjustmentLerper);
    }

    IEnumerator ColorAdjustmentLerper = null;
    IEnumerator ColorAdjustmentLerpCo(float duration = 1.5f)
    {
        float progress = 0f;
        float timeLeft = duration;
        while (true)
        {
            timeLeft = Mathf.Clamp(timeLeft - Time.unscaledDeltaTime, 0f, 9999f);
            progress = 1f - (timeLeft / (duration == 0f ? 1f : duration));

            ColorAdjust.colorFilter.value = Color.Lerp(ColorAdjust.colorFilter.value, ColorAdjustmentProfiles[lastAdjustmentType].colorFilter.value, progress);
            ColorAdjust.contrast.value = Mathf.Lerp(ColorAdjust.contrast.value, ColorAdjustmentProfiles[lastAdjustmentType].contrast.value, progress);
            ColorAdjust.hueShift.value = Mathf.Lerp(ColorAdjust.hueShift.value, ColorAdjustmentProfiles[lastAdjustmentType].hueShift.value, progress);
            ColorAdjust.postExposure.value = Mathf.Lerp(ColorAdjust.postExposure.value, ColorAdjustmentProfiles[lastAdjustmentType].postExposure.value, progress);
            ColorAdjust.saturation.value = Mathf.Lerp(ColorAdjust.saturation.value, ColorAdjustmentProfiles[lastAdjustmentType].saturation.value, progress);

            if (timeLeft == 0f)
                break;
            yield return null;
        }
        ColorAdjustmentLerper = null;
    }

    #endregion
}


[System.Serializable]
public struct PostProRecolorationSettings
{
    public Color colorFilter;
    [Range(-100f, 100f)] public float contrast;
    [Range(-100f, 100f)] public float saturation;
    [Range(-180f, 180f)] public float hueShift;
    public float postExposure;

    public ColorAdjustments GetColorAdjustment()
    {
        ColorAdjustments res = new ColorAdjustments();
        res.colorFilter.value = colorFilter;
        res.colorFilter.overrideState = true;
        res.contrast.value = contrast;
        res.contrast.overrideState = true;
        res.hueShift.value = hueShift;
        res.hueShift.overrideState = true;
        res.postExposure.value = postExposure;
        res.postExposure.overrideState = true;
        res.saturation.value = saturation;
        res.saturation.overrideState = true;
        return res;
    }
}