using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_CameraMovement : MonoBehaviour
{
    public static VFX_CameraMovement Instance;

    public float minSize = 4;
    public float maxSize = 20;
    public KeyCode InputGoToTestPosition = KeyCode.Alpha1;
    public Vector3 TestPosition = new Vector3(-2, -109, -10);
    public KeyCode InputGoToInitialPosition = KeyCode.Alpha2;
    public Vector3 InitialPosition = new Vector3(0, 0, -10);

    private BlockMouse[] BlockMouseInput;
    private bool Block = false;
    private void Awake()
    {
        Instance = this;
        BlockMouseInput = Object.FindObjectsOfType<BlockMouse>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        //if(Mathf.Abs((Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).x) > Screen.width / 5 || Mathf.Abs((Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).y) > Screen.height / 5)
        //{
        foreach (BlockMouse block in BlockMouseInput)
        {
            if (block.MouseIn)
            {
                Block = true;
                break;
            }
            Block = false;
        }
        if (!Block)
        {
            if (Input.GetMouseButton(0))
            {
                transform.position += (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)) / 60000 * GetComponent<Camera>().orthographicSize;

            }
            if (Input.GetKeyDown(InputGoToTestPosition))
            {
                transform.position = TestPosition;

            }
            if (Input.GetKeyDown(InputGoToInitialPosition))
            {
                transform.position = InitialPosition;

            }
        }

        GetComponent<Camera>().orthographicSize -= (Input.mouseScrollDelta.y);
        if (GetComponent<Camera>().orthographicSize < minSize)
        {
            GetComponent<Camera>().orthographicSize = minSize;
        }
        else
        if (GetComponent<Camera>().orthographicSize > maxSize)
        {
            GetComponent<Camera>().orthographicSize = maxSize;
        }
        //}


    }
}
