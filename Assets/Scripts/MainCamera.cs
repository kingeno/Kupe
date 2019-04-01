using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCamera : MonoBehaviour
{
    
    private GameManager gameManager;
    private InGameUIManager _inGameUIManager;
    private Camera _camera;
    [SerializeField] private GameObject _CMFreeLookCam;
    private Renderer CameraTargetRenderer;
    private Color currentColor;

    [Header("Background Colors")]
    public Color playColor;
    public Color pauseColor;
    public Color stopColor;
    public Color levelCompletedColor;

    private Vector3 startPos;
    private Quaternion startRotation;

    private bool canColorSwap;
    public static bool isFreeLookActive;

    [Header("Transition Time")]
    public float colorTransitionTime;
    public float levelCompletedColorTransitionTime;
    [HideInInspector] public bool levelCompletedColorSwap;

    void Start()
    {
        isFreeLookActive = false;
        startPos = transform.position;
        startRotation = transform.rotation;

        if (!_CMFreeLookCam)
            _CMFreeLookCam = GameObject.FindGameObjectWithTag("CMFreeLookCamera");

        if (!gameManager)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (!_inGameUIManager)
            _inGameUIManager = GameObject.FindGameObjectWithTag("InGameUIManager").GetComponent<InGameUIManager>();

        if (CameraTargetRenderer)
            CameraTargetRenderer.material.color = new Color(1f, 1f, 1f, 0f);

        if (!_CMFreeLookCam)
            _CMFreeLookCam = GameObject.FindGameObjectWithTag("CMFreeLookCamera");

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
        if (!isFreeLookActive)
        {
            if ((Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(2)))
                FreeLook();
        }
        else if (isFreeLookActive)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                FreeLook();
        }
    }

    public void FreeLook()
    {
        if (_CMFreeLookCam && !GameManager.gameIsPaused)
        {
            if (!isFreeLookActive)
            {
                _inGameUIManager.UnselectAllTiles();
                GameManager.playerCanModifyBoard = false;
                isFreeLookActive = true;
            }
            else if (isFreeLookActive)
            {
                if (GameManager.simulationHasBeenLaunched || GameManager.simulationIsRunning)
                    GameManager.playerCanModifyBoard = false;
                else
                    GameManager.playerCanModifyBoard = true;
                isFreeLookActive = false;
            }

            if (isFreeLookActive)
                _CMFreeLookCam.SetActive(true);
            else if (!isFreeLookActive)
                _CMFreeLookCam.SetActive(false);
        }
    }

    public void SetToStartPos()
    {
        transform.position = startPos;
        transform.rotation = startRotation;

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
