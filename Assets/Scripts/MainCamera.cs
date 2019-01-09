using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public Camera _camera;
    public GameObject CMFreeLookCam;
    private Color currentColor;
    public Color playColor;
    public Color pauseColor;
    public Color stopColor;
    public Color levelCompletedColor;

    private bool canColorSwap;

    public float colorTransitionTime;
    public float levelCompletedColorTransitionTime;
    public bool levelCompletedColorSwap;

    void Start()
    {
        canColorSwap = true;
        levelCompletedColorSwap = false;
        _camera = GetComponent<Camera>();
        _camera.backgroundColor = stopColor;
        currentColor = stopColor;
    }

    private void Update()
    {
        if (GameManager.levelIsCompleted && !levelCompletedColorSwap)
        {
            backgroundColorSwap();
            levelCompletedColorSwap = true;
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (CMFreeLookCam.activeSelf)
                CMFreeLookCam.SetActive(false);
            else
                CMFreeLookCam.SetActive(true);
        }
    }

    public void backgroundColorSwap()
    {
        if (!GameManager.levelIsCompleted && canColorSwap)
        {
            if (GameManager.simulationIsRunning && !GameManager.playerCanModifyBoard)
                StartCoroutine(changeBackGroundColor(currentColor, playColor, colorTransitionTime));

            if (!GameManager.simulationIsRunning && !GameManager.playerCanModifyBoard)
                StartCoroutine(changeBackGroundColor(currentColor, pauseColor, colorTransitionTime));

            if (!GameManager.simulationIsRunning && GameManager.playerCanModifyBoard)
                StartCoroutine(changeBackGroundColor(currentColor, stopColor, colorTransitionTime));
        }
        else if (GameManager.levelIsCompleted && canColorSwap)
        {
            StartCoroutine(changeBackGroundColor(currentColor, levelCompletedColor, levelCompletedColorTransitionTime));
            canColorSwap = false;
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
