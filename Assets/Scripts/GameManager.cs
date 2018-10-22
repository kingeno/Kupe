using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public PlayerController playerController;
    public PlayerController playerController2;

    public static int turnCount;
    public static int currentTurn;

    public static bool turnIsFinished;
    public static bool playerHasLaunchedSimulation;

    public static float staticTargetTime = 0.5f;
    public static float targetTime;
    public float initialTargetTime;

    private void Start()
    {
        playerHasLaunchedSimulation = false;
        targetTime = initialTargetTime;
        staticTargetTime = initialTargetTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            playerHasLaunchedSimulation = true;
        currentTurn = turnCount;
        turnIsFinished = false;
    }

    void OnGUI()
    {
        GUIStyle whiteStyle = new GUIStyle();
        whiteStyle.fontSize = 16;
        whiteStyle.normal.textColor = Color.white;

        GUIStyle redStyle = new GUIStyle();
        redStyle.fontSize = 16;
        redStyle.normal.textColor = Color.red;

        if (!playerController.hasReachedEndTile && !playerController2.hasReachedEndTile)
            GUI.Box(new Rect(10, 10, 100, 20), "turn: " + turnCount.ToString(), whiteStyle);
        if (playerController.hasReachedEndTile && playerController2.hasReachedEndTile)
            GUI.Box(new Rect(10, 10, 100, 20), "turn: " + turnCount.ToString() + " Has reached end tile !", redStyle);
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
}
