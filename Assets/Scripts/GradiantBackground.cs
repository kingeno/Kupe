﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradiantBackground : MonoBehaviour
{
    public RawImage backgroundImage;
    private Texture2D backgroundTexture;

    public float fadeDuration;

    [Header("Default Colors")]
    public Color defaultTopColor;
    public Color defaultBottomColor;

    [Header("Level Complete Colors")]
    public Color levelCompletedTopColor;
    public Color levelCompletedBottomColor;

    private LevelLoader levelLoader;

    private bool levelCompletedFade;
    private bool nextLevelFadeIsCompleted;

    void Awake()
    {
        backgroundTexture = new Texture2D(1, 2);
        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
        backgroundTexture.filterMode = FilterMode.Trilinear;
        levelCompletedFade = false;

    }

    private void Start()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();

        SetColor(defaultBottomColor, defaultTopColor);

        nextLevelFadeIsCompleted = false;
    }

    private void Update()
    {
        if (!levelCompletedFade && GameManager.levelIsCompleted)
        {
            StartCoroutine(LevelCompletedBackgroundColorFade(fadeDuration, defaultTopColor, defaultBottomColor, levelCompletedTopColor, levelCompletedBottomColor));
            levelCompletedFade = true;
        }
        if(!nextLevelFadeIsCompleted && InGameUIManager.nextLevelIsLoading)
        {
            StartCoroutine(LevelCompletedBackgroundColorFade(fadeDuration, levelCompletedTopColor, levelCompletedBottomColor, defaultTopColor, defaultBottomColor));
            nextLevelFadeIsCompleted = true;
        }
    }

    public void SetColor(Color color1, Color color2)
    {
        backgroundTexture.SetPixels(new Color[] { color1, color2 });
        backgroundTexture.Apply();
        backgroundImage.texture = backgroundTexture;
    }

    IEnumerator LevelCompletedBackgroundColorFade(float fadeTime, Color currentColor1, Color currentColor2, Color lerpedColor1, Color lerpedColor2)
    {
        float currentTime = 0f;
        float normalizedValue;

        Color topColor;
        Color bottomColor;

        while (currentTime <= fadeTime)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / fadeTime;

            topColor = Color.Lerp(currentColor1, lerpedColor1, normalizedValue);
            bottomColor = Color.Lerp(currentColor2, lerpedColor2, normalizedValue);
            SetColor(bottomColor, topColor);
            yield return null;
        }
    }
}
