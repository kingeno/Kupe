using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] cubes;
    public CubeController[] cubesControllers;

    public GameObject[] endTiles;
    public EndTile[] endTilesControllers;

    public static int turnCount;
    public static int currentTurn;
    public static bool turnStart;
    public static bool turnIsFinished;

    public static float staticTurnTime = 0.45f;
    public static float turnTime;
    public float initialTurnTime;

    public static bool simulationIsRunning;

    public bool allEndTilesAreValidated;

    private void Start()
    {
        turnTime = staticTurnTime;
        simulationIsRunning = false;
        allEndTilesAreValidated = false;
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
        endTilesControllers = new EndTile[endTiles.Length];
        i = 0;
        foreach (GameObject endTile in endTiles)
        {
            endTilesControllers[i] = endTile.GetComponent<EndTile>();
            i++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!simulationIsRunning)
                simulationIsRunning = true;
            else
                simulationIsRunning = false;
        }
        if(simulationIsRunning)
            TurnTimer();
        if (!allEndTilesAreValidated)
        {
            if (!turnStart && turnIsFinished /*&& Input.GetKeyDown(KeyCode.Space)*/)
            {
                foreach (EndTile endTile in endTilesControllers)
                {
                    endTile.TurnInitializer();
                }
                CheckIfEndTurnTilesAreValidated();
                if (!allEndTilesAreValidated)
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
            }
        }
        else if (!allEndTilesAreValidated)
        {
            simulationIsRunning = false;
        }
    }

    public void LaunchSimulation()
    {
        if (!simulationIsRunning)
        {
            simulationIsRunning = true;
        }
        else
            simulationIsRunning = false;
    }

    public static void TurnTimer()
    {
        staticTurnTime -= Time.deltaTime;
        if (staticTurnTime <= 0.0f)
        {
            TimerEnded();
        }
    }

    public static void TimerEnded()
    {
        turnIsFinished = true;
        turnStart = false;
        staticTurnTime = turnTime;
    }

    public void CheckIfEndTurnTilesAreValidated()
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
                else if (i == endTilesControllers.Length && endTilesControllers[i].isValidated)
                {
                    allEndTilesAreValidated = true;
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
                    Debug.Log(i + " " + cubesControllers.Length);

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
