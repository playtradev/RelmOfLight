using Spine.Unity;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class SpineAnimationManager : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton skeleton;
    public ISpineCharacter CharOwner;

    [HideInInspector]public AnimationMovementCurveClass Space_Time_Curves;


    public List<Transform> FiringPints = new List<Transform>();
    public string _CurrentAnim;


    public string CurrentAnim
    {
        get
        {
            return _CurrentAnim;
        }
        set
        {
            _CurrentAnim = value;
        }
    }
    public float AnimationTransition = 0.1f;

    public bool Loop = false;

    public List<CustomSpineEventData> CustomEvents = new List<CustomSpineEventData>();


    //initialize all the spine element
    public void SetupSpineAnim()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            skeletonAnimation.enabled = true;
            SpineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
        }
    }

    public void SetAnim(CharacterAnimationStateType anim)
    {

        if (CurrentAnim == CharacterAnimationStateType.Death.ToString() && anim.ToString() != CharacterAnimationStateType.Idle.ToString())
        {
            return;
        }

        if (anim == CharacterAnimationStateType.Arriving || anim.ToString().Contains("Growing"))
        {
            SetAnim(anim, false, 0);
        }
        else
        {
            SetAnim(anim, anim == CharacterAnimationStateType.Death || anim == CharacterAnimationStateType.Idle ? true : false, AnimationTransition);
        }
    }

    public void SetAnim(CharacterAnimationStateType anim, bool loop, float transition)
    {
        if (anim == CharacterAnimationStateType.Idle && !loop)
        {
            //Debug.Log("Arriving start");
        }
        Loop = loop;
        //Debug.Log(anim.ToString());

        SpineAnimationState.SetAnimation(0, anim.ToString(), loop).MixDuration = transition;
        //StartCoroutine(ClearAnim(transition));
        CurrentAnim = anim.ToString();
    }

    public void SetAnim(string anim, bool loop, float transition)
    {
        if (anim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
        {
            //Debug.Log("Arriving start");
        }
        Loop = loop;
        //Debug.Log(anim.ToString());

        SpineAnimationState.SetAnimation(0, anim, loop).MixDuration = transition;
        CurrentAnim = anim;
    }

   
    public float GetAnimTime()
    {
        return skeletonAnimation.state.GetCurrent(1).TrackTime;
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        if (skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()) != null)
        {
            return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
        }
        return 1;
    }

    public float GetAnimLenght(string anim)
    {
        if (skeletonAnimation.Skeleton.Data.FindAnimation(anim) != null)
        {
            return skeletonAnimation.Skeleton.Data.FindAnimation(anim).Duration;
        }
        return 1;
    }

    public void SetAnimationSpeed(float speed)
    {
        //Debug.LogError(CharOwner.ReferenceCharacter.CharInfo.CharacterID + "    Speine speed  " + speed);
        if (SpineAnimationState != null)
        {
            SpineAnimationState.Tracks.ForEach(r => {
                if (r != null)
                {
                    r.TimeScale = speed;
                }
            });
        }
    }

    public float GetAnimationSpeed()
    {
        if (SpineAnimationState != null)
        {
            if (SpineAnimationState.Tracks.Where(r => r != null).FirstOrDefault() != null)
            {
                return SpineAnimationState.Tracks.Where(r => r != null).FirstOrDefault().TimeScale;
            }
        }
        return 1f;
    }

    public void SetSkeletonOrderInLayer(int order)
    {
        GetComponent<MeshRenderer>().sortingOrder = order;
    }

    public void SetSkeletonSortingLayer(LightLayersType layerName)
    {
        GetComponent<MeshRenderer>().sortingLayerName = layerName.ToString();
    }


    public bool HasAnimation(string animName)
    {
        if(skeleton != null && skeleton.Data != null)
        {
            return skeleton.Data.FindAnimation(animName) == null ? false : true;
        }
        return false;
    }

    public void FadeInOut(bool inOut, float delay, float duration)
    {
        StartCoroutine(FadeInOutCo(inOut, delay, duration));
    }
    private IEnumerator FadeInOutCo(bool inOut, float delay, float duration)
    {
        MaterialPropertyBlock temp_MaterialPropertyBlock = new MaterialPropertyBlock();

        yield return BattleManagerScript.Instance.WaitFor(delay, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        float time = 0;
        while (time < 1)
        {
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            time += BattleManagerScript.Instance.DeltaTime / duration;

            temp_MaterialPropertyBlock.SetColor("_Color", new Color(1f, 1f, 1f, Mathf.Lerp(inOut ? 0 : 1, inOut ? 1 : 0, time))); // "_FillColor" is a named property on the used shader.

            Rend.SetPropertyBlock(temp_MaterialPropertyBlock);
        }
    }


    #region Recoloring And Such

    static Shader _desiredShader = null;
    Shader DesiredShader
    {
        get
        {
            if(_desiredShader == null)
            {
                _desiredShader = Shader.Find("Universal Render Pipeline/2D/Spine/Sprite");
            }
            return _desiredShader;
        }
    }
    MeshRenderer _rend = null;
    public MeshRenderer Rend
    {
        get
        {
            if(_rend == null)
            {
                _rend = GetComponent<MeshRenderer>();
            }
            return _rend;
        }
    }


    MaterialPropertyBlock _defaultMaterialBlock = null;
    MaterialPropertyBlock DefaultMaterialBlock
    {
        get
        {
            _defaultMaterialBlock = new MaterialPropertyBlock();
            _defaultMaterialBlock.Clear();
            _defaultMaterialBlock.SetColor("_OverlayColor", DefaultOverlayColor);
            _defaultMaterialBlock.SetFloat("_Hue", DefaultHue);
            _defaultMaterialBlock.SetFloat("_Saturation", DefaultSaturation);
            return _defaultMaterialBlock;
        }
    }

    Color DefaultOverlayColor = new Color(1f,1f,1f,0f);
    float _defaultHue = 0f;
    float DefaultHue { get => _defaultHue; set => _defaultHue = Mathf.Clamp(value, -0.5f, 0.5f); }
    float _defaultSaturation = 1f;
    float DefaultSaturation { get => _defaultSaturation; set => _defaultSaturation = Mathf.Clamp(value, 0f, 2f); }
    public void SetDefaultOverlayColor(ColorHueSat colorHueSat)
    {
        DefaultHue = colorHueSat.hue;
        DefaultSaturation = colorHueSat.sat;
        DefaultOverlayColor = colorHueSat.color;
        Rend.SetPropertyBlock(DefaultMaterialBlock);
    }

    public void SetDefaultOverlayColor(Color color)
    {
        DefaultOverlayColor = color;
        Rend.SetPropertyBlock(DefaultMaterialBlock);
    }

    public ColorHueSat GetColorHueSat()
    {
        ColorHueSat chs;
        chs.color = DefaultMaterialBlock.GetColor("_OverlayColor");
        chs.hue = DefaultMaterialBlock.GetFloat("_Hue");
        chs.sat = DefaultMaterialBlock.GetFloat("_Saturation");
        return chs;
    }


    static MaterialPropertyBlock _whiteOutedMaterialBlock = null;
    MaterialPropertyBlock WhiteOutedMaterialBlock
    {
        get
        {
            if (_whiteOutedMaterialBlock == null)
            {
                _whiteOutedMaterialBlock = new MaterialPropertyBlock();
                _whiteOutedMaterialBlock.SetColor("_OverlayColor", new Color(1f, 1f, 1f, 1f)); // "_FillColor" is a named property on the used shader.
                _whiteOutedMaterialBlock.SetFloat("_OverlayColor", 0.7f); // "_FillPhase" is another named property on the used shader.
            }
            return _whiteOutedMaterialBlock;
        }
    }
    public Color HitColor;


    public void PlayHitWhiteOut(float duration = 0.45f, float firstWhitePerc = 0.3f, int followUpWhites = 0)
    {
        if (Rend == null || Rend.material == null || Rend.material.shader != DesiredShader)
        {
            Debug.Log("Attempted to play a while out on the character, but the shader or the renderer could not be validated... Is it missing a mesh or the right config? [Belt]");
            return;
        }

        if(!isActiveAndEnabled)
        {
            return;
        }

        if (HitWhiteOuter != null) StopCoroutine(HitWhiteOuter);
        HitWhiteOuter = HitWhiteOutCo(duration, firstWhitePerc, followUpWhites);
        StartCoroutine(HitWhiteOuter);
    }

    IEnumerator HitWhiteOuter = null;
    IEnumerator HitWhiteOutCo(float duration, float firstWhitePerc, int followUpWhites)
    {
        yield return SetHitWhiteOutFor(duration * firstWhitePerc - duration * 0.05f, duration * 0.05f);

        float timeStepForFollowUps = ((1f - firstWhitePerc) * duration) / ((float)followUpWhites * 2f);

        for (int i = 0; i < followUpWhites; i++)
        {
            yield return SetHitWhiteOutFor(timeStepForFollowUps, timeStepForFollowUps);
        }
    }

    IEnumerator SetHitWhiteOutFor(float dur, float delayDur)
    {
        Rend.SetPropertyBlock(DefaultMaterialBlock);

        yield return new WaitForSecondsRealtime(delayDur);

        Rend.SetPropertyBlock(WhiteOutedMaterialBlock);
        
        yield return new WaitForSecondsRealtime(dur);

        Rend.SetPropertyBlock(DefaultMaterialBlock);
    }


    public void Start_ColorCurveLooper(AnimationCurve curve, Color overlayColor, Color baseColor, float duration)
    {
        if (_ColorCurveLooperCo != null)
            StopCoroutine(_ColorCurveLooperCo);

        _BaseColorHueSatColor = baseColor;

        _ColorCurveLooperCo = ColorCurveLooper(curve, overlayColor, duration);
        StartCoroutine(_ColorCurveLooperCo);
    }

    public void Stop_ColorCurveLooper()
    {
        if (_ColorCurveLooperCo == null)
            return;

        StopCoroutine(_ColorCurveLooperCo);
        SetDefaultOverlayColor(_BaseColorHueSatColor);
    }

    float _ColorCurveLooperTime = 0f;
    Color _BaseColorHueSatColor = Color.clear;
    IEnumerator _ColorCurveLooperCo = null;
    IEnumerator ColorCurveLooper(AnimationCurve curve, Color overlayColor, float duration)
    {
        _ColorCurveLooperTime = 0f;
        while (true)
        {
            yield return BattleManagerScript.Instance.WaitUpdate(() => false);
            _ColorCurveLooperTime += BattleManagerScript.Instance.DeltaTime;
            if (_ColorCurveLooperTime > duration)
                _ColorCurveLooperTime = Mathf.Clamp(_ColorCurveLooperTime - duration, 0f, 99999f);

            SetDefaultOverlayColor(Color.Lerp(overlayColor, _BaseColorHueSatColor, curve.Evaluate(_ColorCurveLooperTime / duration)));
        }
    }

    #endregion



    public bool changeanim = false;
    public float transition = 0.5f;
    public CharacterAnimationStateType NextAnim = CharacterAnimationStateType.Arriving;
    private void Update()
    {
        if (changeanim)
        {
            changeanim = false;
            SetAnim(NextAnim.ToString(), false, transition);
        }
    }

    private void OnEnable()
    {
        Rend.SetPropertyBlock(DefaultMaterialBlock);
    }

}

[System.Serializable]
public class CurrentAnimClass
{
    public CharacterAnimationStateType CurrentAnimation;
    public int CurrentTrack;

    public CurrentAnimClass()
    {
        CurrentTrack = 0;
    }
}

[System.Serializable]
public class AnimationMovementCurveClass
{
    public AnimationCurve UpMovement;
    public AnimationCurve DownMovement;
    public AnimationCurve BackwardMovement;
    public AnimationCurve ForwardMovement;
}

[System.Serializable]
public struct ColorHueSat
{
    public Color color;
    [Tooltip("Number between -0.5, 0.5, with a default of 0")][Range(-0.5f, 0.5f)] public float hue;
    [Tooltip("Number between 0, 2, with a default of 1")] [Range(0f, 2f)] public float sat;

    public ColorHueSat(Color color, float hue, float sat)
    {
        this.color = color;
        this.hue = hue;
        this.sat = sat;
    }

    public static ColorHueSat generic => new ColorHueSat(Color.white, 0f, 1f);
}

[System.Serializable]
public struct CustomSpineEventData
{
    [Tooltip("The exact name of the event in the fungus animation (CASE SENSITIVE)"), Header("__IDENTITY__")] public string EventID;
    [Header("Camera Shake"), Header("__EVENT EFFECTS__"), Space(15)]
    public bool Use_CameraShake;
    public bool Use_PremadeShake;
    [Header(">Premade Shake Settings")] 
    public string PremadeShake_ID;
    public float PremadeShake_Duration;
    [Header(">Custom Shake Settings"), Space(5)] 
    public ShakeEffect CustomShake_Info;
    [Header("Chromatic Abberation"), Space(8)]
    public bool Use_ChromaticAbberation;
    public AnimationCurve ChromaticAbberation_Curve;
    public float ChromaticAbberation_Duration;
    public string ChromaticAbberation_TrackingID;
    [Header("Play Audio"), Space(8)]
    public bool Use_Audio;
    public bool Use_PremadeAudio;
    [Header(">Premade Audio Settings")]
    public string PremadeAudio_ID;

    bool IsEvent(string EventID) => EventID == this.EventID;

    public bool TryDoEvent(string EventID)
    {
        if (!IsEvent(EventID))
            return false;

        if (Use_CameraShake)
        {
            if (Use_PremadeShake)
                CameraManagerScript.Shaker.PlayShake(PremadeShake_ID, PremadeShake_Duration);
            else
                CameraManagerScript.Shaker.PlayShake(CustomShake_Info);
        }
        if (Use_ChromaticAbberation)
        {
            CameraManagerScript.PostProlone.PlayChromaticAbberationCurve(ChromaticAbberation_Curve, ChromaticAbberation_Duration, ChromaticAbberation_TrackingID);
        }

        return true;
    }
}