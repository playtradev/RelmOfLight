using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

[RequireComponent(typeof(CharacterInfoScript))]
public class CharacterFormesScript : MonoBehaviour
{
    public delegate void FormeChanged(string formeName, CharacterInfoScript charInfo);
    public event FormeChanged FormeChangedEvent;

    BaseCharacter character => transform.parent.gameObject.GetComponentInParent<BaseCharacter>();

    [Header("Configuration")]
    public CharacterForme[] Formes = new CharacterForme[0];
    [Tooltip("Input the ID of the starting forme, if empty, will revert to the first forme in the formes list")] public string StartingFormeID = "";

    [Header("Runtime")]
    protected CharacterForme currentForme = null;
    public CharacterForme CurrentForme => currentForme == null ? Formes.Length > 0 ? Formes[0] : null : currentForme;

    public void Initialize()
    {
        SetDefaultForme(GetComponent<CharacterInfoScript>().GetBaseInfoInjectorClass());
        ChangeForme(StartingFormeID == "" ? Formes[0].ID : StartingFormeID);
    }

    public void SetDefaultForme(BaseInfoInjectorClass info)
    {
        if (Formes.Length <= 0)
            Formes = new CharacterForme[1];

        if (Formes[0] == null)
            Formes[0] =  new CharacterForme();
        Formes[0].ID = Formes[0].ID == "" || Formes[0].ID == "FormeID" ? "default" : Formes[0].ID;
        Formes[0].isDefault = true;


        bool overrideAtks = Formes[0].charInfoOverrides.overrideAttacks;
        List<ScriptableObjectAttackBase> atks = Formes[0].charInfoOverrides.AttacksToAdd;
        bool addAtks = Formes[0].charInfoOverrides.AddToBasicAttacks;

        Formes[0].charInfoOverrides = info;

        Formes[0].charInfoOverrides.overrideAttacks = overrideAtks;
        Formes[0].charInfoOverrides.AttacksToAdd = atks;
        Formes[0].charInfoOverrides.AddToBasicAttacks = addAtks;
    }

    public void ChangeForme(string formeID, float startingHP = -1f)
    {
        CharacterForme formeToChangeTo = Formes.Where(r => r.ID == formeID).FirstOrDefault();
        if(formeToChangeTo == null)
        {
            Debug.LogError("Tried to change " + character.CharInfo.CharacterID.ToString().ToUpper() + " to forme " + formeID.ToUpper() + " but couldn't find a suitable forme, abandoning...");
            return;
        }

        character.SetCharacterStats(formeToChangeTo.charInfoOverrides, startingHP);
        bool formeChanged = CurrentForme == null || !CurrentForme.isEqualTo(formeToChangeTo);
        currentForme = new CharacterForme(formeToChangeTo);

        if (CurrentForme.hasTransitionAnimation && formeChanged)
        {
            character.currentAttackProfile?.InteruptAttack(false);
            if (TransitionAnimationPlayer != null) StopCoroutine(TransitionAnimationPlayer);
            TransitionAnimationPlayer = PlayTransformAnimation(CurrentForme.TransformationAnimationName, CurrentForme.TransformationAnimationFade, CurrentForme.TransformationAnimationSpeed);
            StartCoroutine(TransitionAnimationPlayer);
        }

        FormeChangedEvent?.Invoke(currentForme.ID, character.CharInfo);
    }

    public CharacterForme GetForme(string formeID)
    {
        CharacterForme formeToChangeTo = Formes.Where(r => r.ID == formeID).FirstOrDefault();
        if (formeToChangeTo == null)
        {
            Debug.LogError("Tried to get " + character.CharInfo.CharacterID.ToString().ToUpper() + "'s forme " + formeID.ToUpper() + " but couldn't find a suitable forme, abandoning...");
            return null;
        }
        return formeToChangeTo;
    }

    IEnumerator TransitionAnimationPlayer = null;
    public IEnumerator PlayTransformAnimation(string animationName, float fade = 0f, float speed = 1f)
    {
        character.CanAttack = false;

        if (!character.SpineAnim.HasAnimation(animationName))
        {
            character.CanAttack = true;
            yield break;
        }

        character.SetAnimation(animationName, false, fade, false, false, true);
        character.SpineAnim.SetAnimationSpeed(character.AnimSpeed * speed);

        if (CurrentForme.TransitionEffect == null)
        {
            character.CanAttack = true;
            yield break;
        }

        while (character.SpineAnim.CurrentAnim != animationName) yield return null;

        character.Buff_DebuffCo(character, CurrentForme.TransitionEffect, null);

        while (character.SpineAnim.CurrentAnim == animationName) yield return null;

        if (character.GetBuffDebuff(CurrentForme.TransitionEffect.StatsToAffect) == null)
        {
            character.CanAttack = true;
            yield break;
        }

        character.GetBuffDebuff(CurrentForme.TransitionEffect.StatsToAffect).Stop_Co = true;
        character.CanAttack = true;
    }

    private void OnValidate()
    {
        if (Formes.Length <= 0)
            SetDefaultForme(new BaseInfoInjectorClass());

        //if (Formes[0] == null || Formes[0].isEqualTo(new CharacterForme()) || !Formes[0].charInfoOverrides.isEqualTo(GetComponent<CharacterInfoScript>().GetBaseInfoInjectorClass())) 
        SetDefaultForme(GetComponent<CharacterInfoScript>().GetBaseInfoInjectorClass());

        foreach (CharacterForme forme in Formes)
        {
            forme.OnValidate();
        }
    }
}

[System.Serializable]
public class CharacterForme
{
    [HideInInspector] public string Name = "";
    [HideInInspector] public bool isDefault = false;
    [Header("General")]
    public string ID = "";

    [Header("Animation Changes")]
    public string AnimationPrefix = "";
    public string AnimationSuffix = "";

    [Header("Transformation")]
    public bool hasTransitionAnimation = false;
    [ConditionalField("hasTransitionAnimation")][Tooltip("The absolute animation name for the transform to be called upon transitioning TO this forme")] public string TransformationAnimationName = "Transformation";
    [ConditionalField("hasTransitionAnimation")] public ScriptableObjectAttackEffect TransitionEffect = null;
    [ConditionalField("hasTransitionAnimation")] public float TransformationAnimationFade = 0f;
    [ConditionalField("hasTransitionAnimation")] public float TransformationAnimationSpeed = 1f;

    [Header("Char Info Changes")]
    public BaseInfoInjectorClass charInfoOverrides = new BaseInfoInjectorClass();

    public CharacterForme()
    {

    }

    public CharacterForme(CharacterForme copyFrom)
    {
        if (copyFrom == null) return;
        ID = copyFrom.ID;
        isDefault = copyFrom.isDefault;
        charInfoOverrides = copyFrom.charInfoOverrides;
        AnimationSuffix = copyFrom.AnimationSuffix;
        AnimationPrefix = copyFrom.AnimationPrefix;
        hasTransitionAnimation = copyFrom.hasTransitionAnimation;
        TransformationAnimationName = copyFrom.TransformationAnimationName;
        TransitionEffect = copyFrom.TransitionEffect;
    }

    public bool isEqualTo(CharacterForme forme)
    {
        if (forme.isDefault != isDefault) return false;
        if (forme.ID != ID) return false;
        //if (forme.AnimationPrefix != AnimationPrefix) return false;
        //if (forme.AnimationSuffix != AnimationSuffix) return false;
        if (!charInfoOverrides.isEqualTo(forme.charInfoOverrides)) return false;
        return true;
    }



    public void OnValidate()
    {
        Name = ID;
    }
}