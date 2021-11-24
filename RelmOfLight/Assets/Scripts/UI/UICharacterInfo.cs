using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharacterInfo : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public ControllerType Controller;
	[HideInInspector]public BaseCharacter CB;
	public RectTransform ParentIcon;
	public CharacterNameType CharacterId;
	public Image CharIcon;
	public Button Skill;
	public Image SkillIcon;
	public Vector3 OffSetPosition;
	public TeamSideType Side;

	public void Strongattack()
	{
		if (CB != null && CB.IsOnField && BattleManagerScript.Instance.ManaCostSkill <= (Side == TeamSideType.LeftSideTeam ? BattleManagerScript.Instance.LeftMana.CurrentMana : BattleManagerScript.Instance.RightMana.CurrentMana))
		{
			if (Side == TeamSideType.LeftSideTeam)
			{
				BattleManagerScript.Instance.LeftMana.CurrentMana -= BattleManagerScript.Instance.ManaCostSkill;
			}
			else
			{
				BattleManagerScript.Instance.RightMana.CurrentMana -= BattleManagerScript.Instance.ManaCostSkill;
			}
			CB.currentInputProfile.UseStrong = true;
			CB.currentInputProfile.UseDir = false;
			StartCoroutine(skillDisable());
		}
	}

	private IEnumerator skillDisable()
	{
		Color color = CharIcon.color;
		SkillIcon.color = Color.black;
		Skill.interactable = false;
		while (CB.currentInputProfile.UseStrong)
		{
			yield return null;
		}
		SkillIcon.color = Color.white;
		Skill.interactable = true;
	}


	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!CB.died && !CB.IsOnField && CB.CharInfo.ManaCostN <= (Side == TeamSideType.LeftSideTeam ? BattleManagerScript.Instance.LeftMana.CurrentMana : BattleManagerScript.Instance.RightMana.CurrentMana))
		{
			transform.position = eventData.position;
			//MoveCharOnBoard(eventData.position);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!CB.died && !CB.IsOnField && CB.CharInfo.ManaCostN <= (Side == TeamSideType.LeftSideTeam ? BattleManagerScript.Instance.LeftMana.CurrentMana : BattleManagerScript.Instance.RightMana.CurrentMana))
		{
			transform.position = OffSetPosition;
			SetCharOnBoard(eventData.position);
			
		}
	}

	

	// Use this for initialization
	void Start()
	{
        BattleManagerScript.Instance.CharacterCreatedEvent += Instance_CharacterCreatedEvent;
		OffSetPosition = transform.position;
	}

    private void Instance_CharacterCreatedEvent(BaseCharacter cb)
    {
		if (CharacterId != CharacterNameType.None)
		{
			CB = BattleManagerScript.Instance.AllCharacters.Where(r => r.CharInfo.CharacterID == CharacterId).FirstOrDefault();
			CharIcon.sprite = CB.CharInfo.CharacterIcon;
			SkillIcon.sprite = CB.CharInfo._CurrentAttackTypeInfo.GridFight_Where_FirstOrDefault(r=> r.AttackInput == AttackInputType.Strong).AttackIcon;
		}
	}

    void Update()
	{

		if (CharIcon != null && CB != null &&
			(CB.CharInfo.ManaCostN > (Side == TeamSideType.LeftSideTeam ? BattleManagerScript.Instance.LeftMana.CurrentMana : BattleManagerScript.Instance.RightMana.CurrentMana)))
		{
			Color color = CharIcon.color;
			if (CB.died)
			{
				color = Color.black;
			}
			color.a = 0.6f;
			CharIcon.color = color;
		}
		else if (CharIcon != null && CB != null && CB.CharInfo.ManaCostN <= (Side == TeamSideType.LeftSideTeam ? BattleManagerScript.Instance.LeftMana.CurrentMana : BattleManagerScript.Instance.RightMana.CurrentMana)
			&& !CB.died)
		{
			Color color = CharIcon.color;
			color = Color.white;
			color.a = 1;
			CharIcon.color = color;
		}

		if (CharIcon != null && CB != null)
		{

			ParentIcon.localScale = CB.IsOnField ? new Vector3(.5f, .5f, 1) : Vector3.one;
		}
	}

	
	private void MoveCharOnBoard(Vector3 pointer)
	{
		Ray ray = Camera.main.ScreenPointToRay(pointer);
		Plane p = new Plane(Vector3.up, Vector3.zero);
		float dist = 0;
		p.Raycast(ray, out dist);
		Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 30);
		List<RaycastHit> hits = Physics.RaycastAll(ray, 100).ToList();
		if (hits.Where(r => r.collider.tag == "Tile").ToList().Count > 0)
		{
			BattleTileScript boardS = hits.Where(r => r.collider.tag == "Tile").First().collider.GetComponentInParent<BattleTileScript>();
			if (!boardS.isTaken && boardS.cb == null)
			{
				CB.gameObject.SetActive(true);
				/*if (Side == Side)
				{
					CB.transform.rotation = Quaternion.Euler(0, 180, 0);
				}*/
				CB.transform.position = boardS.transform.position;
				GetComponent<Image>().enabled = false;
			}
		}
		else
		{
			CB.transform.position = new Vector3(100, 100, 100);
			GetComponent<Image>().enabled = true;
			GetComponent<Image>().color = Color.white;
		}
	}


	private void SetCharOnBoard(Vector3 pointer)
	{
		Ray ray = Camera.main.ScreenPointToRay(pointer);
		Plane p = new Plane(Vector3.up, Vector3.zero);
		float dist = 0;
		p.Raycast(ray, out dist);
		Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 30);
		List<RaycastHit> hits = Physics.RaycastAll(ray, 100).ToList(); //Physics.RaycastAll(ray, out hit, 100);
		if (hits.Where(r => r.collider.tag == "Tile").ToList().Count > 0)
		{
			BattleTileScript boardS = hits.Where(r => r.collider.tag == "Tile").First().collider.GetComponentInParent<BattleTileScript>();
			if (!boardS.isTaken && boardS.cb == null)
			{
				if ((Side == TeamSideType.LeftSideTeam && boardS.Pos.y == 1) || (Side == TeamSideType.RightSideTeam && boardS.Pos.y == 10))
				{
					if (Side == TeamSideType.LeftSideTeam)
					{
						BattleManagerScript.Instance.LeftMana.CurrentMana -= CB.CharInfo.ManaCostN;
					}
					else
					{
						BattleManagerScript.Instance.RightMana.CurrentMana -= CB.CharInfo.ManaCostN;
					}
					BattleManagerScript.Instance.SetCharOnBoard(Controller, CB, boardS.Pos);

				}
			}
		}
	}
}


public enum TouchPhaseType
{
	none,
	Began,
	Drag,
	End
}