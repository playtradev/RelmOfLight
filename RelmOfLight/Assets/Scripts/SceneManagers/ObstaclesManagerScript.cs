using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclesManagerScript : MonoBehaviour
{
    public delegate void ItemPickedUp();
    public event ItemPickedUp ItemPickedUpEvent;



    public static ObstaclesManagerScript Instance;
    private BaseCharacter obstacle;

    private List<BaseCharacter> obstacles = new List<BaseCharacter>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreateObstacle(BattleTileScript bts)
    {
        obstacle = obstacles.Where(r => !r.gameObject.activeInHierarchy && !r.IsOnField).FirstOrDefault();
        if (obstacle == null)
        {
            //obstacle = BattleManagerScript.Instance.CreateChar()
            //obstacles.Add(obstacle);
        }
    }

   
}