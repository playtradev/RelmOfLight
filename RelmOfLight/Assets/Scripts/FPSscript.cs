using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
#if UNITY_SWITCH
using UnityEngine.Switch;
#endif
public class FPSscript : MonoBehaviour
{
    public TextMeshProUGUI FPStxt;
    public TextMeshProUGUI Modetxt;
    public TextMeshProUGUI Resolutiontxt;
    public TextMeshProUGUI Performancestxt;

    public TextMeshProUGUI Timertxt;


    public int count;
    public int samples = 100;
    public float totalTime;

    public void Start()
    {
        count = samples;
        totalTime = 0f;
    }

    float deltaTime = 0.0f;

    List<string> teststring = new List<string>()
    {
        "1",
        "2",
        "3"
    };

    // Update is called once per frame
    void Update()
    {
        count -= 1;
        totalTime += Time.deltaTime;

        if (count <= 0)
        {
            float fps = samples / totalTime;
            FPStxt.text = fps.ToString();
            totalTime = 0f;
            count = samples;
        }

        Timertxt.text = Time.time.ToString();


        //Debug.Log(teststring.Where(a => a == "3").FirstOrDefault() + "   " + Time.time);

           //Debug.Log(teststring.GridFight_Where_FirstOrDefault((a) => a == "3") + "   " + Time.time);




        //FPStxt.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
        //deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        //  Modetxt.text = Operation.mode.ToString();
        //Resolutiontxt.text = "Width: " + Display.main.renderingWidth + "  Height: " + Display.main.renderingHeight;
        //  Performancestxt.text = Performance.mode.ToString();



        /* if (Input.GetKeyUp(KeyCode.R))
         {
             //long numBytes = System.GC.GetTotalMemory(false);

             System.Diagnostics.Process currentProcess1 = System.Diagnostics.Process.GetCurrentProcess();

             currentProcess1.Refresh();

             long totalMemoryUsedByProcess1 = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(GridManagerScript.Instance.transform.parent.gameObject); //currentProcess1.WorkingSet64;
             totalMemoryUsedByProcess1 = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();

             UnityEngine.Debug.Log(totalMemoryUsedByProcess1);
             FPStxt.text = totalMemoryUsedByProcess1.ToString();
             //Debug.Log("- Allocated memory by Unity: " + Profiler.GetTotalAllocatedMemoryLong() + "Bytes");
         }*/
    }



    /* void OnGUI()
     {
         int w = Screen.width, h = Screen.height;

         GUIStyle style = new GUIStyle();

         Rect rect = new Rect(0, 0, w, h * 2 / 100);
         style.alignment = TextAnchor.UpperLeft;
         style.fontSize = h * 2 / 100;
         style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
         float msec = deltaTime * 1000.0f;
         float fps = 1.0f / deltaTime;
         string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
         GUI.Label(rect, text, style);
     }*/
}
