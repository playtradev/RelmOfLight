using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Threading;

public class BuffIcon2 : BuffIcon
{
    [Header("Exclusive to Two")]
    public GameObject statusObject;
    public Color BuffColor = Color.blue;
    public Color DebuffColor = Color.blue;
    protected SpriteRenderer[] sprites = null;
    public Transform ArrowContainer;

    protected override void Awake()
    {
        statusIcon.color = Color.clear;
        anim = statusObject.GetComponentInChildren<Animation>();
        sprites = statusObject.GetComponentsInChildren<SpriteRenderer>();
        statusObject.SetActive(false);
    }


    public override void InitiateStatusIcon(BuffDebuffIconClass buffDebuffIcon)
    {
        if (BuffDebuffIcon == buffDebuffIcon) return;
        BuffDebuffIcon = buffDebuffIcon;

        //Switch to either buff/debuff icon background
        statusIcon.sprite = buffDebuffIcon.Icon;
        foreach (SpriteRenderer sprite in sprites)
            sprite.color = buffDebuffIcon.Classification == StatusEffectType.Buff ? BuffColor : DebuffColor;

        ArrowContainer.rotation = Quaternion.Euler(0f, 0f, buffDebuffIcon.Classification == StatusEffectType.Buff ? 0f : 180f);

        statusObject.SetActive(true);

        //Start Anims And Shit
        if (isActiveAndEnabled)
        {
            if (CurrentEnterExitProcess != null) StopCoroutine(CurrentEnterExitProcess);
            CurrentEnterExitProcess = InitiatorProcess();
            StartCoroutine(CurrentEnterExitProcess);
        }
        else
        {
            anim.gameObject.SetActive(false);
        }
    }



    public override void InitiateStatusIcon(ScriptableObjectAttackEffect statusEffect)
    {
        if (statusEffect == StatusEffect) return;
        StatusEffect = statusEffect;

        //Switch to either buff/debuff icon background
        statusIcon.sprite = statusEffect.icon;
        foreach (SpriteRenderer sprite in sprites)
            sprite.color = statusEffect.classification == StatusEffectType.Buff ? BuffColor : DebuffColor;

        ArrowContainer.rotation = Quaternion.Euler(0f, 0f, statusEffect.classification == StatusEffectType.Buff ? 0f : 180f);

        statusObject.SetActive(true);

        //Start Anims And Shit
        if (isActiveAndEnabled)
        {
            if (CurrentEnterExitProcess != null) StopCoroutine(CurrentEnterExitProcess);
            CurrentEnterExitProcess = InitiatorProcess();
            StartCoroutine(CurrentEnterExitProcess);
        }
        else
        {
            anim.gameObject.SetActive(false);
        }
    }

    public override void TerminateStatusIcon()
    {
        StatusEffect = null;
        BuffDebuffIcon = null;
        //Start Anims And Shit
        if (isActiveAndEnabled)
        {
            if (CurrentEnterExitProcess != null) StopCoroutine(CurrentEnterExitProcess);
            CurrentEnterExitProcess = TerminatorProcess();
            StartCoroutine(CurrentEnterExitProcess);
        }
        else
        {
            anim.gameObject.SetActive(false);
        }
    }

    IEnumerator CurrentEnterExitProcess = null;
    IEnumerator InitiatorProcess()
    {
        anim.gameObject.SetActive(true);
        anim.Stop();

        anim.clip = anim.GetClip("BuffIcon2-PopIn");
        anim.Play();
        while (isActiveAndEnabled && anim.isPlaying)
        {
            yield return null;
        }

        anim.clip = anim.GetClip("BuffIcon2-Idle");
        anim.Play();
    }

    IEnumerator TerminatorProcess()
    {
        anim.Stop();

        anim.clip = anim.GetClip("BuffIcon2-PopOut");
        anim.Play();
        while (isActiveAndEnabled && anim.isPlaying)
        {
            yield return null;
        }
        anim.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        anim.gameObject.SetActive(false);
    }
}
