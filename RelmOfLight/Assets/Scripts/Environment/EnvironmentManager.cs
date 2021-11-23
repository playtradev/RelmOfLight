using PlaytraGamesLtd;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    public Vector2Int BattleFieldSize;
    public List<ScriptableObjectGridStructure> GridStructures = new List<ScriptableObjectGridStructure>();
    public bool isChangeGridStructure = false;

    public float defaultTransitionTime = 3f;
    public Transform fightGridMaster;
    public int currentGridIndex = 0;
    [HideInInspector] public int prevGrid;
    public FightGrid[] fightGrids;

    public CameraStageInfoScript CameraStage;
	public ScriptableObjectGridStructure BaseGrid;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
		ChangeGridStructure(BaseGrid, 0, true);
    }

    //Setting up the camera position and new grid stuff
    public void ChangeGridStructure(ScriptableObjectGridStructure gridStructure,int stageIndex, bool moveCameraInternally, float cameraChangeDuration = 0f)
    {
        GridManagerScript.Instance.ResetGrid();
        isChangeGridStructure = false;
       // CameraManagerScript.Instance.ChangeFocusToNewGrid(CameraStage.CameraInfo.Where(r=> r.StageIndex == (stageIndex != -1 ? stageIndex : currentGridIndex)).First(), cameraChangeDuration, moveCameraInternally);
        GridManagerScript.Instance.SetupGrid(gridStructure);
        
    }
}

