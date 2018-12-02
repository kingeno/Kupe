﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;

    public GameObject[] cubes;
    public CubeController[] cubesControllers;

    public GameObject[] endTiles;
    public EndTile[] endTilesControllers;

    public GameObject[] greenArrows;
    public GreenArrow[] greenArrowsControllers;

    public GameObject[] player_GreenArrows;
    public Player_GreenArrow[] player_GreenArrowsControllers;

    public static int turnCount;
    public static int currentTurn;
    public static bool turnStart;
    public static bool turnIsFinished;

    public float currentTurnTime;
    public float turnTime = 0.6f;
    public float initialTurnTime;

    public static bool simulationIsRunning;

    public bool allEndTilesAreValidated;

    private void Start()
    {
        currentTurnTime = turnTime;
        simulationIsRunning = false;
        allEndTilesAreValidated = false;
        turnCount = 0;

        boardManager = GameObject.FindGameObjectWithTag("Board Manager").GetComponent<BoardManager>();

        cubes = GameObject.FindGameObjectsWithTag("Cube");  // find all object of a certain type and put it in an array
        cubesControllers = new CubeController[cubes.Length]; // uses the previous made array to determine the lenght of the script array
        for (int i = 0; i < cubesControllers.Length; i++)
        {
            cubesControllers[i] = cubes[i].GetComponent<CubeController>();
        }

        endTiles = GameObject.FindGameObjectsWithTag("End Tile");
        endTilesControllers = new EndTile[endTiles.Length];
        for (int i = 0; i < endTilesControllers.Length; i++)
        {
            endTilesControllers[i] = endTiles[i].GetComponent<EndTile>();
        }

        greenArrows = GameObject.FindGameObjectsWithTag("Green Arrow");
        greenArrowsControllers = new GreenArrow[greenArrows.Length];
        for (int i = 0; i < greenArrowsControllers.Length; i++)
        {
            greenArrowsControllers[i] = greenArrows[i].GetComponent<GreenArrow>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchSimulation();
        }

        if (simulationIsRunning)
            TurnTimer();

        if (simulationIsRunning && !allEndTilesAreValidated)
        {
            //player_GreenArrows = GameObject.FindGameObjectsWithTag("Player Green Arrow");
            //player_GreenArrowsControllers = new Player_GreenArrow[player_GreenArrows.Length];
            //for (int i = 0; i < player_GreenArrowsControllers.Length; i++)
            //{
            //    player_GreenArrowsControllers[i] = player_GreenArrows[i].GetComponent<Player_GreenArrow>();
            //}

            if (!turnStart && turnIsFinished)
            {
                foreach (EndTile endTile in endTilesControllers)
                {
                    endTile.TurnInitializer();
                }

                CheckIfEndTilesAreValidated();

                if (!allEndTilesAreValidated && simulationIsRunning)
                {
                    foreach (CubeController cube in cubesControllers)
                    {
                        cube.TurnInitializer();
                    }

                    PredictCubesNextTurnPos();
                    CompareCubesPredictedNextTurnPositions();
                    AnimateCube();
                    turnStart = true;
                }

                foreach (GreenArrow greenArrow in greenArrowsControllers)
                {
                    greenArrow.TurnInitializer();
                }
                foreach (Player_GreenArrow playerGreenArrow in player_GreenArrowsControllers)
                {
                    playerGreenArrow.TurnInitializer();
                }

                if (!allEndTilesAreValidated && simulationIsRunning)
                {
                    currentTurn++;
                    Debug.LogWarning("turn: " + currentTurn);
                }
            }
        }
    }

    public void LaunchSimulation()
    {
        if (!simulationIsRunning)
        {
            simulationIsRunning = true;

            player_GreenArrows = GameObject.FindGameObjectsWithTag("Player Green Arrow");
            player_GreenArrowsControllers = new Player_GreenArrow[player_GreenArrows.Length];
            for (int i = 0; i < player_GreenArrowsControllers.Length; i++)
            {
                player_GreenArrowsControllers[i] = player_GreenArrows[i].GetComponent<Player_GreenArrow>();
            }
        }
        else
            simulationIsRunning = false;
    }

    public void TurnTimer()
    {
        currentTurnTime -= Time.deltaTime;
        if (currentTurnTime <= 0.0f)
        {
            TimerEnded();
        }
    }

    public void TimerEnded()
    {
        turnIsFinished = true;
        turnStart = false;
        currentTurnTime = turnTime;
    }

    public void CheckIfEndTilesAreValidated()
    {
        if (endTilesControllers.Length > 1)
        {
            for (int i = 0; i < endTilesControllers.Length; i++)
            {
                if (!endTilesControllers[i].isValidated)
                {
                    allEndTilesAreValidated = false;
                    break;
                }
                else if (i < endTilesControllers.Length - 1 && endTilesControllers[i].isValidated)
                {
                    continue;
                }
                else if (i == endTilesControllers.Length - 1 && endTilesControllers[i].isValidated)
                {
                    allEndTilesAreValidated = true;
                    simulationIsRunning = false;
                    break;
                }
            }
        }
        else if (endTilesControllers.Length == 1)
        {
            foreach (EndTile endTile in endTilesControllers)
            {
                if (endTile.isValidated)
                    allEndTilesAreValidated = true;
                else
                    allEndTilesAreValidated = false;
            }
        }
    }

    public void PredictCubesNextTurnPos()
    {
        foreach (CubeController cube in cubesControllers)
        {
            cube.predicted_NextTurnPos = cube.PredictNextTurnPos(cube.TileCheck(cube.below_AdjacentPos));
        }
    }

    public void CompareCubesPredictedNextTurnPositions()
    {
        foreach (CubeController cube in cubesControllers)
        {
            Debug.Log(cube.name + " predicted_NextTurnPos= " + cube.predicted_NextTurnPos);

            if (cube.willMove && cubesControllers.Length > 1) //(1)
            {
                for (int i = 0; i < cubesControllers.Length; i++)
                {
                    //Debug.Log(i + " " + cubesControllers.Length);

                    if (cube.willMove && cube.cubeNumber != cubesControllers[i].cubeNumber)
                    {
                        if (cube.predicted_NextTurnPos != cubesControllers[i].predicted_NextTurnPos)
                        {
                            // cube will do the predicted movement --> meaning that it will eather move or not move but will not do a round-trip
                            cube.willRoundTrip = false;
                            cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
                            //if (i == cubesControllers.Length - 1)
                            Debug.Log(cube.name + " do the predicted movement.");
                            continue;
                        }
                        else if (cube.predicted_NextTurnPos == cubesControllers[i].predicted_NextTurnPos) //(2)
                        {
                            // -------- front
                            if (cube.willMoveForward)
                            {
                                Transform tile = cube.TileCheck(cube.belowFront_AdjacentPos); // below front
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveForward = true;
                                    cube.willRoundTrip = cube.willRoundTripForward = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.aboveFront_AdjacentPos); // above front
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveForward = false;
                                    cube.willRoundTrip = cube.willRoundTripForward = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.frontRight_DiagonalPos); // front right
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveForward = false;
                                    cube.willRoundTrip = cube.willRoundTripForward = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.frontLeft_DiagonalPos);  // front left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveForward = false;
                                    cube.willRoundTrip = cube.willRoundTripForward = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.front_TwoTilesAwayPos);    // front two tiles away
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripForward = true;
                                    cube.willMove = cube.willMoveForward = false;
                                    cube.lastMovement = new Vector3(0, 0, -1);
                                    tile = null;
                                    break;
                                }
                            }
                            // -------- back
                            else if (cube.willMoveBack)
                            {
                                Transform tile = cube.TileCheck(cube.belowBack_AdjacentPos); // below back
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveBack = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.aboveBack_AdjacentPos);    // above back
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveBack = false;
                                    cube.willRoundTrip = cube.willRoundTripBack = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.backRight_DiagonalPos);   // back right
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveBack = false;
                                    cube.willRoundTrip = cube.willRoundTripBack = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.backLeft_DiagonalPos);    // back left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveBack = false;
                                    cube.willRoundTrip = cube.willRoundTripBack = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.back_TwoTilesAwayPos); // back two tiles away
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripBack = true;
                                    cube.willMove = cube.willMoveBack = false;
                                    cube.lastMovement = new Vector3(0, 0, 1);
                                    tile = null;
                                }
                            }
                            // -------- right
                            else if (cube.willMoveRight)
                            {
                                Transform tile = cube.TileCheck(cube.belowRight_AdjacentPos);  // below right
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveRight = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.aboveRight_AdjacentPos);  // above right
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveRight = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.frontRight_DiagonalPos);  // front right
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveRight = false;
                                    cube.willRoundTrip = cube.willRoundTripRight = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.backRight_DiagonalPos);   // back right
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveRight = false;
                                    cube.willRoundTrip = cube.willRoundTripRight = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.right_TwoTilesAwayPos);     // right two tiles away
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripRight = true;
                                    cube.willMove = cube.willMoveRight = false;
                                    cube.lastMovement = new Vector3(-1, 0, 0);
                                    tile = null;
                                }
                            }
                            // -------- left
                            else if (cube.willMoveLeft)
                            {
                                Transform tile = cube.TileCheck(cube.belowLeft_AdjacentPos);   // below left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.aboveLeft_AdjacentPos);   // above left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.frontLeft_DiagonalPos);   // front left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.willRoundTrip = cube.willRoundTripLeft = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.backLeft_DiagonalPos);    // back left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.willRoundTrip = cube.willRoundTripLeft = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.left_TwoTilesAwayPos);  // left two tiles away
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willRoundTrip = cube.willRoundTripLeft = true;
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.lastMovement = new Vector3(1, 0, 0);
                                    tile = null;
                                }
                            }
                            // -------- below
                            else if (cube.willMoveDown)
                            {
                                Transform tile = cube.TileCheck(cube.belowFront_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.belowBack_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.belowRight_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.belowLeft_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.below_TwoTilesAwayPos);     // below two tiles away
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripDown = true;
                                    cube.willMove = cube.willMoveDown = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                // if the cube is at the upper exit of an elevator witch is going to bring up the compared cube next turn
                                // How elevator works :    
                                // turn 1 : a cube arrives on it
                                // turn 2 : if the elevator is down it moves up || if the elevator is up it goes down
                                // turn 3 : if the elevator has an arrow on it, it gives a direction to the cube
                                // turn 3 : if the elevator is blank the player will move to the direction it has at turn 1 (when arriving on the elevator)
                            }
                            else if (cube.willMoveUp)
                            {
                                Transform tile = cube.TileCheck(cube.above_TwoTilesAwayPos);     // above two tiles away
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    Debug.Log(cube.name + " A");
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (cube.willMove && cubesControllers.Length == 1)
            {
                cube.willRoundTrip = false;
                cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
                //if (i == cubesControllers.Length - 1)
                Debug.Log(cube.name + " do the predicted movement.");
                continue;
            }
            if (!cube.willMove && !cube.willRoundTrip) // (4)
            {
                cube.willMove = cube.willMoveForward = cube.willMoveBack = cube.willMoveRight = cube.willMoveLeft = cube.willMoveUp = cube.willMoveDown = false;
                cube.willRoundTrip = cube.willRoundTripForward = cube.willRoundTripBack = cube.willRoundTripRight = cube.willRoundTripLeft = cube.willRoundTripUp = cube.willRoundTripDown = false;
            }
            // do an array that regroup every movement boolean and check if at least one of them is true --> go to animation phase (this sould be a method within the CubeController script)
            // if there is no true boolean in the array display an error message in the console
            cube.AnimationBooleanChecker();
        }
    }

    public void AnimateCube()
    {
        // foreach cube in the cubeControllers array trigger the correct animation
        foreach (CubeController cube in cubesControllers)
        {
            cube.TriggerAnimation();
        }
    }

    public void RestartLevel()
    {
        simulationIsRunning = false;
        allEndTilesAreValidated = false;
        turnIsFinished = true;
        turnStart = false;
        currentTurnTime = turnTime;
        turnCount = 0;
        currentTurn = 0;
        if (greenArrowsControllers.Length > 0)
        {
            foreach (GreenArrow greenArrow in greenArrowsControllers)
            {
                greenArrow.SetInitialState();
            }
        }

        player_GreenArrowsControllers = new Player_GreenArrow[player_GreenArrows.Length];
        for (int i = 0; i < player_GreenArrowsControllers.Length; i++)
        {
            player_GreenArrowsControllers[i] = player_GreenArrows[i].GetComponent<Player_GreenArrow>();
        }
        if (player_GreenArrowsControllers.Length > 0)
        {
            foreach (Player_GreenArrow playerGreenArrow in player_GreenArrowsControllers)
            {
                playerGreenArrow.SetInitialState();
            }
        }




        foreach (CubeController cube in cubesControllers)
        {
            cube.SetInitialState();
        }
        foreach (EndTile endTile in endTilesControllers)
        {
            endTile.SetInitialState();
        }

        boardManager.SetInitialState();
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
