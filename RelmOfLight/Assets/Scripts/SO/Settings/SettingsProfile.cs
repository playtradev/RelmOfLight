using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Settings Profile", menuName = "ScriptableObjects/SettingsProfile")]
public class SettingsProfile : ScriptableObject
{
    public Vector2Int ScreenResolution = new Vector2Int(1920, 1080);
    public FullScreenMode FullScreenType = FullScreenMode.Windowed;
    public SpriteAtlasQualityType TextureQuality = SpriteAtlasQualityType.Mid;

    public bool TipsEnabled = true;
    public bool BlurEnabled = true;

    [Range(0f, 1f)] public float MasterVolume = 0.8f;
    [Range(0f, 1f)] public float SFXVolume = 0.8f;
    [Range(0f, 1f)] public float MusicVolume = 0.8f;

    [Range(0f, 1f)] public float VibrationIntensity = 1f;
}