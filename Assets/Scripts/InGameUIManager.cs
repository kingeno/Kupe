using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameManager gameManager;
    public MainCamera _mainCamera;

    public GameObject pauseMenu;
    public GameObject controlsScheme;
    public GameObject winScreen;

    public GameObject playSimulationButton;
    public GameObject pauseSimulationButton;

    public Transform blankTilePrefab;
    public Transform greenArrowPrefab;
    
public static bool isDeleteTileSelected;
    public static bool isGreenArrowSelected;
    public static bool unselectAll;

    private void Start()
    {
        unselectAll = false;

        if (!gameManager)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        if (!_mainCamera)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();
        }
        if (!levelLoader)
        {
            levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();  //if you encounter a null reference exception here it means that you have launched the game without going through the main menu
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("right mouse input");
            unselectAll = true;
        }
        else
            unselectAll = false;

        if (unselectAll)
        {
            isGreenArrowSelected = false;
            isDeleteTileSelected = false;
            Debug.Log("unselect all tiles");
        }


        if (!GameManager.simulationIsRunning && pauseSimulationButton.activeSelf)
        {
            pauseSimulationButton.SetActive(false);
            playSimulationButton.SetActive(true);
            if (_mainCamera)
            {
                _mainCamera.backgroundColorSwap();
            }
        }
        else if (GameManager.simulationIsRunning && playSimulationButton.activeSelf)
        {
            playSimulationButton.SetActive(false);
            pauseSimulationButton.SetActive(true);
            if (_mainCamera)
            {
                _mainCamera.backgroundColorSwap();
            }
        }


        if (GameManager.levelIsRestart)
        {
            playSimulationButton.SetActive(true);
            pauseSimulationButton.SetActive(false);
            GameManager.levelIsRestart = false;
        }

        if (GameManager.levelIsCompleted && !winScreen.activeSelf)
        {
            winScreen.SetActive(true);
        }
    }

    public void DeleteSelection()
    {
        unselectAll = true;
        if (unselectAll)
        {
            isGreenArrowSelected = false;
            isDeleteTileSelected = false;
        }
        if (!isDeleteTileSelected)
        {
            isDeleteTileSelected = true;
            Debug.Log("delete button selected = " + isDeleteTileSelected);
        }
    }

    public void GreenArrowSelection()
    {
        unselectAll = true;
        if (unselectAll)
        {
            isGreenArrowSelected = false;
            isDeleteTileSelected = false;
        }
        if (!CurrentLevelManager.isGreenArrowStockEmpty && !isGreenArrowSelected)
        {
            isGreenArrowSelected = true;
            Debug.Log("green arrow tile selected = " + isGreenArrowSelected);
        }
    }

    public void SimulationButton()
    {
        if (!GameManager.simulationIsRunning)
        {
            gameManager.LaunchSimulation();
            playSimulationButton.SetActive(true);
            pauseSimulationButton.SetActive(false);
            if (_mainCamera)
            {
                _mainCamera.backgroundColorSwap();
            }
        }
        else if (GameManager.simulationIsRunning)
        {
            gameManager.LaunchSimulation();
            playSimulationButton.SetActive(false);
            pauseSimulationButton.SetActive(true);
            if (_mainCamera)
            {
                _mainCamera.backgroundColorSwap();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
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
        levelLoader.loadNextLevel();
    }

    public void RestartLevel()
    {
        gameManager.RestartLevel();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}