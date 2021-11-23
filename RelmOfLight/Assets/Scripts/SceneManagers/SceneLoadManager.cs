using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;
using Spine.Unity;
using MyBox;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Spine;
using PlaytraGamesLtd;
using UnityEngine.Profiling;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

[RequireComponent(typeof(CanvasRenderer))]
public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance;

    #region SAVING INFO

    ///<summary>The application time at the point of the last load</summary>
    float LastLoad_ApplicationTimePoint = 0f;
    ///<summary>The time played in the last save slot</summary>
    float LastLoad_Slot_PlayTime_Seconds = 0f;
    ///<summary>The total time played on the save if you were to save right now</summary>
    float CurrentSave_PlayTime_Seconds => LastLoad_Slot_PlayTime_Seconds + (Time.time - LastLoad_ApplicationTimePoint);

    #endregion

    public List<SpriteAtlasInfoClass> SpriteAtlasses = new List<SpriteAtlasInfoClass>();

    //public bool Debug_Motherlode = false;
    //[ConditionalField("Debug_Motherlode")] [SerializeField] protected KeyCode MotherLodeKeyCode = KeyCode.Keypad5;
    public bool Debug_BlockLoadingSystem = false;
    public bool Debug_BlockSavingSystem = false;

    public GameObject SpineLoaderPrefab = null;


    [HideInInspector] public Dictionary<string, SkeletonDataAsset> SkeletonDatas = new Dictionary<string, SkeletonDataAsset>();

    public GameObject NavigatorPrefab = null;
    public GameObject RewiredPrefab = null;

    public bool UseSteam = true;
    public bool UseCoreSight = true;
    public bool CheatsEnabled = false;
    public bool MouseSupportEnabled = false;

    public GameObject SteamManagerGO = null;
    public GameObject StorageCoreManagerGO = null;
    public GameObject AnalyticsGO = null;
    private GameObject steamGO = null;
    protected string startingSceneID = "";
    [SerializeField] protected const float _loadingFadeTime = 2f;

    [Header("Universal Values")]
    public List<Color> playersColor = new List<Color>();
    public Color[] teamsColor = new Color[2];
    public float[] charLevelThresholds = { 2000, 4500, 7000 };
    public float[] maskLevelThresholds = { 2000, 4500, 7000 };

    //Props___Props___Props___Props___Props___Props___Props___Props
    public class StatsClass
    {
        public float HP = 0f;
        public float Ether = 0f;
        public float Power = 0f;
        public float Speed = 0f;
        public float Shield = 0f;
        public float Defence = 0f;
    }
    public StatsClass maxCharStats = new StatsClass();
    public StatsClass maxMaskAdjustments = new StatsClass();
  

    [Header("Runtime Values")]
    public bool GameStart = false;
    protected bool loadingScene = false;

    public ScriptableObjectSkillMasksContainer MaskContainer;
    public ScriptableObjectStageProfileContainer StageContainer;
    public ScriptableObjectContainingAllCharsSO CharsContainer;
    public ScriptableObjectAchievementsDataContainer AchievementsContainer;

    public ScriptableObjectCharactersEvolution AllcharsEvolution;

    #region Char Evolution and Level Calculations

    public static ElementalType GetElementSigilForEvoStatType(CharacterEvolutionStatType type)
    {
        ElementalType res = ElementalType.Neutral;

        if (type == CharacterEvolutionStatType.HP || type == CharacterEvolutionStatType.HPRegen)
            res = ElementalType.Earth;
        else if (type == CharacterEvolutionStatType.Ether || type == CharacterEvolutionStatType.EtherRegen)
            res = ElementalType.Light;
        else if (type == CharacterEvolutionStatType.DamageWeak || type == CharacterEvolutionStatType.DamageStrong)
            res = ElementalType.Fire;
        else if (type == CharacterEvolutionStatType.SpeedMovement || type == CharacterEvolutionStatType.SigilDropBonus || type == CharacterEvolutionStatType.SigilDropBonus)
            res = ElementalType.Air;
        else if (type == CharacterEvolutionStatType.CriticalChances || type == CharacterEvolutionStatType.Agility)
            res = ElementalType.Dark;
        else if (type == CharacterEvolutionStatType.Armour || type == CharacterEvolutionStatType.EtherRegen || type == CharacterEvolutionStatType.ArmourShieldRegen)
            res = ElementalType.Water;

        return res;
    }

    #endregion


    public ScriptableObjectAllElementData elementData = null;
    public ScriptableObjectAllClassData classData = null;
    [SerializeField] protected string AudioManagerPath = "AudioManagerMk2";

    [Header("Loaded Data")]
 

    public string[] loadedCompletedTutorials = new string[0];
    public string BuildSettingsDate = "";
    public int DateBuildNum = 0;
    GameObject rewired;




    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!CheatsEnabled)
        {
#if UNITY_EDITOR
            CheatsEnabled = true;
#endif
        }
  

        startingSceneID = SceneManager.GetActiveScene().name;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
#if !UNITY_SWITCH && !UNITY_XBOXONE
        GridForceLogType LogType = 
            type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception ? GridForceLogType.Error : 
            type == UnityEngine.LogType.Assert || type == UnityEngine.LogType.Log ? GridForceLogType.Debug : 
            GridForceLogType.Warning;

#endif
    }




 

    private void Start()
    {
        StartCoroutine(InitialLoadCo());
    }

 

 

    IEnumerator InitialLoadCo()
    {
        AsyncOperationHandle<GameObject> audioManagerLoader = Addressables.LoadAssetAsync<GameObject>(AudioManagerPath);
        while (!audioManagerLoader.IsDone)
            yield return null;
        Instantiate(audioManagerLoader.Result, Vector3.zero, Quaternion.identity);

        SettingsManager.Instance.ApplySettings();
        yield return null;
        yield return new WaitForSecondsRealtime(0.5f);
        GameStart = true;
    }






    public void LoadScene(string sceneToLoad, string sceneToDeload = null, float gracePeriod = 2f)
    {
        LoadScenes(sceneToLoad == null ? new List<string>() : new List<string> { sceneToLoad },
            sceneToDeload == null ? new List<string>() : new List<string> { sceneToDeload });
    }

    public void LoadScenes(List<string> scenesToLoad, List<string> scenesToDeload = null, float gracePeriod = 2f)
    {
        if (currentlyLoading) return;

        StartCoroutine(LoadSceneCo(scenesToLoad == null ? new string[0] : scenesToLoad.ToArray(),
           scenesToDeload == null ? new string[0] : scenesToDeload.ToArray(), gracePeriod: gracePeriod));
    }




    [HideInInspector] public bool currentlyLoading = false;
    int scenesLoading = 0;
    IEnumerator LoadSceneCo(string[] scenesToLoad, string[] scenesToDeload, bool instantCurtain = false, float gracePeriod = 2f)
    {
        currentlyLoading = true;
        scenesLoading = scenesToLoad.Length + scenesToDeload.Length;
        ReleaseAddressables();

        AsyncOperation load = SceneManager.LoadSceneAsync(scenesToLoad[0]);

        if (scenesToDeload.Length > 0)
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(scenesToDeload[0]));
            while (!unload.isDone)
            {
                //Debug.Log(asyncs[0].progress);
                yield return null;
            }

        }

        while (!load.isDone)
        {
            //Debug.Log(asyncs[0].progress);
            yield return null;
        }


        yield return null;

        yield return new WaitForSecondsRealtime(gracePeriod);

        bool containsBattleScene = false;
        foreach (string scene in scenesToLoad)
        {
            if (scene.Contains("BattleScene"))
            {
                containsBattleScene = true;
            }
            else if (scene.Contains("Menu"))
            {
            }
        }
        //dssdfsdfsdfsdfsdfsd
        yield return new WaitForSecondsRealtime(gracePeriod);

		StartCoroutine(EnableInputPlayer());

        yield return new WaitForSecondsRealtime(0.3f);

        currentlyLoading = false;
    }


    public void ReleaseAddressables()
    {
        foreach (var item in SpriteAtlasses)
        {
            Addressables.Release(item.DataHolder);
        }
        SpriteAtlasses.Clear();
    }


    IEnumerator EnableInputPlayer()
    {
        yield return new WaitForSecondsRealtime(1);
        GameStart = true;
    }



    public void SceneLoadSeqComplete(AsyncOperation asy)
    {
        scenesLoading--;
    }
}


[System.Serializable]
public class GridForceBuildSettings
{
    [Header("Supported Platforms")]
    public bool CoreSightEnabled = false;
    public bool SteamEnabled = false;
    [Header("Misc"), Space(10)]
    public bool EnableMouseSupport = false;
    public bool CheatsEnabled = false;


    public GridForceBuildSettings(bool CheatsEnabled, bool CoreSightEnabled, bool SteamEnabled, bool EnableMouseSupport)
    {
        this.CheatsEnabled = CheatsEnabled;
        this.CoreSightEnabled = CoreSightEnabled;
        this.SteamEnabled = SteamEnabled;
        this.EnableMouseSupport = EnableMouseSupport;
    }
}

public class SpriteAtlasInfoClass
{
    public string Path;
    public SpriteAtlas Atlas;
    public AsyncOperationHandle<SpriteAtlas> DataHolder;

    public SpriteAtlasInfoClass()
    {
    }
    public SpriteAtlasInfoClass(string path, SpriteAtlas atlas)
    {
        Path = path;
        Atlas = atlas;
    }
    public SpriteAtlasInfoClass(string path, SpriteAtlas atlas, AsyncOperationHandle<SpriteAtlas> dataHolder)
    {
        Path = path;
        Atlas = atlas;
        DataHolder = dataHolder;
    }
}
