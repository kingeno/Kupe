using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    public static int previousScene;
    public static bool loadedFromLevelHub;
    public static bool loadingFromLevelHub;

    [HideInInspector] public int currentSceneBuildIndex;
    [HideInInspector] public int levelIndexToLaod;
    public static bool loadingLevelFromMainMenu;
    public static bool loadingLevelFromLevel;

    public float loadingFirstLevelDelay;
    public float loadingNextLevelDelay;
    public float loadingSpecificLevelDelay;

    private void Start()
    {
        DontDestroyOnLoad(this);
        currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        loadingLevelFromLevel = false;
        loadingFromLevelHub = false;
    }

    void Update()
    {
        if (currentSceneBuildIndex != SceneManager.GetActiveScene().buildIndex)
            currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadFirstLevel()
    {
        previousScene = currentSceneBuildIndex;
        if (currentSceneBuildIndex == 0)
            AudioManager.instance.LoadLevelFromMainMenuCrossFade();
        StartCoroutine(LoadNextAsyncScene(1, loadingFirstLevelDelay));
        GameManager.currentSceneTime = 0f;
    }

    public void LoadNextLevel()
    {
        previousScene = currentSceneBuildIndex; previousScene = currentSceneBuildIndex;
        if (currentSceneBuildIndex == 0)
            AudioManager.instance.LoadLevelFromMainMenuCrossFade();
        StartCoroutine(LoadNextAsyncScene(currentSceneBuildIndex + 1, loadingNextLevelDelay));
        GameManager.currentSceneTime = 0f;
        
    }

    public void LoadSpecificLevel(int levelIndex)
    {
        previousScene = currentSceneBuildIndex;
        if (levelIndex == 0)
        {
            AudioManager.instance.ExitToMainMenuCrossFade();
            AudioManager.instance.ResumeMusicCutoff();
        }
        else if (currentSceneBuildIndex == 0)
        {
            AudioManager.instance.LoadLevelFromMainMenuCrossFade();
        }
        else if (currentSceneBuildIndex != 0)
        {
            loadingFromLevelHub = true;
            if (levelIndex != 0)
                AudioManager.instance.ResumeMusicCutoff();
        }

        StartCoroutine(LoadSpecificAsyncScene(levelIndex, loadingSpecificLevelDelay));
        GameManager.currentSceneTime = 0f;
    }


    IEnumerator LoadNextAsyncScene(int sceneBuildIndexToLoad, float loadingDelay)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad;

        if (currentSceneBuildIndex == 0)
        {
            loadingLevelFromMainMenu = true;
            yield return new WaitForSecondsRealtime(loadingDelay);

            if (sceneBuildIndexToLoad < 10)
                asyncLoad = SceneManager.LoadSceneAsync("Level00" + sceneBuildIndexToLoad.ToString());
            else
                asyncLoad = SceneManager.LoadSceneAsync("Level0" + sceneBuildIndexToLoad.ToString());

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                loadingLevelFromMainMenu = false;
                yield return null;
            }
        }
        else
        {
            loadingLevelFromLevel = true;
            yield return new WaitForSecondsRealtime(loadingDelay);

            if (sceneBuildIndexToLoad < 10)
                asyncLoad = SceneManager.LoadSceneAsync("Level00" + sceneBuildIndexToLoad.ToString());
            else
                asyncLoad = SceneManager.LoadSceneAsync("Level0" + sceneBuildIndexToLoad.ToString());

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                loadingLevelFromLevel = false;
                yield return null;
            }
        }
    }

    IEnumerator LoadSpecificAsyncScene(int sceneBuildIndexToLoad, float loadingDelay)
    {
        loadedFromLevelHub = true;

        AsyncOperation asyncLoad;

        loadingLevelFromMainMenu = true;
        loadingLevelFromLevel = true;
        yield return new WaitForSecondsRealtime(loadingDelay);
        
        if (sceneBuildIndexToLoad < 10)
            asyncLoad = SceneManager.LoadSceneAsync("Level00" + sceneBuildIndexToLoad.ToString());
        else
            asyncLoad = SceneManager.LoadSceneAsync("Level0" + sceneBuildIndexToLoad.ToString());

        while (!asyncLoad.isDone)
        {
            loadingLevelFromMainMenu = false;
            loadingLevelFromLevel = false;
            yield return null;
        }
    }
}
