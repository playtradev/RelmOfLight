using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;

public class UIBattleFieldManager : MonoBehaviour
{
    public static UIBattleFieldManager Instance;

    [Header("Basic Setup")]
    public float UIDamageMultiplier = 1f;

    [Header("Burst Indicating [New System]"), Space(10)]
    public GameObject BurstIndicatorPrefab = null;
    [HideInInspector] public List<BurstIndicator> BurstIndicators = new List<BurstIndicator>();
    [Tooltip("The Start Position of the text element, based on the origin of the spine skeleton of the character")] public Vector2 Burst_StartOffset = Vector2.zero;
    [Tooltip("The End Position of the text element, based on the origin of the spine skeleton of the character")] public Vector2 Burst_EndOffset = Vector2.zero;
    
    public BurstIndicationMovementTypeClass[] Burst_MovementTypes = new BurstIndicationMovementTypeClass[0];
    public bool HasBurstMovementType(string ID) => Burst_MovementTypes.Where(r => r.MovementTypeID == ID.ToUpper()).FirstOrDefault() != null;
    public BurstIndicationStyleClass[] Burst_StyleTypes = new BurstIndicationStyleClass[0];
    public bool HasBurstStyleType(string ID) => Burst_StyleTypes.Where(r => r.TypeID == ID.ToUpper()).FirstOrDefault() != null;
    public BurstBattleFieldEventLinkInfoClass[] Burst_BattleFieldEventTypes = new BurstBattleFieldEventLinkInfoClass[0];


    [Header("Combo Indicating")]
    public bool UseComboSystem = false;
    public GameObject ComboIndicator;
    private List<GameObject> ComboIndicators = new List<GameObject>();


    [Header("Aiming Indicating")]
    [SerializeField] protected Transform AimIndicator_BoundryLine_Container = null;
    [SerializeField] protected Transform AimIndicator_BulletPath_Container = null;
    public bool UseAttackAimIndicating = true;
    protected List<AimIndicatorClass> AimIndicators = new List<AimIndicatorClass>();
    [SerializeField] protected GameObject AimIndicator_BoundryLine_RendererPrefab = null;
    protected List<LineRenderer> AimIndicator_BoundryLine_Renderers = new List<LineRenderer>();
    [SerializeField] protected GameObject AimIndicator_BulletPath_RendererPrefab = null;
    protected List<LineRenderer> AimIndicator_BulletPath_Renderers = new List<LineRenderer>();


    [Header("Battlefield Indicating"), Header("OBSOLETE SYSTEMS_________________"), Space(40)]
    public List<BattleFieldIndicatorMaterialClass> Materials = new List<BattleFieldIndicatorMaterialClass>();
    public GameObject BaseBattleFieldIndicator;
    TextMeshProUGUI currentTMP;
    GameObject b;
    BattleFieldIndicatorMaterialClass currentBFIM;
    private List<GameObject> BattleFieldIndicators = new List<GameObject>();

    private Camera mCamera;
    bool setupIsComplete = false;

    private void Awake()
    {
        Instance = this;
        mCamera = Camera.main;
    }

    public void SetupCharListener(BaseCharacter charOwner, bool playerChar)
    {
        charOwner.StatsChangedEvent += CharOwner_StatsChangedEvent;
        charOwner.EffectChangedEvent += CharOwner_EffectChangedEvent;
        
        charOwner.CurrentCharShieldDepletedEvent += CharOwner_ShieldDepletedEvent;

        if (!playerChar)
        {
            charOwner.CurrentCharEtherDepletedEvent += CharOwner_EtherDepletedEvent;
        }
    }


    #region BURST INDICATOR SYSTEM_____BURST INDICATOR SYSTEM_____BURST INDICATOR SYSTEM_____BURST INDICATOR SYSTEM_____BURST INDICATOR SYSTEM

    private void CharOwner_StatsChangedEvent(float value, BattleFieldIndicatorType changeType, BaseCharacter charOwner)
    {
        if(value != 0)
        {
            StatsChangedTypeHandler(Mathf.CeilToInt(value * UIDamageMultiplier).ToString(), changeType, charOwner);
        }
    }
    
    private void CharOwner_EffectChangedEvent(string value, BattleFieldIndicatorType changeType, BaseCharacter charOwner)
        => PlayBurstIndicator(changeType.ToString().ToUpper(), value, charOwner);
    public void CharOwner_EtherDepletedEvent(BaseCharacter baseChar)
        => PlayBurstIndicator("WARNING", "NO ETHER", baseChar);
    public void CharOwner_ShieldDepletedEvent(BaseCharacter baseChar)
        => PlayBurstIndicator("WARNING", "NO SHIELD", baseChar);

    void StatsChangedTypeHandler(string damageText, BattleFieldIndicatorType changeType, BaseCharacter charOwner)
    {
        BurstBattleFieldEventLinkInfoClass thaLink = Burst_BattleFieldEventTypes.Where(r => r.Type == changeType).FirstOrDefault();

        if (thaLink == null)
        {
            Debug.LogError("Tried to assign a style to the battle UI text for <b>" + changeType.ToString() + "</b>, but could not find a matching type in the INDICATOR CANVAS, PLEASE SET IT UP FIRST [Belt]");
            return;
        }
        switch (thaLink.ValueConcatination)
        {
            case BurstBattleFieldEventLinkInfoClass.ValueConcatType.NO_VALUE:
                damageText = thaLink.BurstText;
                break;
            case BurstBattleFieldEventLinkInfoClass.ValueConcatType.VALUE_AFTER_TEXT:
                damageText = thaLink.BurstText + " " + damageText;
                break;
            case BurstBattleFieldEventLinkInfoClass.ValueConcatType.VALUE_BEFORE_TEXT:
                damageText += " " + thaLink.BurstText;
                break;
            default:
                break;
        }
        PlayBurstIndicator(thaLink.StyleToUseID, damageText, charOwner);
    }


    public void PlayBurstIndicator(string typeID, string message, BaseCharacter character)
    {

        BurstIndicationStyleClass style = GetBurstStyleInfo(typeID);
        BurstIndicationMovementTypeClass movement = Burst_MovementTypes.Where(r => r.MovementTypeID == style.MovementTypeRef).FirstOrDefault();
        if ((style.OverrideMovementQueueDuration ? style.OverrideQueueDuration : movement.QueueDelay) != 0)
        {
            QueuedIndicators.Add(new BurstIndicatorQueueInfoClass(typeID, message, character, style, movement));
            if (!QueuingBurstIndicators)
                StartCoroutine(BurstIndicatorQueueCo());
            return;
        }

        GetFreeBurstIndicators(style, character).PlayIndicator(
                style,
                movement,
                character,
                Burst_StartOffset,
                Burst_EndOffset,
                message,
                () => !character.isActiveAndEnabled
            );
    }

    List<BurstIndicatorQueueInfoClass> QueuedIndicators = new List<BurstIndicatorQueueInfoClass>();
    bool QueuingBurstIndicators = false;
    IEnumerator BurstIndicatorQueueCo()
    {
        QueuingBurstIndicators = true;

        while (QueuedIndicators.Count > 0)
        {
            if(QueuedIndicators.Count == 0 || QueuedIndicators[0].Character == null)
            {

            }
            if (QueuedIndicators[0].Character.isActiveAndEnabled)
            {
                BurstIndicatorQueueInfoClass currentQueuedIndicator = QueuedIndicators[0];
                QueuedIndicators.RemoveAt(0);

                GetFreeBurstIndicators(currentQueuedIndicator.Style, currentQueuedIndicator.Character).PlayIndicator(
                        currentQueuedIndicator.Style,
                        currentQueuedIndicator.Movement,
                        currentQueuedIndicator.Character,
                        Burst_StartOffset,
                        Burst_EndOffset,
                        currentQueuedIndicator.Message,
                        () => !currentQueuedIndicator.Character.isActiveAndEnabled
                    );
                float i = currentQueuedIndicator.Style.OverrideMovementQueueDuration ? currentQueuedIndicator.Style.OverrideQueueDuration : currentQueuedIndicator.Movement.QueueDelay;
                while(i > 0f)
                {
                    i -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                QueuedIndicators.RemoveAt(0);
            }
        }

        QueuingBurstIndicators = false;
    }

    BurstIndicationStyleClass GetBurstStyleInfo(string typeID)
    {
        BurstIndicationStyleClass info = Burst_StyleTypes.Where(r => r.TypeID == typeID).FirstOrDefault();
        if (info == null)
        {
            Debug.LogError("BURST INDICATOR STYLE WITH ID *" + typeID + "* DOES NOT EXIST, ABORTING");
            return null;
        }
        return info;
    }

    BurstIndicator GetFreeBurstIndicators(BurstIndicationStyleClass style = null, BaseCharacter character = null)
    {
        BurstIndicator res = BurstIndicators.Where(r => !r.isActiveAndEnabled || (style != null && style.TypeID == r.styleTypeID && r.charOwner == character && style.StackUntilProgress > 0f && style.StackUntilProgress > r.PlayProgress)).FirstOrDefault();
        if (res == null)
        {
            res = Instantiate(BurstIndicatorPrefab, transform).GetComponentInChildren<BurstIndicator>();
            BurstIndicators.Add(res);
        }
        res.gameObject.SetActive(true);
        res.transform.SetAsLastSibling();
        return res;
    }

    #endregion


    #region Combo
    public void DisplayComboStyleSplasher(string text, Vector3 pos, float scaler, Color color, bool animateLong, out float animLength)
    {
        if (!UseComboSystem)
        {
            animLength = 0f;
            return;
        }


        GameObject cI = ComboIndicators.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (cI == null)
        {
            cI = Instantiate(ComboIndicator, transform);
            ComboIndicators.Add(cI);
        }

        List<Color> colors = new List<Color>();
        //colors.Add(color * 0.3f);
        colors.Add(color);

        TextMeshProUGUI[] thaScaredtexts = cI.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < thaScaredtexts.Length; i++)
        {
            thaScaredtexts[i].color = colors[i];
            thaScaredtexts[i].text = text;
        }

        cI.transform.localScale = Vector3.one * scaler;
        cI.transform.position = Camera.main.WorldToScreenPoint(pos);

        cI.GetComponent<Animation>().clip = cI.GetComponent<Animation>().GetClip(animateLong ? "ComboSplash_Long":"ComboSplash");
        animLength = cI.GetComponent<Animation>().clip.length;

        StartCoroutine(DisplayComboSplashAnim(cI));
    }

    IEnumerator DisplayComboSplashAnim(GameObject obj)
    {
        obj.SetActive(true);
        Animation anim = obj.GetComponent<Animation>();
        anim.Play();

        while (anim.isPlaying)
        {
            yield return null;
        }
        obj.SetActive(false);
    }

    #endregion


    #region AIM INDICATOR SYSTEM____AIM INDICATOR SYSTEM____AIM INDICATOR SYSTEM____AIM INDICATOR SYSTEM____AIM INDICATOR SYSTEM____AIM INDICATOR SYSTEM

    public void StartSkillDisplay(ControllerType controllerType, AttackInputType atk)
    {
        if (!UseAttackAimIndicating || BattleManagerScript.Instance == null || BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || BattleManagerScript.Instance.CurrentSelectedCharacters[controllerType] == null)
            return;

        StopSkillIndicatorDisplay(controllerType);
        AimIndicatorClass aimClass = new AimIndicatorClass(BattleManagerScript.Instance.CurrentSelectedCharacters[controllerType], atk);
        aimClass.Coroutine = aimClass.DisplayAttackTrajectories();
        StartCoroutine(aimClass.Coroutine);
        AimIndicators.Add(aimClass);
    }

    public void StopSkillIndicatorDisplay(ControllerType controllerType)
    {
        if (!UseAttackAimIndicating || BattleManagerScript.Instance == null || BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || BattleManagerScript.Instance.CurrentSelectedCharacters[controllerType] == null)
            return;

        AimIndicatorClass aimClass = AimIndicators.Where(r => r.OriginChar_PlayerControllerType == controllerType).FirstOrDefault();
        if (aimClass == null)
            return;

        AimIndicators.Remove(aimClass);
        if(aimClass.Coroutine != null)
        {
            StopCoroutine(aimClass.Coroutine);
        }
        aimClass.ClearCoroutine();
    }

    LineRenderer Get_FreeBoundryLineRenderer()
    {
        LineRenderer res = AimIndicator_BoundryLine_Renderers.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (res == null)
        {
            res = Instantiate(AimIndicator_BoundryLine_RendererPrefab, AimIndicator_BoundryLine_Container != null ? AimIndicator_BoundryLine_Container : transform).GetComponent<LineRenderer>();
            res.material = Instantiate(res.material);
            AimIndicator_BoundryLine_Renderers.Add(res);
        }
        return res;
    }

    public LineRenderer DrawBoundryLineRenderer(GridManagerScript.BoundryLine boundryLine)
    {
        LineRenderer res = Get_FreeBoundryLineRenderer();
        res.transform.position = Vector3.zero;
        res.positionCount = boundryLine.LineCoordsNoDupes.Length;
        res.SetPositions(boundryLine.LineCoordsNoDupes);
        res.gameObject.SetActive(true);
        return res;
    }

    LineRenderer Get_FreeBulletPathRenderer()
    {
        LineRenderer res = AimIndicator_BulletPath_Renderers.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (res == null)
        {
            res = Instantiate(AimIndicator_BulletPath_RendererPrefab, AimIndicator_BulletPath_Container != null ? AimIndicator_BulletPath_Container : transform).GetComponent<LineRenderer>();
            res.material = Instantiate(res.material);
            AimIndicator_BulletPath_Renderers.Add(res);
        }
        return res;
    }

    public LineRenderer DrawBulletPathRenderer(BulletScript.BulletPath path)
    {
        LineRenderer res = Get_FreeBulletPathRenderer();
        res.transform.position = Vector3.zero;
        res.positionCount = path.PathNodes.Length;
        res.SetPositions(path.PathNodes);
        res.gameObject.SetActive(true);
        return res;
    }

    #endregion



  


    private void OnDrawGizmos()
    {
        foreach (AimIndicatorClass aimIndicator in AimIndicators)
        {
            aimIndicator.OnDrawGizmos();
        }
    }

    private void OnValidate()
    {
        foreach (BurstIndicationMovementTypeClass t in Burst_MovementTypes) t.OnValidate();
        foreach (BurstIndicationStyleClass t in Burst_StyleTypes) t.OnValidate(HasBurstMovementType(t.MovementTypeRef));
    }
}


[System.Serializable]
public class BattleFieldIndicatorMaterialClass
{
    [HideInInspector]public string name;
    public BattleFieldIndicatorType BattleFieldIndicatorT;
    public Material Mat;

    public BattleFieldIndicatorMaterialClass()
    {

    }

    public BattleFieldIndicatorMaterialClass(BattleFieldIndicatorType battleFieldIndicatorT, Material mat)
    {
        BattleFieldIndicatorT = battleFieldIndicatorT;
        Mat = mat;
    }
}

[System.Serializable]
public class DepletionIndicationInfoClass
{
    [HideInInspector] public string Name = "";

    public string ID = "NO_ID";
    public string Title = "NO TITLE GIVEN";
    public Color FontColor = Color.white;
    public Color OutlineColor = Color.black;
    public Color ShadowColor = Color.black;
    [Tooltip("The comparer value that dictates when the popup fades. Could be duration, could be health percentage to check, depends on the context")][Range(1f,100f)] public float DepletionPopOutThreshold = 100f;

    public void OnValidate()
    {
        Name = ID + "  ||  " + Title;
    }

    public DepletionIndicationInfoClass(string title)
    {
        Title = title;
    }
}

/// <summary>
/// The class used to inject style into the Burst Indicator Object
/// </summary>
[System.Serializable]
public class BurstIndicationStyleClass
{
    [HideInInspector] public string Name = "";

    [Header("Style")]
    public string TypeID = "NO_ID";
    public Color FontColor = Color.white;
    public Color OutlineColor = Color.black;
    public Color ShadowColor = Color.black;
    public int FontSize = 60;

    [Header("Image")]
    public Sprite AccompanyingImage = null;
    public enum ImageSideType { AfterText, BeforeText, Both }
    [ConditionalField("_UsingImage")] public ImageSideType ImageSide = ImageSideType.AfterText;
    [ConditionalField("_UsingImage")] public Color ImageColor = Color.white;
    
    [Header("Movement")]
    [Tooltip("The Movement Type ID of the desired move system")][SerializeField] protected string movementTypeRef = "Default";
    public string MovementTypeRef => movementTypeRef.ToUpper();
    public bool OverrideMovementQueueDuration = false;
    [ConditionalField("OverrideMovementQueueDuration")] public float OverrideQueueDuration = 0f;

    [Header("Stacking")]
    [Tooltip("Declares whether the values given to indicators of this type should be added together at all")][SerializeField] protected bool Stackable = false;
    [ConditionalField("Stackable")][Tooltip("Until what point within the duration of this indicator, may the values from an incoming play stack ontop the current one")][Range(0f, 1f)] public float StackUntilProgress = 0f;

    public void OnValidate(bool HasBurstMovementType)
    {
        Name = TypeID != "NO_ID" && TypeID != "" ? TypeID + (HasBurstMovementType ? "" : " - THE SPECIFIED MOVEMENT TYPE DOES NOT EXIST") : "NO TYPE ASSIGNED!!!!!";
        StackUntilProgress = Stackable ? StackUntilProgress : 0f;
        FontSize = FontSize == 0 ? 60 : FontSize;
    }
}

/// <summary>
/// The class used to inject style into the Burst Indicator Object
/// </summary>
[System.Serializable]
public class BurstIndicationMovementTypeClass
{
    [HideInInspector] public string Name = "";

    [SerializeField] protected string movementTypeID = "NO_ID";
    public string MovementTypeID => movementTypeID.ToUpper();
    public AnimationCurve MoveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve AplhaChangeCurve = AnimationCurve.Constant(0f, 1f, 1f);
    public AnimationCurve ScaleChangeCurve = AnimationCurve.Constant(0f, 1f, 1f);
    public float Duration = 0.7f;
    public float QueueDelay = 0f;

    public void OnValidate()
    {
        Name = movementTypeID != "NO_ID" && movementTypeID != "" ? MovementTypeID : "NO ID ASSIGNED!!!!!";
    }
}

/// <summary>
/// A class containing the information for a queued play of a burst indicator
/// </summary>
public class BurstIndicatorQueueInfoClass
{
    public string TypeID;
    public string Message;
    public BaseCharacter Character;
    public BurstIndicationStyleClass Style;
    public BurstIndicationMovementTypeClass Movement;

    public BurstIndicatorQueueInfoClass(string typeID, string message, BaseCharacter character, BurstIndicationStyleClass style, BurstIndicationMovementTypeClass movement)
    {
        TypeID = typeID;
        Message = message;
        Character = character;
        Style = style;
        Movement = movement;
    }
}

/// <summary>
/// A way of letting the designer link actions coming in with a design of their choosing
/// </summary>
[System.Serializable]
public class BurstBattleFieldEventLinkInfoClass
{
    [HideInInspector] public string Name;

    public BattleFieldIndicatorType Type;
    [ConditionalField("ValueConcatination", true, ValueConcatType.JUST_VALUE)] public string BurstText;
    public enum ValueConcatType { NO_VALUE, VALUE_AFTER_TEXT, VALUE_BEFORE_TEXT, JUST_VALUE }
    [Tooltip("Where does the damage value appear in relation to the flavour text")]public ValueConcatType ValueConcatination = ValueConcatType.NO_VALUE;
    [SerializeField] protected string styleToUseID = "";
    public string StyleToUseID => styleToUseID.ToUpper();
    [Tooltip("This audio will attempt to trigger from the Universal Audio Profile, if it doesn't exist with this ID, it will not play")] public string EventAudioID = "";
    
  
}

[System.Serializable]
public class ElementalIndicatorLevel
{
    [HideInInspector] public string Name = "";
    public float MultiplierThreshold = 1f;
    public string TextToDisplay = "";
    public Color TextColor = Color.white;
    public Color TextOutlineColor = Color.black;
    public float durationOfPopup = 1f;
    public float scaleMultiplier = 1f;

    public void OnValidate()
    {
        Name = MultiplierThreshold.ToString("F2") + " || " + TextToDisplay;
    }
}

[System.Serializable]
public class AimIndicatorClass
{
    //CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO____CHAR INFO
    private BaseCharacter _OriginChar = null;
    public BaseCharacter OriginChar
    {
        private get => _OriginChar;
        set
        {
            _OriginChar = value;
			OriginChar_PlayerControllerType = _OriginChar != null ? BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value != null && r.Value == _OriginChar).First().Key : ControllerType.None;
        }
    }
    public ControllerType OriginChar_PlayerControllerType { private set; get; } = ControllerType.None;
    //___________________________________________________________________________________________________________________________________________


    //ATTACK INFO_____ATTACK INFO_____ATTACK INFO_____ATTACK INFO_____ATTACK INFO_____ATTACK INFO_____ATTACK INFO_____ATTACK INFO
    private ScriptableObjectAttackBase _AttackInfo = null;
    private ScriptableObjectAttackBase AttackInfo
    {
        get => _AttackInfo;
        set
        {
            _AttackInfo = value;
            AttackInfo_InputType =
                _AttackInfo != null ?
                _AttackInfo.AttackInput == AttackInputType.Strong ? CharacterActionType.Strong :
                _AttackInfo.AttackInput == AttackInputType.Skill1 ? CharacterActionType.Skill1 :
                _AttackInfo.AttackInput == AttackInputType.Skill2 ? CharacterActionType.Skill2 :
                _AttackInfo.AttackInput == AttackInputType.Skill3 ? CharacterActionType.Skill3 :
                CharacterActionType.None : CharacterActionType.None;
        }
    }
    protected CharacterActionType AttackInfo_InputType = CharacterActionType.None;
    //____________________________________________________________________________________________________________________________


    //RUNTIME VARS___RUNTIME VARS___RUNTIME VARS___RUNTIME VARS___RUNTIME VARS___RUNTIME VARS___RUNTIME VARS___RUNTIME VARS___RUNTIME VARS

    bool CanDisplayAttack => AttackInfo != null && OriginChar != null && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle && OriginChar.IsOnField && OriginChar.CharActionlist.Contains(AttackInfo_InputType);
    BattleTileScript[] Trajectories_BattleTiles = new BattleTileScript[0];
    GridManagerScript.BoundryLine[] Trajectories_BattleTiles_BoundryLines = new GridManagerScript.BoundryLine[0];
    List<LineRenderer> Trajectories_BattleTiles_LineRenderers = new List<LineRenderer>();
    BulletScript.BulletPath[] Trajectories_BulletPaths = new BulletScript.BulletPath[0];
    List<LineRenderer> Trajectories_BulletPath_LineRenderers = new List<LineRenderer>();


    //____________________________________________________________________________________________________________________________________




    public AimIndicatorClass(BaseCharacter OriginChar, AttackInputType AttackInfoType)
    {
        this.OriginChar = OriginChar;
        if(OriginChar.CharInfo == null)
        {
            Debug.LogError("CHECK THIS WITH A DEBUGGER");
        }
		this.AttackInfo = OriginChar.CharInfo.CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r => r != null && r.AttackInput == AttackInfoType && !r.name.ToLower().Contains("enemy"));
        ClearCoroutine();
    }

    public IEnumerator Coroutine = null;
    public IEnumerator DisplayAttackTrajectories()
    {
        //Debug.LogError("StartedAimingCo");
        if (!CanDisplayAttack)
        {
            ClearCoroutine();
            yield break;
        }

        yield return BattleManagerScript.Instance.WaitFor(0.5f, () => false, () => !CanDisplayAttack);

        if (!CanDisplayAttack)
        {
            ClearCoroutine();
            yield break;
        }

        spineTBasedLastVisualPos = Vector3.negativeInfinity;
        tileBasedLastVisualPos = new Vector2Int(-1, -1);
        while (CanDisplayAttack)
        {
            if(OriginChar.GridPosition != tileBasedLastVisualPos)
            {
                ClearTileBasedVisuals();
                SetupTileBasedVisuals();
            }
            if (OriginChar.spineT.position != spineTBasedLastVisualPos)
            {
                ClearSpineTBasedVisuals();
                SetupSpineTBasedVisuals();
            }
            yield return null;
        }

        ClearCoroutine();
    }

    public void ClearCoroutine()
    {
        //Debug.LogError("StoppedAimingCo");
        ClearTileBasedVisuals();
        ClearSpineTBasedVisuals();
        Coroutine = null;
        UIBattleFieldManager.Instance.StopSkillIndicatorDisplay(OriginChar_PlayerControllerType);
    }

    Vector2Int tileBasedLastVisualPos = new Vector2Int(-1, -1);
    bool tileBasedVisualsActive = false;
    protected void SetupTileBasedVisuals()
    {
        if (!CanDisplayAttack)
            return;
        tileBasedLastVisualPos = OriginChar.GridPosition;
        tileBasedVisualsActive = true;
        //Debug.LogError("Started Aim Visuals");
        Trajectories_BattleTiles = OriginChar.currentAttackProfile.GetAttackTargetBattleTileScripts(OriginChar.currentInputProfile.nextAttackPos, new CurrentAttackInfoClass(AttackInfo, OriginChar.currentAttackProfile), true);
        Trajectories_BattleTiles_BoundryLines = GridManagerScript.Instance.GetBattleTilesBoundryLines(Trajectories_BattleTiles);
        foreach (GridManagerScript.BoundryLine boundryLine in Trajectories_BattleTiles_BoundryLines)
        {
            Trajectories_BattleTiles_LineRenderers.Add(UIBattleFieldManager.Instance.DrawBoundryLineRenderer(boundryLine));
        }
    }

    protected void ClearTileBasedVisuals()
    {
        if (!tileBasedVisualsActive)
            return;
        //if there are visuals setup here, clean them
        tileBasedVisualsActive = false;
        //Debug.LogError("Stopped Aim Visuals");
        Trajectories_BattleTiles = new BattleTileScript[0];
        Trajectories_BattleTiles_BoundryLines = new GridManagerScript.BoundryLine[0];
        if (Trajectories_BattleTiles_LineRenderers.Count != 0)
        {
            foreach (LineRenderer lineRenderer in Trajectories_BattleTiles_LineRenderers)
            {
                lineRenderer.gameObject.SetActive(false);
            }
        }
        Trajectories_BattleTiles_LineRenderers = new List<LineRenderer>();
    }


    Vector3 spineTBasedLastVisualPos = Vector3.negativeInfinity;
    bool spineTBasedVisualsActive = false;
    protected void SetupSpineTBasedVisuals()
    {
        if (!CanDisplayAttack)
            return;
        spineTBasedLastVisualPos = OriginChar.spineT.position;
        spineTBasedVisualsActive = true;


        Trajectories_BulletPaths = BulletScript.GetBulletPaths(OriginChar, AttackInfo, true);
        foreach (BulletScript.BulletPath bulletPath in Trajectories_BulletPaths)
        {
            Trajectories_BulletPath_LineRenderers.Add(UIBattleFieldManager.Instance.DrawBulletPathRenderer(bulletPath));
        }
    }

    protected void ClearSpineTBasedVisuals()
    {
        if (!spineTBasedVisualsActive)
            return;
        //if there are visuals setup here, clean them
        spineTBasedVisualsActive = false;

        if (Trajectories_BulletPath_LineRenderers.Count != 0)
        {
            foreach (LineRenderer lineRenderer in Trajectories_BulletPath_LineRenderers)
            {
                lineRenderer.gameObject.SetActive(false);
            }
        }
        Trajectories_BulletPath_LineRenderers = new List<LineRenderer>();
        Trajectories_BulletPaths = new BulletScript.BulletPath[0];
    }

    public void OnDrawGizmos()
    {
        return;
        Gizmos.color = Color.red;
        foreach (BulletScript.BulletPath bulletPath in Trajectories_BulletPaths)
        {
            for (int i = 0; i < bulletPath.PathNodes.Length - 1; i++)
            {
                Gizmos.DrawLine(bulletPath.PathNodes[i], bulletPath.PathNodes[i + 1]);
            }
        }
        //Debug.LogError("Drawing Gizmos...");
        if (!tileBasedVisualsActive) return;
        //Debug.LogError("Visuals Active...");
        //Debug.LogError("Targets Size: " + Trajectories_BattleTiles.Length.ToString() + "...");
        foreach (BattleTileScript tile in Trajectories_BattleTiles)
        {
            //Debug.LogError(tile.Pos);
            Gizmos.DrawCube(tile.transform.position, Vector3.one * 0.5f);
        }
        foreach (GridManagerScript.BoundryLine line in Trajectories_BattleTiles_BoundryLines)
        {
            for (int i = 0; i < line.LineCoords.Length; i+=2)
            {
                Gizmos.DrawLine(line.LineCoords[i], line.LineCoords[i + 1]);
            }
        }
    }
}