using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public CanvasGroup lvl1TutoCanvasGroup;
    public CanvasGroup lvl2Tuto1CanvasGroup;
    public CanvasGroup lvl2Tuto2CanvasGroup;
    public CanvasGroup lvl3TutoCanvasGroup;

    private LevelLoader levelLoader;

    [Header("Fades Parameters")]
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float timeToWaitBetweenFades;

    private bool tuto1_isComplete;
    private bool tuto2_isComplete;
    private bool allTuto_areComplete;

    private void Awake()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        tuto1_isComplete = tuto2_isComplete = allTuto_areComplete = false;
    }

    void Update()
    {
        if (levelLoader.currentSceneBuildIndex == 1 && !tuto1_isComplete)
        {
            if (GameManager.currentSceneTime > 1.5f && lvl1TutoCanvasGroup.alpha == 0f) //when the player starts lvl 1
            {
                StartCoroutine(CanvasGroupFade(lvl1TutoCanvasGroup, fadeInDuration, 0f, 1f, 0f));
            }
            if (lvl1TutoCanvasGroup.alpha == 1f && Input.GetMouseButtonDown(2))
            {
                StartCoroutine(CanvasGroupFade(lvl1TutoCanvasGroup, fadeOutDuration, 1f, 0f, 2f));
                tuto1_isComplete = true;
            }
            else if (InGameUIManager.nextLevelIsLoading && !tuto1_isComplete)
            {
                StartCoroutine(CanvasGroupFade(lvl1TutoCanvasGroup, fadeOutDuration, 1f, 0f, 0f));
                tuto1_isComplete = true;
            }
        }
        else if (levelLoader.currentSceneBuildIndex == 2 && !allTuto_areComplete)
        {
            if (GameManager.currentSceneTime > 1.5f && CurrentLevelManager.greenArrowStockIsFull && !tuto1_isComplete) //when the player starts lvl 2
            {
                StartCoroutine(CanvasGroupFade(lvl2Tuto1CanvasGroup, fadeInDuration, 0f, 1f, 0f));
                tuto1_isComplete = true;
            }
            else if (lvl2Tuto1CanvasGroup.alpha == 1f && CurrentLevelManager.isGreenArrowStockEmpty && tuto1_isComplete && !tuto2_isComplete) //when the player has put a green arrow on the board
            {
                StartCoroutine(CanvasGroupFade(lvl2Tuto1CanvasGroup, fadeOutDuration, 1f, 0f, 1f));
                StartCoroutine(CanvasGroupFade(lvl2Tuto2CanvasGroup, fadeOutDuration, 0f, 1f, timeToWaitBetweenFades));
                tuto2_isComplete = true;
            }
            else if (InGameUIManager.nextLevelIsLoading && !tuto1_isComplete && tuto2_isComplete && lvl2Tuto2CanvasGroup.alpha == 1f)
            {
                StartCoroutine(CanvasGroupFade(lvl2Tuto2CanvasGroup, fadeOutDuration, 1f, 0f, 0f));
                allTuto_areComplete = true;
            }
        }
        else if (levelLoader.currentSceneBuildIndex == 3 && tuto2_isComplete)
        {
            if (GameManager.currentSceneTime > 1.5f && CurrentLevelManager.isGreenArrowStockEmpty && tuto1_isComplete) //when the player starts lvl 2
            {
                StartCoroutine(CanvasGroupFade(lvl3TutoCanvasGroup, fadeInDuration, 0f, 1f, 1f));
            }
            else if (InGameUIManager.nextLevelIsLoading && tuto1_isComplete && !tuto2_isComplete)
            {
                StartCoroutine(CanvasGroupFade(lvl3TutoCanvasGroup, fadeOutDuration, 1f, 0f, 0f));
                tuto2_isComplete = true;
            }
        }
    }

    IEnumerator CanvasGroupFade(CanvasGroup canvasGroup, float fadeDuration, float startFadeValue, float endFadeValue, float timeToWait)
    {
        float currentTime = 0f;
        float normalizedValue;
        yield return new WaitForSecondsRealtime(timeToWait);
        while (currentTime <= fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startFadeValue, endFadeValue, normalizedValue);

            yield return null;
        }
    }
}
