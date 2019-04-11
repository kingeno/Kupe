using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public GameObject image;

    public float fadeInDuration;
    public float fadeOutDuration;

    private bool isFadingOut;

    void Start()
    {
        StartCoroutine(SceneFade(image, fadeInDuration, 1f, 0f));
    }

    private void Update()
    {
        if (LevelLoader.loadingLevelFromMainMenu && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(SceneFade(image, fadeOutDuration, 0f, 1f));
        }
    }

    IEnumerator SceneFade(GameObject target, float fadeDuration, float startFadeValue, float endFadeValue)
    {
        float currentTime = 0f;
        float normalizedValue;
        CanvasRenderer _cR;
        while (currentTime <= fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / fadeDuration;
            _cR = target.GetComponent<CanvasRenderer>();
            _cR.SetAlpha(Mathf.Lerp(startFadeValue, endFadeValue, normalizedValue));

            yield return null;
        }
    }
}
