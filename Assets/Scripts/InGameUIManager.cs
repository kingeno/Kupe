using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class InGameUIManager : MonoBehaviour
{
    private GameManager gameManager;
    private LevelLoader levelLoader;
    private MainCamera _mainCamera;
    private GameObject cinemachineCamera;

    [Header("Level Completed Text")]
    public float levelCompletedText_displayDelay;
    public float levelCompletedText_distanceToTravel;
    public float levelCompletedText_timeOfTravel;
    public bool levelCompletedText_fadeIsDone;

    [Header("Next Level Button")]
    public float nextLevelButton_displayDelay;
    public float nextLevelButton_distanceToTravel;
    public float nextLevelButton_timeOfTravel;
    public bool nextLevelButton_fadeIsDone;

    [Header("Level Name")]
    public TextMeshProUGUI levelNameText;

    [Header("Tile Prefabs")]
    public Transform blankTilePrefab;
    public Transform greenArrowPrefab;

    [Header("General UI")]
    public GameObject inGameUI;
    public GameObject pauseMenu;
    public GameObject levelHub;
    public GameObject controlsScheme;
    public GameObject winScreen;

    [Header("Level Completed UI")]
    public GameObject levelCompletedText;
    public GameObject nextLevelButton;

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
    public GreenArrowStockText greenArrowStockText;

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


    float normalizedTime = 0;

    private void Start()
    {
        SetLevelNameText();

        //greenArrowStockText.SetArrowStockToDisplay();

        GameManager.gameIsPaused = false;
        nothingIsSelected = true;
        isDeleteTileSelected = false;
        isGreenArrowSelected = false;
        changeSpeedSimulation = false;

        levelCompletedText_fadeIsDone = false;
        nextLevelButton_fadeIsDone = false;

        if (!gameManager)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (!_mainCamera)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();

        if (!levelLoader)
            levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();  //if you encounter a null reference exception here it means that you have launched the game without going through the main menu
    }

    private bool levelCompletedText_fadeStarted = false;
    private bool nextLevelButton_fadeStarted = false;

    private void Update()
    {

        if (!cinemachineCamera)
        {
            cinemachineCamera = GameObject.FindGameObjectWithTag("CMFreeLookCamera");
            if (cinemachineCamera)
                cinemachineCamera.SetActive(false);
        }

        if (!GameManager.gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                PauseMenu();

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
            }
            else if (GameManager.simulationIsRunning && GameManager.simulationHasBeenLaunched && playSimulationButton.activeSelf)
            {
                playSimulationButton.SetActive(false);
                pauseSimulationButton.SetActive(true);
            }

            if (GameManager.levelIsCompleted)
            {
                if (!winScreen.activeSelf)
                {
                    winScreen.SetActive(true);
                }
                else if (winScreen.activeSelf && !levelCompletedText_fadeStarted)
                {
                    AudioManager.instance.Play("ig level completed");
                    StartCoroutine(FadeAndMoveText(levelCompletedText, levelCompletedText_displayDelay, levelCompletedText_timeOfTravel,
                    new Vector2(levelCompletedText.GetComponent<RectTransform>().anchoredPosition.x, levelCompletedText.GetComponent<RectTransform>().anchoredPosition.y),
                    new Vector2(levelCompletedText.GetComponent<RectTransform>().anchoredPosition.x, levelCompletedText.GetComponent<RectTransform>().anchoredPosition.y + levelCompletedText_distanceToTravel)));

                    StartCoroutine(FadeAndMoveText(nextLevelButton, nextLevelButton_displayDelay, nextLevelButton_timeOfTravel,
                    new Vector2(nextLevelButton.GetComponent<RectTransform>().anchoredPosition.x, nextLevelButton.GetComponent<RectTransform>().anchoredPosition.y),
                    new Vector2(nextLevelButton.GetComponent<RectTransform>().anchoredPosition.x, nextLevelButton.GetComponent<RectTransform>().anchoredPosition.y + nextLevelButton_distanceToTravel)));

                    levelCompletedText_fadeStarted = true;
                }
            }

            if (isOverPlayerArrowTile)
            {
                contextualWindow.SetActive(true);
                contextualWindowText.text = "rotate : r / left clic";
            }
            else
                contextualWindow.SetActive(false);
        }

        else if (GameManager.gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (controlsScheme.activeSelf)
                {
                    controlsScheme.SetActive(false);
                    pauseMenu.SetActive(true);
                }
                else if (levelHub.activeSelf)
                {
                    levelHub.SetActive(false);
                    pauseMenu.SetActive(true);
                }
                else if (pauseMenu.activeSelf)
                    ResumeGame();
            }
        }

    }

    public void UnselectAllTiles()
    {
        isGreenArrowSelected = false;
        isDeleteTileSelected = false;
        deleteTileSelectedOutline.SetActive(false);
        greenArrowSelectedOutline.SetActive(false);
        nothingIsSelected = true;
        //greenArrowStockText.SetArrowStockToDisplay();
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

                greenArrowStockText.SetArrowStockToDisplay();
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

                AudioManager.instance.Play("ig simulation play");
            }
            else if (GameManager.simulationIsRunning)
            {
                gameManager.LaunchSimulation();
                AudioManager.instance.Play("ig simulation pause");
            }
        }
    }

    public void SpeedUpSimulation()
    {
        AudioManager.instance.Play("ig simulation speed up");
        if (GameManager.simulationSpeed == 1f)
        {
            changeSpeedSimulation = true;
            GameManager.simulationSpeed = 2f;
            gameManager.turnTime = 0.47f;
        }
        else if (GameManager.simulationSpeed == 2f)
        {
            changeSpeedSimulation = true;
            GameManager.simulationSpeed = 3f;
            gameManager.turnTime = 0.44f;
        }
        else if (GameManager.simulationSpeed == 3f)
        {
            changeSpeedSimulation = true;
            GameManager.simulationSpeed = 1f;
            gameManager.turnTime = 0.5f;
        }
    }

    public void SlowDownSimulation()
    {
        AudioManager.instance.Play("ig simulation slow down");
        if (GameManager.simulationSpeed == 1f)
        {
            changeSpeedSimulation = true;
            GameManager.simulationSpeed = 3f;
            gameManager.turnTime = 0.44f;
        }
        else if (GameManager.simulationSpeed == 2f)
        {
            changeSpeedSimulation = true;
            GameManager.simulationSpeed = 1f;
            gameManager.turnTime = 0.5f;
        }
        else if (GameManager.simulationSpeed == 3f)
        {
            changeSpeedSimulation = true;
            GameManager.simulationSpeed = 2f;
            gameManager.turnTime = 0.47f;
        }
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

            AudioManager.instance.Play("ig simulation stop");
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
            AudioManager.instance.Play("ig pause menu open");
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
        AudioManager.instance.Play("menu button close");
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

    public void DisplayLevelHub()
    {
        pauseMenu.SetActive(false);
        levelHub.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        GameManager.simulationSpeed = Time.timeScale = 1f;
        levelLoader.loadSpecificLevel(0);
        GameManager.levelIsCompleted = false;
    }

    public void BackButton()
    {
        if (controlsScheme.activeSelf)
        {
            controlsScheme.SetActive(false);
            pauseMenu.SetActive(true);
        }
        else if (levelHub.activeSelf)
        {
            levelHub.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        if (!MainCamera.isFreeLookActive)
        {
            UnselectAllTiles();
            GameManager.simulationHasBeenLaunched = false;
            levelLoader.loadNextLevel();
        }
    }

    private void SetLevelNameText()
    {
        levelNameText.text = "Level " + SceneManager.GetActiveScene().buildIndex;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeAndMoveText(GameObject target, float timeToWaitBeforeFade, float timeOfTravel, Vector2 startPos, Vector2 endPos)
    {
        yield return new WaitForSecondsRealtime(timeToWaitBeforeFade);

        float currentTime = 0f;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.unscaledDeltaTime;
            normalizedValue = currentTime / timeOfTravel;

            target.GetComponent<CanvasGroup>().alpha = EasingFunction.EaseInOutSine(0f, 1f, normalizedValue);
            target.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, EasingFunction.EaseOutExpo(0f, 1f, normalizedValue));
            yield return null;
        }
    }
}