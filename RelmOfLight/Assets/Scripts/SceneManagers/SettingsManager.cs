using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance = null;

    public delegate void ResolutionChanged();
    public event ResolutionChanged ResolutionChangedEvent;

    public static SettingsProfile Settings = null;
    public SettingsProfile DefaultSettings = null;
    public bool HasSettings => Settings != null;

    public Vector2Int[] SupportedResolutionsList = new Vector2Int[]
    {
        new Vector2Int(720,480),
        new Vector2Int(960,540),
        new Vector2Int(1280,720),
        new Vector2Int(1600,900),
        new Vector2Int(1920,1080),
        new Vector2Int(2560,1440),
        new Vector2Int(3840,2160),
    };


    private void Awake()
    {
        Instance = this;
        LoadSettings();
        if (Settings == null)
        {
            Settings = Instantiate(DefaultSettings);
            Settings.name = "PlayerSettings";
        }
        ApplySettings();
    }

    private void Start()
    {
        ApplySettings();
    }

    void LoadSettings(string SettingsProfileName = "")
    {
        //Debug.LogError("Attempting to Fetch Settings....");
#if !UNITY_SWITCH && !UNITY_XBOXONE

        //Debug.LogError("Fetching....");
        if (PlayerPrefs.HasKey("Settings"))
        {
            //Debug.LogError("Fetching Success!!");
            Settings = PlaytraGamesLtd.Utils.DeserializeFromString<SettingsProfile>(PlayerPrefs.GetString("Settings"));
        }
        else
        {
            //Debug.LogError("Fetching Failure!!");
        }
#endif

    }

    void SaveSettings(string SettingsProfileName = "")
    {
        //Debug.LogError("Attempting to Save Settings....");
#if !UNITY_SWITCH && !UNITY_XBOXONE

        Debug.LogError("Saving....");
        if (Settings != null)
        {
            //Debug.LogError("Saving Success!!");
            PlayerPrefs.SetString("Settings", PlaytraGamesLtd.Utils.SerializeToString<SettingsProfile>(SettingsManager.Settings));
        }
        else
        {
            //.LogError("Saving Failure!!");
        }
#endif
    }





    /// <summary>
    /// Refresh the settings that need to be set and aren't querried
    /// </summary>
    public void ApplySettings()
    {
        //Debug.LogError("Attempting to Apply Settings...");
        if (!HasSettings)
        {
            //Debug.LogError("Settings NOT APPLIED!!");
            return;
        }

        Screen.SetResolution(Settings.ScreenResolution.x, Settings.ScreenResolution.y, Settings.FullScreenType);
        ResolutionChangedEvent?.Invoke();

        if (CameraManagerScript.PostProlone != null)
            CameraManagerScript.PostProlone.EnableBlur(Settings.BlurEnabled);
    }





    private void OnApplicationQuit() => SaveSettings();
}
