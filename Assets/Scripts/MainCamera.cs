using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public Camera _camera;

    private Color currentColor;
    public Color playColor;
    public Color pauseColor;
    public Color levelCompletedColor;

    public float colorTransitionTime;
    public float levelCompletedColorTransitionTime;
    public bool levelCompletedColorSwap;

    void Start()
    {
        levelCompletedColorSwap = false;
        _camera = GetComponent<Camera>();
        _camera.backgroundColor = pauseColor;
    }

    private void Update()
    {
        if (GameManager.levelIsCompleted && !levelCompletedColorSwap)
        {
            backgroundColorSwap();
            levelCompletedColorSwap = true;
        }
    }

    public void backgroundColorSwap()
    {
        if (!GameManager.levelIsCompleted && GameManager.simulationIsRunning)
        {
            StartCoroutine(changeBackGroundColor(pauseColor, playColor, colorTransitionTime));
        }
        else if (!GameManager.levelIsCompleted && !GameManager.simulationIsRunning)
        {
            StartCoroutine(changeBackGroundColor(playColor, pauseColor, colorTransitionTime));
        }
        else if (GameManager.levelIsCompleted)
        {
            StartCoroutine(changeBackGroundColor(currentColor, levelCompletedColor, levelCompletedColorTransitionTime));
        }
    }


    IEnumerator changeBackGroundColor(Color startColor, Color endColor, float seconds)
    {
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            _camera.backgroundColor = Color.Lerp(startColor, endColor, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _camera.backgroundColor = endColor;
        currentColor = endColor;
    }
}
