using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] players;
    public PlayerController[] playerControllers;

    public GameObject[] endTiles;
    public EndTile[] endTileScripts;

    public Vector3[] startPos;

    public static bool resetTileState;

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

    public bool allPlayersHaveFinishedTheirTurn;
    public bool allEndTilesAreValidated;
    public bool aPlayerIsOutOfBoardRange;

    private void Start()
    {
        resetTileState = false;
        simulationCanBeLaunched = false;
        turnCount = 0;
        playerHasLaunchedSimulation = false;
        simulationHasEnded = false;
        playerHasLost = false;
        playerHasWon = false;
        targetTime = initialTargetTime;
        staticTargetTime = initialTargetTime;
        allPlayersHaveFinishedTheirTurn = false;

        players = GameObject.FindGameObjectsWithTag("Player");
        playerControllers = new PlayerController[players.Length];
        startPos = new Vector3[players.Length];

        int i = 0;
        foreach (GameObject player in players)
        {
            playerControllers[i] = player.GetComponent<PlayerController>();
            startPos[i] = playerControllers[i].startPos;
            i++;
        }

        endTiles = GameObject.FindGameObjectsWithTag("End Tile");
        endTileScripts = new EndTile[endTiles.Length];
        i = 0;
        foreach (GameObject endTile in endTiles)
        {
            endTileScripts[i] = endTile.GetComponent<EndTile>();
            i++;
        }
    }

    private void Update()
    {
        if (TileUIManager.levelIsReset == true)
        {
            RestartLevel();
            TileUIManager.levelIsReset = false;
        }

        if (simulationCanBeLaunched)
            playerHasLaunchedSimulation = true;


        if (playerHasLaunchedSimulation && !simulationHasEnded)
        {
            foreach (PlayerController controller in playerControllers)
            {
                if (controller.hasFinishItsTurn)
                    allPlayersHaveFinishedTheirTurn = true;
                else if (!controller.hasFinishItsTurn)
                    allPlayersHaveFinishedTheirTurn = false;
            }

            if (allPlayersHaveFinishedTheirTurn)
            {
                TurnTimer();
                if (turnIsFinished)
                {
                    turnCount++;
                    currentTurn = turnCount;
                    Debug.LogWarning("turn " + turnCount + " has ended");

                    foreach (PlayerController controller in playerControllers)
                    {
                        controller.hasFinishItsTurn = false;
                    }

                    foreach (EndTile endtile in endTileScripts)
                    {
                        if (endtile.hasAPlayerCubeOnIt)
                            allEndTilesAreValidated = true;
                        else if (!endtile.hasAPlayerCubeOnIt)
                        {
                            allEndTilesAreValidated = false;
                        }
                    }
                }
            }

            foreach (PlayerController controller in playerControllers)
            {
                if (allPlayersHaveFinishedTheirTurn)
                {
                    if (controller.isOutOfBoardRange)
                        aPlayerIsOutOfBoardRange = true;
                    else if (!controller.isOutOfBoardRange)
                        aPlayerIsOutOfBoardRange = false;
                }
            }

            if (aPlayerIsOutOfBoardRange)
            {
                allPlayersHaveFinishedTheirTurn = true;
                turnIsFinished = false;
                simulationHasEnded = true;
                Debug.LogWarning("simulation has ended = " + simulationHasEnded);
                playerHasLaunchedSimulation = false;
                playerHasLost = true;
            }

            allPlayersHaveFinishedTheirTurn = false;

            if (!allPlayersHaveFinishedTheirTurn)
            {
                turnIsFinished = false;
            }

            if (allEndTilesAreValidated)
            {
                allPlayersHaveFinishedTheirTurn = true;
                turnIsFinished = true;

                playerHasLaunchedSimulation = false;
                simulationHasEnded = true;
                playerHasWon = true;
                Debug.LogWarning("simulation has ended = " + simulationHasEnded);
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
            TimerEnded();
        }
    }

    public static void TimerEnded()
    {
        turnIsFinished = true;
        staticTargetTime = targetTime;
    }

    public void RestartLevel()
    {
        playerHasLaunchedSimulation = false;
        simulationHasEnded = false;
        simulationCanBeLaunched = false;
        resetTileState = true;
        playerHasWon = false;
        turnCount = 0;
        int i = 0;
        foreach (GameObject player in players)
        {
            player.transform.position = startPos[i];
            i++;
        }
        foreach (PlayerController playerScript in playerControllers)
        {
            playerScript.hasReachedEndTile = false;
            playerScript.hasFinishItsTurn = false;
        }
        foreach (EndTile endtile in endTileScripts)
        {
            endtile.hasAPlayerCubeOnIt = false;
        }
        allEndTilesAreValidated = false;

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

        if (allEndTilesAreValidated)
            GUI.Box(new Rect(10, 10, 2000, 40), "turn: " + turnCount.ToString(), whiteStyle);
        else if (playerHasWon)
            GUI.Box(new Rect(10, 10, 2000, 40), " You solved the puzzle !", greenStyle);
        else if (playerHasLost)
            GUI.Box(new Rect(10, 10, 2000, 40), "You Lost... Try again !", redStyle);
    }
}
