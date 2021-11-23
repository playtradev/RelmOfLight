using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemsPowerUPsInfoScript : MonoBehaviour
{

    public delegate void ItemPickedUp();
    public event ItemPickedUp ItemPickedUpEvent;

    public ScriptableObjectItemPowerUps ItemPowerUpInfo;
    //public SpriteRenderer Icon;
    public PowerUpColorTypes color = PowerUpColorTypes.White;
    public TextMeshPro puText = null;
    public Animator Anim;
    public Vector2Int Pos;
    protected Vector3 position;
    protected GameObject activeParticles = null;
    private IEnumerator OnField_Co;
    private BaseCharacter CharHitted;


    public void SetItemPowerUp(ScriptableObjectItemPowerUps itemPowerUpInfo, Vector3 worldPos, Vector2Int gridPos)
    {
        position = worldPos;
        Pos = gridPos;
        ItemPowerUpInfo = itemPowerUpInfo;
        //Icon.sprite = itemPowerUpInfo.Icon;
        puText.text = itemPowerUpInfo.powerUpText;
        color = itemPowerUpInfo.color;
        transform.position = worldPos;
        if(ItemPowerUpInfo.activeParticles != null)
        {
            activeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(ItemPowerUpInfo.activeParticles, transform.position);
            activeParticles.transform.position -= new Vector3(0f, 0.3f, 0f);
        }
        Anim.SetInteger("Color", (int)color);
        Anim.SetBool("FadeInOut", true);
        StartCoroutine(spawn_Co());
    }

    private IEnumerator spawn_Co()
    {
        OnField_Co = DurationOnBattleField_Co();
        if (ItemPowerUpInfo.InfiniteOnFieldDuration) yield break;

        yield return OnField_Co;
        yield return StopItem_Co();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Side"))
        {
            ItemPickedUpEvent?.Invoke();
            CharHitted = other.GetComponentInParent<BaseCharacter>();

            ItemSpawnerManagerScript.Instance.CollectPowerUp(ItemPowerUpInfo, CharHitted, transform.position);

            StopCoroutine(OnField_Co);
            StartCoroutine(StopItem_Co());
        }
    }


    private IEnumerator StopItem_Co()
    {
        activeParticles?.SetActive(false);
        Anim.SetBool("FadeInOut", false);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }


    private IEnumerator DurationOnBattleField_Co()
    {
        yield return BattleManagerScript.Instance.WaitFor(ItemPowerUpInfo.DurationOnField, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
    }
}
