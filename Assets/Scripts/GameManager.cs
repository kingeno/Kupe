using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] cubes;
    public CubeController[] cubesControllers;

    public GameObject[] endTiles;
    public EndTile[] endTileScripts;

    public bool startTurn;
    public static int turnCount;
    public static int currentTurn;
    public static bool turnIsFinished;

    public static float staticTargetTime = 0.5f;
    public static float targetTime;
    public float initialTargetTime;

    public static bool playerHasLaunchedSimulation;

    private void Start()
    {
        playerHasLaunchedSimulation = false;
        startTurn = false;
        turnCount = 0;

        cubes = GameObject.FindGameObjectsWithTag("Cube");
        cubesControllers = new CubeController[cubes.Length];
        int i = 0;
        foreach (GameObject cube in cubes)
        {
            cubesControllers[i] = cube.GetComponent<CubeController>();
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

    public void PredictCubesNextTurnPos()
    {
        foreach (CubeController cube in cubesControllers)
        {
            cube.predicted_NextTurnPos = cube.PredictNextTurnPos(cube.TileCheck(cube.belowAdjacentPos));
        }
    }

    public void CompareCubesPredictedNextTurnPositions()
    {
        foreach (CubeController cube in cubesControllers)
        {
            if (cube.willMove)
            {
                for (int i = 0; i < cubesControllers.Length; i++)
                {
                    if (cube.cubeNumber != cubesControllers[i].cubeNumber && cube.predicted_NextTurnPos == cubesControllers[i].predicted_NextTurnPos)
                    {
                        if (cube.willMoveForward && cube.TileCheck(cube.frontTwoTilesAwayPos).GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                        {
                            cube.willRoundTrip = cube.willRoundTripForward = true;
                            cube.willMove = cube.willMoveForward = false;
                        }
                        if (cube.willMoveBack && cube.TileCheck(cube.backTwoTilesAwayPos).GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                        {
                            cube.willRoundTrip = cube.willRoundTripBack = true;
                            cube.willMove = cube.willMoveBack = false;
                        }
                        if (cube.willMoveRight && cube.TileCheck(cube.rightTwoTilesAwayPos).GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                        {
                            cube.willRoundTrip = cube.willRoundTripRight = true;
                            cube.willMove = cube.willMoveRight = false;
                        }
                        if (cube.willMoveLeft && cube.TileCheck(cube.lefttTwoTilesAwayPos).GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                        {
                            cube.willRoundTrip = cube.willRoundTripLeft = true;
                            cube.willMove = cube.willMoveLeft = false;
                        }
                        if (cube.willMoveDown && cube.TileCheck(cube.belowTwoTilesAwayPos).GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                        {
                            cube.willRoundTrip = cube.willRoundTripDown = true;
                            cube.willMove = cube.willMoveDown = false;
                            // if the cube is at the upper exit of an elevator witch is going to bring up the compared cube next turn
                            // How elevator works :    
                            // turn 1 : a cube arrives on it
                            // turn 2 : if the elevator is down it moves up || if the elevator is up it goes down
                            // turn 3 : if the elevator has an arrow on it, it gives a direction to the cube
                            // turn 3 : if the elevator is blank the player will move to the direction it has at turn 1 (when arriving on the elevator)
                        }
                        if (cube.TileCheck(cube.belowFront_AdjacentPos).GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                        {
                            cube.willNotMove = true;
                            cube.willMove = cube.willMoveForward = false;
                        }
                        if (cube.TileCheck(cube.belowFront_AdjacentPos).GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                        {
                            cube.willNotMove = true;
                            cube.willMove = cube.willMoveForward = false;
                        }
                    }
                }
            }
            //else if (cube.willNotMove)
            //{

            //}
        }
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

    //void OnGUI()
    //{
    //    GUIStyle whiteStyle = new GUIStyle();
    //    whiteStyle.fontSize = 26;
    //    whiteStyle.normal.textColor = Color.white;

    //    GUIStyle redStyle = new GUIStyle();
    //    redStyle.fontSize = 26;
    //    redStyle.normal.textColor = Color.red;

    //    GUIStyle greenStyle = new GUIStyle();
    //    greenStyle.fontSize = 26;
    //    greenStyle.normal.textColor = Color.green;

    //    if (allEndTilesAreValidated)
    //        GUI.Box(new Rect(10, 10, 2000, 40), "turn: " + turnCount.ToString(), whiteStyle);
    //    else if (playerHasWon)
    //        GUI.Box(new Rect(10, 10, 2000, 40), " You solved the puzzle !", greenStyle);
    //    else if (playerHasLost)
    //        GUI.Box(new Rect(10, 10, 2000, 40), "You Lost... Try again !", redStyle);
    //}
}
