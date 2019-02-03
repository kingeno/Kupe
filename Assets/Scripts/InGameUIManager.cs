﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    private GameManager gameManager;
    private LevelLoader levelLoader;
    private MainCamera _mainCamera;
    private GameObject cinemachineCamera;

    [Header("Tile Prefabs")]
    public Transform blankTilePrefab;
    public Transform greenArrowPrefab;

    [Header("General UI")]
    public GameObject inGameUI;
    public GameObject pauseMenu;
    public GameObject controlsScheme;
    public GameObject winScreen;

    [Header("Contextual Window")]
    public GameObject contextualWindow;
    public TextMeshProUGUI contextualWindowText;

    [Header("Button Outlines")]
    public GameObject deleteTileSelectedOutline;
    public GameObject greenArrowSelectedOutline;

    [Header("Delete Tile Button")]
    public Button deleteButtonScript;
    public Image deleteTileButtonImage;

    [Header("Arrow Tile Button")]
    public Button arrowButtonScript;
    public Image greenArrowButtonImage;

    [Header("Slow Down Button")]
    public Button slowDownButtonScript;
    public Image slowDownButtonImage;

    [Header("Speed Up Button")]
    public Button speedUpButtonScript;
    public Image speedUpButtonImage;

    [Header("Play Simulation Button")]
    public GameObject playSimulationButton;
    public Button playButtonScript;
    public Image playButtonImage;

    [Header("Pause Simulation Button")]
    public GameObject pauseSimulationButton;
    public Button pauseButtonScript;
    public Image pauseButtonImage;

    [Header("Stop Simulation Button")]
    public Button stopButtonScript;
    public Image stopSimulationButtonImage;

    [Header("Pause Menu Button")]
    public Button pauseMenuButtonScript;
    public Image pauseMenuButtonImage;

    [Header("Camera Buttons")]
    public Button resetCameraPosButtonScript;
    public Image resetCameraPosButtonImage;

    public Image cameraControlButtonImage;

    [Header("Interactable Colors")]
    public Color notInteractableButtonColor;
    public Color interactableButtonColor;

    public static bool isDeleteTileSelected;
    public static bool isGreenArrowSelected;
    public static bool nothingIsSelected;

    public static bool changeSpeedSimulation;

    [HideInInspector] public bool isOverPlayerArrowTile;


    private void Start()
    {
        nothingIsSelected = true;
        isDeleteTileSelected = false;
        isGreenArrowSelected = false;
        changeSpeedSimulation = false;

        if (!gameManager)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (!levelLoader)
            levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();  //if you encounter a null reference exception here it means that you have launched the game without going through the main menu

        if (!_mainCamera)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();
        if (!cinemachineCamera)
            cinemachineCamera = GameObject.FindGameObjectWithTag("CMFreeLookCamera");
    }

    private void Update()
    {
        if (!GameManager.gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                SpeedUpSimulation();
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                SlowDownSimulation();

            if (Input.GetMouseButtonDown(1))
                UnselectAllTiles();


            if (MainCamera.isFreeLookActive)
            {
                ToggleUIButton(arrowButtonScript, false, greenArrowButtonImage, notInteractableButtonColor);
                ToggleUIButton(deleteButtonScript, false, deleteTileButtonImage, notInteractableButtonColor);
                ToggleUIButton(playButtonScript, false, playButtonImage, notInteractableButtonColor);
                ToggleUIButton(pauseButtonScript, false, pauseButtonImage, notInteractableButtonColor);
                ToggleUIButton(slowDownButtonScript, false, slowDownButtonImage, notInteractableButtonColor);
                ToggleUIButton(speedUpButtonScript, false, speedUpButtonImage, notInteractableButtonColor);
                ToggleUIButton(stopButtonScript, false, stopSimulationButtonImage, notInteractableButtonColor);
                ToggleUIButton(pauseMenuButtonScript, false, pauseMenuButtonImage, notInteractableButtonColor);
                ToggleUIButton(resetCameraPosButtonScript, false, resetCameraPosButtonImage, notInteractableButtonColor);
            }
            else if (!MainCamera.isFreeLookActive)
            {
                ToggleUIButton(resetCameraPosButtonScript, true, resetCameraPosButtonImage, notInteractableButtonColor);
                ToggleUIButton(pauseButtonScript, true, pauseButtonImage, interactableButtonColor);
                ToggleUIButton(pauseMenuButtonScript, true, pauseMenuButtonImage, interactableButtonColor);
                ToggleUIButton(slowDownButtonScript, true, slowDownButtonImage, interactableButtonColor);
                ToggleUIButton(speedUpButtonScript, true, speedUpButtonImage, interactableButtonColor);


                if (gameManager.allEndTilesAreValidated)
                    ToggleUIButton(playButtonScript, false, playButtonImage, notInteractableButtonColor);
                else
                    ToggleUIButton(playButtonScript, true, playButtonImage, interactableButtonColor);

                if (!CurrentLevelManager.isGreenArrowStockEmpty)
                    ToggleUIButton(arrowButtonScript, true, greenArrowButtonImage, interactableButtonColor);
                else if (CurrentLevelManager.isGreenArrowStockEmpty)
                {
                    isGreenArrowSelected = false;
                    greenArrowSelectedOutline.SetActive(false);
                    ToggleUIButton(arrowButtonScript, false, greenArrowButtonImage, notInteractableButtonColor);
                }


                if (!CurrentLevelManager.greenArrowStockIsFull)
                    ToggleUIButton(deleteButtonScript, true, deleteTileButtonImage, interactableButtonColor);
                else if (CurrentLevelManager.greenArrowStockIsFull)
                {
                    isDeleteTileSelected = false;
                    deleteTileSelectedOutline.SetActive(false);
                    ToggleUIButton(deleteButtonScript, false, deleteTileButtonImage, notInteractableButtonColor);
                }


                if (!GameManager.simulationHasBeenLaunched)
                    ToggleUIButton(stopButtonScript, false, stopSimulationButtonImage, notInteractableButtonColor);
                else if (GameManager.simulationHasBeenLaunched)
                {
                    ToggleUIButton(stopButtonScript, true, stopSimulationButtonImage, interactableButtonColor);
                    ToggleUIButton(deleteButtonScript, false, deleteTileButtonImage, notInteractableButtonColor);
                    ToggleUIButton(arrowButtonScript, false, greenArrowButtonImage, notInteractableButtonColor);
                }
            }

            if (!GameManager.simulationIsRunning && pauseSimulationButton.activeSelf)
            {
                UnselectAllTiles();
                playSimulationButton.SetActive(true);
                pauseSimulationButton.SetActive(false);
                if (_mainCamera)
                {
                    _mainCamera.backgroundColorSwap();
                }
            }
            else if (GameManager.simulationIsRunning && GameManager.simulationHasBeenLaunched && playSimulationButton.activeSelf)
            {
                playSimulationButton.SetActive(false);
                pauseSimulationButton.SetActive(true);
                if (_mainCamera)
                {
                    _mainCamera.backgroundColorSwap();
                }
            }

            if (GameManager.levelIsCompleted && !winScreen.activeSelf)
            {
                winScreen.SetActive(true);
            }

            if (isOverPlayerArrowTile)
            {
                contextualWindow.SetActive(true);
                contextualWindowText.text = "rotate : r / left clic";
            }
            else
                contextualWindow.SetActive(false);
        }
        
    }

    public void UnselectAllTiles()
    {
        isGreenArrowSelected = false;
        isDeleteTileSelected = false;
        deleteTileSelectedOutline.SetActive(false);
        greenArrowSelectedOutline.SetActive(false);
        nothingIsSelected = true;
    }

    public void ToggleUIButton(Button button, bool toggleOn, Image buttonImage, Color imageColor)
    {
        button.interactable = toggleOn;
        buttonImage.color = imageColor;
    }

    public void DeleteSelection()
    {
        if (!MainCamera.isFreeLookActive)
        {
            UnselectAllTiles();
            isDeleteTileSelected = true;
            deleteTileSelectedOutline.SetActive(true);
        }
    }

    public void GreenArrowSelection()
    {
        if (!MainCamera.isFreeLookActive)
        {
            UnselectAllTiles();

            if (!CurrentLevelManager.isGreenArrowStockEmpty)
            {
                isGreenArrowSelected = true;
                greenArrowSelectedOutline.SetActive(true);
            }
        }
    }

    public void SimulationButton()
    {
        if (!MainCamera.isFreeLookActive)
        {
            if (!GameManager.simulationIsRunning)
            {
                UnselectAllTiles();
                gameManager.LaunchSimulation();
                greenArrowButtonImage.color = notInteractableButtonColor;
                deleteTileButtonImage.color = notInteractableButtonColor;

                stopButtonScript.interactable = true;
                stopSimulationButtonImage.color = interactableButtonColor;

                if (_mainCamera)
                {
                    _mainCamera.backgroundColorSwap();
                }
            }
            else if (GameManager.simulationIsRunning)
            {
                gameManager.LaunchSimulation();
                if (_mainCamera)
                {
                    _mainCamera.backgroundColorSwap();
                }
            }
        }
    }

    public void SpeedUpSimulation()
    {
        //if (!MainCamera.isFreeLookActive)
        //{
            if (gameManager.simulationSpeed == 1f)
            {
                changeSpeedSimulation = true;
                gameManager.simulationSpeed = 2f;
                gameManager.turnTime = 0.47f;
            }
            else if (gameManager.simulationSpeed == 2f)
            {
                changeSpeedSimulation = true;
                gameManager.simulationSpeed = 3f;
                gameManager.turnTime = 0.44f;
        }
            else if (gameManager.simulationSpeed == 3f)
            {
                changeSpeedSimulation = true;
                gameManager.simulationSpeed = 1f;
            gameManager.turnTime = 0.5f;
        }
        //}
    }

    public void SlowDownSimulation()
    {
        //if (!MainCamera.isFreeLookActive)
        //{
            if (gameManager.simulationSpeed == 1f)
            {
                changeSpeedSimulation = true;
                gameManager.simulationSpeed = 3f;
            gameManager.turnTime = 0.44f;
        }
            else if (gameManager.simulationSpeed == 2f)
            {
                changeSpeedSimulation = true;
                gameManager.simulationSpeed = 1f;
            gameManager.turnTime = 0.5f;
        }
            else if (gameManager.simulationSpeed == 3f)
            {
                changeSpeedSimulation = true;
                gameManager.simulationSpeed = 2f;
            gameManager.turnTime = 0.47f;
        }
        //}
    }

    public void StopSimulation()
    {
        if (!MainCamera.isFreeLookActive)
        {
            UnselectAllTiles();
            gameManager.StopSimulation();
            ToggleUIButton(playButtonScript, true, playButtonImage, interactableButtonColor);

            stopButtonScript.interactable = false;
            stopSimulationButtonImage.color = notInteractableButtonColor;

            greenArrowButtonImage.color = interactableButtonColor;
            deleteTileButtonImage.color = interactableButtonColor;
        }
    }

    public void ToggleFreeLook()
    {
        UnselectAllTiles();
        _mainCamera.FreeLook();
    }

    public void ResetCameraPosition()
    {
        _mainCamera.SetToStartPos();
    }

    public void PauseMenu()
    {
        if (!MainCamera.isFreeLookActive)
        {
            UnselectAllTiles();
            GameManager.gameIsPaused = true;
            GameManager.playerCanModifyBoard = false;
            Time.timeScale = 0f;
            inGameUI.gameObject.SetActive(false);
            winScreen.gameObject.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = gameManager.simulationSpeed;
        pauseMenu.SetActive(false);
        GameManager.gameIsPaused = false;
        inGameUI.gameObject.SetActive(true);
        if (!GameManager.simulationIsRunning && !GameManager.simulationHasBeenLaunched)
            GameManager.playerCanModifyBoard = true;
    }

    public void DisplayControls()
    {
        pauseMenu.SetActive(false);
        controlsScheme.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        levelLoader.loadSpecificLevel(0);
    }

    public void BackButton()
    {
        if (controlsScheme.activeSelf)
        {
            controlsScheme.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        if (!MainCamera.isFreeLookActive)
        {
            Time.timeScale = gameManager.simulationSpeed = 1f;
            UnselectAllTiles();
            GameManager.simulationHasBeenLaunched = false;
            levelLoader.loadNextLevel();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}