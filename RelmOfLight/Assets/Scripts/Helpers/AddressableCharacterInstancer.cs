using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class AddressableCharacterInstancer
{
    public BaseCharacter Result = null;
    public bool IsWorking = false;
    public IEnumerator Process = null;
    protected bool displayWarning = true;

    public AddressableCharacterInstancer(MonoBehaviour origin, TeamSideInformationClass teamInfoC, Transform parent, TeamSideType teamSide, bool addToList = true, bool showHP = true, bool showEther = true, bool isBornOfWave = false, bool displayWarning = true)
    {
        this.displayWarning = displayWarning;
        StartCreateChar(origin, teamInfoC, parent, teamSide, addToList, showHP, showEther, isBornOfWave);
    }

    void StartCreateChar(MonoBehaviour origin, TeamSideInformationClass teamInfoC, Transform parent, TeamSideType teamSide, bool addToList = true, bool showHP = true, bool showEther = true, bool isBornOfWave = false)
    {
        Result = null;
        IsWorking = true;
        if (Process != null) origin.StopCoroutine(Process);
        Process = GetAddressableCharacter(teamInfoC, parent, teamSide, addToList, showHP, showEther, isBornOfWave);
        origin.StartCoroutine(Process);
    }

    string operationsLog;
    float timeElapsed;
    ScriptableObjectCharacterPrefab charObject;
    AsyncOperationHandle<GameObject> async;
    protected IEnumerator GetAddressableCharacter(TeamSideInformationClass teamInfoC, Transform parent, TeamSideType teamSide, bool addToList, bool showHP, bool showEther, bool isBornOfWave)
    {
        operationsLog = "";
        if (BattleManagerScript.Instance.GetCharacterPrefab(teamInfoC.CharacterName) == null)
        {
            charObject = BattleManagerScript.Instance.CharactersData.CharacterSo.Where(r => r.AbridgedCharInfo != null && r.AbridgedCharInfo.CharacterID == teamInfoC.CharacterName).FirstOrDefault();
            if(charObject == null)
            {
                Debug.LogError("TRIED TO LOAD CHARACTER " + teamInfoC.CharacterName.ToString().ToUpper() + " FROM ADDRESSABLES RUNTIME, BUT CHARACTER DOES NOT EXIST IN ADDRESSABLES SYSTEM\n<color=red>PLEASE ENSURE CHARACTER SCRIPTABLEOBJECT AND PREFAB ARE CORRECTLY UP TO DATE</color> -Belt");
                IsWorking = false;
                yield break;
            }
            operationsLog = "Character " + charObject.AbridgedCharInfo.Name.ToUpper() + "(" + charObject.AbridgedCharInfo.CharacterID + ") was NOT preloaded, attempting now... [CLICK FOR DETAILS]";
            operationsLog += "\n<color=red>NOTE: There is a system in place to load the character from the addressables during the game, but it is NOT recommended as it might cause idling in fungus, idling in the wave, or break the flow of sequenced events in extreme cases!\n<b>FOR BEST PERFORMANCES, PLEASE ASSIGN THE CORRECT STAGE THE CHARACTER IN QUESTION VIA THE ADDRESSABLES</b></color>";
            operationsLog += "\n\nAttempting Character Load...";
            timeElapsed = Time.time;
            string AddressableLocation = charObject.AbridgedCharInfo.AddressableLocation;
            async = Addressables.LoadAssetAsync<GameObject>(AddressableLocation);
            while (!async.IsDone)
            {
                yield return null;
            }
            timeElapsed = Time.time - timeElapsed;
            operationsLog += "<b>\n.\n.\n.</b>\nCHARACTER LOAD COMPLETED IN " + timeElapsed.ToString("F3") + " SECONDS";
            BattleManagerScript.Instance.AddCharacterPrefab(teamInfoC.CharacterName, async.Result);
            if(displayWarning) 
                Debug.LogError(operationsLog);
        }
        Result = BattleManagerScript.Instance.CreateChar(teamInfoC, parent, teamSide, addToList, showHP, showEther, isBornOfWave);
        IsWorking = false;
    }
}
