using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public PlayerController playerController;
    public PlayerController playerController2;

    public GameObject[] players;
    public PlayerController[] playerControllers;

    public static int turnCount;
    public static int currentTurn;

    public static bool turnIsFinished;
    public static bool playerHasLaunchedSimulation;
    public static bool simulationHasEnded;
    public static bool playerHasLost;
    public static bool playerHasWon;

    public static float staticTargetTime = 0.5f;
    public static float targetTime;
    public float initialTargetTime;

    public bool simulationCanBeLaunched;

    private void Start()
    {
        simulationCanBeLaunched = false;
        turnCount = 0;
        playerHasLaunchedSimulation = false;
        simulationHasEnded = false;
        playerHasLost = false;
        playerHasWon = false;
        targetTime = initialTargetTime;
        staticTargetTime = initialTargetTime;

        players = GameObject.FindGameObjectsWithTag("Player");
        playerControllers = new PlayerController[players.Length];
        int i = 0;
        foreach (GameObject player in players)
        {
            playerControllers[i] = player.GetComponent<PlayerController>();
            Debug.Log("caca " + i);
            i++;
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //if (!playerHasLaunchedSimulation)
        //    //    Debug.LogWarning("turn " + turnCount + " has ended");
        //    //playerHasLaunchedSimulation = true;
        //    LaunchSimulation();
        //}
        if (simulationCanBeLaunched)
            playerHasLaunchedSimulation = true;

        if (playerHasLaunchedSimulation && !simulationHasEnded)
        {
            if (playerController.hasFinishItsTurn && playerController2.hasFinishItsTurn)
            {
                TurnTimer();
                if (turnIsFinished)
                {
                    playerController.hasFinishItsTurn = false;
                    playerController2.hasFinishItsTurn = false;
                    turnCount++;
                    currentTurn = turnCount;
                    Debug.LogWarning("turn " + turnCount + " has ended");
                }
            }
            if (!playerController.hasFinishItsTurn && !playerController2.hasFinishItsTurn)
            {
                if (playerController.isOutOfTheBoard && playerController2.isOutOfTheBoard
                    || !playerController.isOutOfTheBoard && playerController2.isOutOfTheBoard
                    || playerController.isOutOfTheBoard && !playerController2.isOutOfTheBoard)
                {
                    turnIsFinished = false;
                    simulationHasEnded = true;
                    Debug.LogWarning("simulation has ended = " + simulationHasEnded);
                    playerHasLaunchedSimulation = false;
                    playerHasLost = true;
                }
                else
                    turnIsFinished = false;
            }

            if (playerController.hasReachedEndTile && playerController2.hasReachedEndTile)
            {
                turnIsFinished = false;
                simulationHasEnded = true;
                Debug.LogWarning("simulation has ended = " + simulationHasEnded);
                playerHasLaunchedSimulation = false;
                playerHasWon = true;
            }
        }




        // Rethink the logic of how I handle the multiple cubes controllers by turn
        if (simulationCanBeLaunched)
            playerHasLaunchedSimulation = true;

        if (playerHasLaunchedSimulation && !simulationHasEnded)
        {
            if (playerController.hasFinishItsTurn && playerController2.hasFinishItsTurn)
            {
                TurnTimer();
                if (turnIsFinished)
                {
                    playerController.hasFinishItsTurn = false;
                    playerController2.hasFinishItsTurn = false;
                    turnCount++;
                    currentTurn = turnCount;
                    Debug.LogWarning("turn " + turnCount + " has ended");
                }
            }
            if (!playerController.hasFinishItsTurn && !playerController2.hasFinishItsTurn)
            {
                if (playerController.isOutOfTheBoard && playerController2.isOutOfTheBoard
                    || !playerController.isOutOfTheBoard && playerController2.isOutOfTheBoard
                    || playerController.isOutOfTheBoard && !playerController2.isOutOfTheBoard)
                {
                    turnIsFinished = false;
                    simulationHasEnded = true;
                    Debug.LogWarning("simulation has ended = " + simulationHasEnded);
                    playerHasLaunchedSimulation = false;
                    playerHasLost = true;
                }
                else
                    turnIsFinished = false;
            }

            if (playerController.hasReachedEndTile && playerController2.hasReachedEndTile)
            {
                turnIsFinished = false;
                simulationHasEnded = true;
                Debug.LogWarning("simulation has ended = " + simulationHasEnded);
                playerHasLaunchedSimulation = false;
                playerHasWon = true;
            }
        }
    }

    public void LaunchSimulation()
    {
        simulationCanBeLaunched = true;
    }

    public static void TurnTimer()
    {
        staticTargetTime -= Time.deltaTime;

        if (staticTargetTime <= 0.0f)
        {
            timerEnded();
        }
    }

    public static void timerEnded()
    {
        turnIsFinished = true;
        staticTargetTime = targetTime;
    }

    void OnGUI()
    {
        GUIStyle whiteStyle = new GUIStyle();
        whiteStyle.fontSize = 26;
        whiteStyle.normal.textColor = Color.white;

        GUIStyle redStyle = new GUIStyle();
        redStyle.fontSize = 26;
        redStyle.normal.textColor = Color.red;

        GUIStyle greenStyle = new GUIStyle();
        greenStyle.fontSize = 26;
        greenStyle.normal.textColor = Color.green;

        if (!playerController.hasReachedEndTile && !playerController2.hasReachedEndTile)
            GUI.Box(new Rect(10, 10, 2000, 40), "turn: " + turnCount.ToString(), whiteStyle);
        else if (playerHasWon)
            GUI.Box(new Rect(10, 10, 2000, 40), " You solved the puzzle !", greenStyle);
        else if (playerHasLost)
            GUI.Box(new Rect(10, 10, 2000, 40), "You Lost... Try again !", redStyle);
    }
}
