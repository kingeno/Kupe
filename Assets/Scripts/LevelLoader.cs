using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    private int currentSceneBuildIndex;
    public int levelIndexToLaod;

    private void Start()
    {
        DontDestroyOnLoad(this);
        currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (currentSceneBuildIndex != SceneManager.GetActiveScene().buildIndex)
            currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        // Uncomment to activate level skipping shortcut
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //    StartCoroutine(LoadNextAsyncScene(currentSceneBuildIndex + 1));
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    StartCoroutine(LoadPreviousAsyncScene(currentSceneBuildIndex - 1));
    }

    public void loadFirstLevel()
    {
        StartCoroutine(LoadNextAsyncScene(1));
    }

    public void loadNextLevel()
    {
        StartCoroutine(LoadNextAsyncScene(currentSceneBuildIndex + 1));
    }

    public void loadPreviousLevel()
    {
        StartCoroutine(LoadPreviousAsyncScene(currentSceneBuildIndex - 1));
    }

    public void loadSpecificLevel(int levelIndex)
    {
        StartCoroutine(LoadPreviousAsyncScene(levelIndex));
    }


    IEnumerator LoadNextAsyncScene(int sceneBuildIndexToLoad)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad;
        if (sceneBuildIndexToLoad < 10)
            asyncLoad = SceneManager.LoadSceneAsync("Level00" + sceneBuildIndexToLoad.ToString());
        else
            asyncLoad = SceneManager.LoadSceneAsync("Level0" + sceneBuildIndexToLoad.ToString());

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadPreviousAsyncScene(int sceneBuildIndexToLoad)
    {
        AsyncOperation asyncLoad;
        if (sceneBuildIndexToLoad < 10)
            asyncLoad = SceneManager.LoadSceneAsync("Level00" + sceneBuildIndexToLoad.ToString());
        else
            asyncLoad = SceneManager.LoadSceneAsync("Level0" + sceneBuildIndexToLoad.ToString());

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadSpecificAsyncScene(int sceneBuildIndexToLoad)
    {
        AsyncOperation asyncLoad;
        if (sceneBuildIndexToLoad < 10)
            asyncLoad = SceneManager.LoadSceneAsync("Level00" + sceneBuildIndexToLoad.ToString());
        else
            asyncLoad = SceneManager.LoadSceneAsync("Level0" + sceneBuildIndexToLoad.ToString());

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
