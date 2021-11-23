using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;

public class FightGrid : MonoBehaviour
{
    [Tooltip("A unique index for the grid in the environment")][Range(0, 99)][SerializeField] public int index = 99;
    [HideInInspector] public Vector3 pivot;

    public bool ShowSprites = true;

    SpriteAtlasInfoClass SAIC;
    string val;

    private void Awake()
    {
        pivot = transform.position;
    }
}
