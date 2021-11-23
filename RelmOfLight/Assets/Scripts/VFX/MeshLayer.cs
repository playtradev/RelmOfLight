using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshLayer : MonoBehaviour
{
    public int Layer = 305;
    public bool AllChildren = false;
    public string SortingLayerMesh = "Background";
    private void OnEnable()
    {
        AssignLayer();
    }

    public void AssignLayer()
    {
        if (AllChildren)
        {
            foreach (MeshRenderer m in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                m.GetComponent<MeshRenderer>().sortingOrder = Layer;
                m.GetComponent<MeshRenderer>().sortingLayerName = SortingLayerMesh;
            }
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().sortingOrder = Layer;
            gameObject.GetComponent<MeshRenderer>().sortingLayerName = SortingLayerMesh;
        }
    }
}
