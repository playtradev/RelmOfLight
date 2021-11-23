using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(CanvasGroup))]
public class BurstIndicator : MonoBehaviour
{
    [FormerlySerializedAs("textMesh")] public TextMeshProUGUI TextMesh = null;
    public Image RightImageRef = null;
    public Image LeftImageRef = null;
    public float SpaceBetweenItems = 12f;

    [Space(10)]
    public Animation anim = null;
    CanvasGroup canv = null;
    Vector3 startingScale = new Vector3();

    BurstIndicationStyleClass style = null;
    public string styleTypeID => style == null ? null : style.TypeID;
    BurstIndicationMovementTypeClass movement = null;
    [HideInInspector] public BaseCharacter charOwner = null;
    public Transform followParent => charOwner == null ? null : charOwner.spineT.transform;

    [HideInInspector] public float playTime = 999f;
    public float PlayProgress => movement == null ? 0f : playTime / (movement.Duration == 0f ? 1f : movement.Duration);

    private void Awake()
    {
        startingScale = transform.localScale;
        canv = GetComponentInChildren<CanvasGroup>();
        anim = GetComponentInChildren<Animation>();
        TextMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (TextMesh == null)
            Debug.LogError("DEPLETION INDICATOR NOT CONFIGURED WITH A TEXTMESH COMPONENT || INDICATORCANVAS > DEPLETION INDICATOR PREFAB");
    }

    void SetIndicator(BurstIndicationStyleClass styleInfo, BurstIndicationMovementTypeClass moveInfo, BaseCharacter target, string title)
    {
        //Set info
        style = styleInfo;
        movement = moveInfo;
        charOwner = target;

        RefreshIndicator(title);
    }

    void RefreshIndicator() => RefreshIndicator(TextMesh.text);

    FacingType LastFacingType;
    void RefreshIndicator(string title)
    {
        if (charOwner == null)
            return;
        LastFacingType = charOwner.CharInfo.Facing;

        //Set text
        TextMesh.text = title;
        TextMesh.fontSize = style.FontSize;
        TextMesh.color = style.FontColor;
        TextMesh.outlineColor = style.OutlineColor;
        TextMesh.material.SetColor("_UnderlayColor", style.ShadowColor); 
        TextMesh.alignment = LastFacingType == FacingType.Right ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
        TextMesh.ForceMeshUpdate();

        //Image Setting
        if (style.AccompanyingImage != null)
        {
            RightImageRef.sprite = style.ImageSide != BurstIndicationStyleClass.ImageSideType.AfterText ? style.AccompanyingImage : null;
            RightImageRef.rectTransform.sizeDelta = Vector2.one * TextMesh.textBounds.size.y;
            LeftImageRef.sprite = style.ImageSide != BurstIndicationStyleClass.ImageSideType.BeforeText ? style.AccompanyingImage : null;
            LeftImageRef.rectTransform.sizeDelta = Vector2.one * TextMesh.textBounds.size.y;
        }
        RightImageRef.color = style.AccompanyingImage != null && style.ImageSide != BurstIndicationStyleClass.ImageSideType.AfterText ? style.ImageColor : Color.clear;
        LeftImageRef.color = style.AccompanyingImage != null && style.ImageSide != BurstIndicationStyleClass.ImageSideType.BeforeText ? style.ImageColor : Color.clear;

        //Spacing Setting
        float xPositioning = 0;
        if (RightImageRef.sprite != null)
        {
            RightImageRef.rectTransform.pivot = new Vector2(LastFacingType == FacingType.Right ? 1f : 0f, 0.5f);
            RightImageRef.transform.localPosition = new Vector3(xPositioning, RightImageRef.transform.localPosition.y, RightImageRef.transform.localPosition.z);
            xPositioning += (RightImageRef.rectTransform.sizeDelta.x + SpaceBetweenItems) * (LastFacingType == FacingType.Right ? -1 : 1);
        }

        TextMesh.rectTransform.pivot = new Vector2(LastFacingType == FacingType.Right ? 1f : 0f, 0.5f);
        TextMesh.transform.localPosition = new Vector3(xPositioning, TextMesh.transform.localPosition.y, TextMesh.transform.localPosition.z);
        xPositioning += (TextMesh.textBounds.size.x + SpaceBetweenItems) * (LastFacingType == FacingType.Right ? -1 : 1);

        if (LeftImageRef.sprite != null)
        {
            LeftImageRef.rectTransform.pivot = new Vector2(LastFacingType == FacingType.Right ? 1f : 0f, 0.5f);
            LeftImageRef.transform.localPosition = new Vector3(xPositioning, LeftImageRef.transform.localPosition.y, LeftImageRef.transform.localPosition.z);
            xPositioning += (LeftImageRef.rectTransform.sizeDelta.x + SpaceBetweenItems) * (LastFacingType == FacingType.Right ? -1 : 1);
        }
    }

    public void PlayIndicator(BurstIndicationStyleClass styleInfo, BurstIndicationMovementTypeClass moveInfo, BaseCharacter targetChar, Vector2 startOffset, Vector2 endOffset, string textToDisplay, System.Func<bool> stopRule)
    {
        if (playTime != 999f)
        {
            if (styleInfo.StackUntilProgress > 0f) StackIndicators(textToDisplay);
            return;
        }
        playTime = 0f;

        SetIndicator(styleInfo, moveInfo, targetChar, textToDisplay);

        StartCoroutine(PlayIndicatorCo(stopRule, startOffset, endOffset));
    }


    void StackIndicators(string newValue)
    {
        if(float.TryParse(TextMesh.text, out float valueFromCurrent) && float.TryParse(newValue, out float valueFromNew))
        {
            RefreshIndicator((valueFromCurrent + valueFromNew).ToString("F0"));
            //transform.SetAsLastSibling();
            //anim.Stop();
            //anim.Play();
        }
        else
        {
            RefreshIndicator(newValue);
            //Debug.LogError("Tried to stack values from two damage indicators together but the values failed to parse [Belt]");
        }
        transform.SetAsLastSibling();
        anim.Stop();
        anim.Play();
    }


    IEnumerator PlayIndicatorCo(System.Func<bool> stopRule, Vector2 startOffset, Vector2 endOffset)
    {
        float durationLeft = movement.Duration;
        Vector2 curOffset;

        while (durationLeft != 0f && !stopRule())
        {
            durationLeft = Mathf.Clamp(durationLeft - Time.deltaTime, 0f, 10000f);
            playTime += Time.deltaTime;

            curOffset = Vector2.LerpUnclamped(startOffset, endOffset, movement.MoveCurve.Evaluate(1f - durationLeft / movement.Duration));
            transform.position = Camera.main.WorldToScreenPoint(followParent.position + new Vector3((charOwner.CharInfo.Facing == FacingType.Right ? 1f : -1f) * curOffset.x, curOffset.y));

            //transform.localScale = new Vector3(startingScale.x * movement.ScaleChangeCurve.Evaluate(1f - durationLeft / movement.Duration), startingScale.y, startingScale.z);

            if (LastFacingType != charOwner.CharInfo.Facing)
                RefreshIndicator();

            canv.alpha = Mathf.Lerp(0f, 1f, movement.AplhaChangeCurve.Evaluate(1f - durationLeft / movement.Duration));

            yield return null;
        }
        ResetIndicator();
    }

    void ResetIndicator()
    {
        playTime = 999f;
        gameObject.SetActive(false);
    }

    public void StopIndicator()
    {
        StopAllCoroutines();
        ResetIndicator();
    }
}
