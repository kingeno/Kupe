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

    private Vector3 startPos;
    private Quaternion startRotation;

    public static bool isFreeLookActive;

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

        if (!_CMFreeLookCam)
            _CMFreeLookCam = GameObject.FindGameObjectWithTag("CMFreeLookCamera");

    }

    private void Update()
    {
        if (!isFreeLookActive)
        {
            if ((Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(2)))
            {
                FreeLook();
            }
        }
        else if (isFreeLookActive)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                FreeLook();
            }
        }
    }

    public void FreeLook()
    {
        if (_CMFreeLookCam && !GameManager.gameIsPaused)
        {
            if (!isFreeLookActive)
            {
                AudioManager.instance.Play("ig free look on");
                _inGameUIManager.UnselectAllTiles();
                GameManager.playerCanModifyBoard = false;
                isFreeLookActive = true;
            }
            else if (isFreeLookActive)
            {
                AudioManager.instance.Play("ig free look off");
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
}
