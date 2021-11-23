using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "New AI Transition Effect", menuName = "ScriptableObjects/AI/TransitionEffect")]
public class ScriptableObjectAITransitionEffect : ScriptableObject
{
    public AITransitionType TransitionType;

    [Header("Animation")]
    public bool _UseCustomAnimation;
    public bool UseAnimation => TransitionAnimation.ToLower() != "" && TransitionAnimation.ToLower() != "none" && TransitionAnimation.ToLower() != "nomesh";
    public string TransitionAnimation => _UseCustomAnimation ? _TransitionAnimationName : _TransitionAnimationClip.ToString();
    [ConditionalField("UseCustomAnimation", true)] public CharacterAnimationStateType _TransitionAnimationClip;
    [ConditionalField("UseCustomAnimation", false)] [SerializeField] protected string _TransitionAnimationName;
    public float TransitionAnimationSpeed = 1f;

    [Header("Timing")]
    [Tooltip("How long the AI will spend idle for the transition")] public float TransitionWaitDuration;

    [Header("Particles")]
    public ParticlesType ParticleEffect;

}
