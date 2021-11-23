using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManagerScript : MonoBehaviour
{
    public static MainMenuManagerScript Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void GoToScene(string nextScene)
    {
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        LoaderManagerScript.Instance.LoadNewSceneWithLoading(nextScene, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public IEnumerator LoadLoadingScene(string nextScene)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Loading", UnityEngine.SceneManagement.LoadSceneMode.Additive);



        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Wait until the asynchronous scene fully loads
        while (LoaderManagerScript.Instance == null)
        {
            yield return null;
        }

        


    }
}
