using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{

    [Header("Menu Screens")]
    public GameObject mainMenu;
    public GameObject levelHub;
    public GameObject controlsScheme;

    public Button levelHubButton;

    [Header("Button Text Colors")]
    public Color defaultColor;
    public Color highlightColor;
    public Color notInteractableColor;

    [Header("Menu Fade In")]
    public float fadeDuration;
    public float timeToWaitBeforeFade;
    private bool fadeIsCompleted;

    [Header("Scene Fade")]
    public GameObject imageToFade;
    public float fadeOutDuration;

    private void Start()
    {
        AudioManager.instance.Play("menu game opening");
        CanvasRenderer _cR = imageToFade.GetComponent<CanvasRenderer>();
        _cR.SetAlpha(0f);
        levelHubButton = levelHub.GetComponent<Button>();
    }

    private void Update()
    {
        if (!fadeIsCompleted)
        {
            StartCoroutine(MenuButtonFadeIn(this.gameObject, timeToWaitBeforeFade, fadeDuration));
            fadeIsCompleted = true;
        }
        if (LevelLoader.loadingLevelFromMainMenu)
        {
            StartCoroutine(SceneFadeOut(imageToFade, fadeOutDuration));
        }
    }

    IEnumerator MenuButtonFadeIn(GameObject target, float timeToWaitBeforeFade, float fadeDuration)
    {
        yield return new WaitForSecondsRealtime(timeToWaitBeforeFade);

        float currentTime = 0f;
        float normalizedValue;

        while (currentTime <= fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / fadeDuration;

            target.GetComponent<CanvasGroup>().alpha = EasingFunction.EaseOutQuad(0f, 1f, normalizedValue);

            yield return null;
        }
    }

    IEnumerator SceneFadeOut(GameObject target, float fadeDuration)
    {
        float currentTime = 0f;
        float normalizedValue;
        CanvasRenderer _cR;
        while (currentTime <= fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / fadeDuration;
            _cR = target.GetComponent<CanvasRenderer>();
            _cR.SetAlpha(EasingFunction.EaseOutQuad(0f, 1f, normalizedValue));

            yield return null;
        }
    }

    public void DisplayLevelHub()
    {
        mainMenu.SetActive(false);
        levelHub.SetActive(true);
    }

    public void DisplayControlsScheme()
    {
        mainMenu.SetActive(false);
        controlsScheme.SetActive(true);
    }

    public void BackButton()
    {
        if (controlsScheme.activeSelf)
        {
            controlsScheme.SetActive(false);
            mainMenu.SetActive(true);
        }
        else if (levelHub.activeSelf)
        {
            levelHub.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void PlaySound(string soundName)
    {
        AudioManager.instance.Play(soundName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
