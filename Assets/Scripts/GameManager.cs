using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MainCamera _mainCamera;

    public static bool levelIsStopped;
    public static bool gameIsPaused;
    private BoardManager boardManager;

    public GameObject[] cubes;
    public CubeController[] cubesControllers;

    public GameObject[] endTiles;
    public EndTile[] endTilesControllers;

    public GameObject[] greenArrows;
    public GreenArrow[] greenArrowsControllers;

    public GameObject[] player_GreenArrows;
    public Player_GreenArrow[] player_GreenArrowsControllers;

    public GameObject[] liftTiles;
    public LiftTile[] liftTilesControllers;

    public GameObject[] ephemereTiles;
    public EphemereTile[] ephemereTilesControllers;

    public static int turnCount;
    public static int currentTurn;
    public static bool turnStart;
    public static bool turnIsFinished;

    public float currentTurnTime;
    public float turnTime = 0.4f;
    public float initialTurnTime;

    public static bool simulationIsRunning;
    public static bool playerCanModifyBoard;
    public static bool simulationHasBeenLaunched;

    public bool allEndTilesAreValidated;
    public static bool levelIsCompleted;
    public bool playerIsStuck;

    public float simulationSpeed;

    private void Start()
    {
        simulationSpeed = 1;
        Time.timeScale = simulationSpeed;

        currentTurnTime = turnTime;
        levelIsCompleted = false;
        simulationIsRunning = false;
        allEndTilesAreValidated = false;
        playerCanModifyBoard = true;
        turnCount = 0;

        if (!boardManager)
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

        liftTiles = GameObject.FindGameObjectsWithTag("LiftTile");
        liftTilesControllers = new LiftTile[liftTiles.Length];
        for (int i = 0; i < liftTilesControllers.Length; i++)
        {
            liftTilesControllers[i] = liftTiles[i].GetComponent<LiftTile>();
        }

        ephemereTiles = GameObject.FindGameObjectsWithTag("EphemereTile");
        ephemereTilesControllers = new EphemereTile[ephemereTiles.Length];
        for (int i = 0; i < ephemereTilesControllers.Length; i++)
        {
            ephemereTilesControllers[i] = ephemereTiles[i].GetComponent<EphemereTile>();
        }
    }

    private void Update()
    {
        //Time.timeScale = 0.1f;

        if (Input.GetKeyDown(KeyCode.S))
            Debug.LogError(simulationSpeed);

        if (InGameUIManager.changeSpeedSimulation)
        {
            Time.timeScale = simulationSpeed;
            InGameUIManager.changeSpeedSimulation = false;
        }

        if (!_mainCamera)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchSimulation();
        }

        if (simulationIsRunning)
            TurnTimer();

        if (allEndTilesAreValidated)
        {
            foreach (CubeController cube in cubesControllers)
            {
                cube.isOnEndTile = true;
            }
        }

        if (simulationIsRunning && !allEndTilesAreValidated)
        {
            if (!turnStart && turnIsFinished)
            {
                foreach (EndTile endTile in endTilesControllers)
                {
                    endTile.TurnInitializer();
                }

                CheckIfEndTilesAreValidated();

                if (simulationIsRunning && !allEndTilesAreValidated)
                {
                    foreach (CubeController cube in cubesControllers)
                    {
                        cube.TurnInitializer();
                    }

                    CheckIfCubeIsStuck();
                    CheckIfCubeIsOnEndTile();

                    PredictCubesNextTurnPos();
                    CompareCubesPredictedNextTurnPositions();
                    AnimateCube();
                    turnStart = true;

                    foreach (GreenArrow greenArrow in greenArrowsControllers)
                    {
                        greenArrow.TurnInitializer();
                    }
                    foreach (Player_GreenArrow playerGreenArrow in player_GreenArrowsControllers)
                    {
                        playerGreenArrow.TurnInitializer();
                    }
                    foreach(LiftTile liftTile in liftTilesControllers)
                    {
                        liftTile.TurnInitializer();
                    }
                    foreach (EphemereTile ephemereTile in ephemereTilesControllers)
                    {
                        ephemereTile.TurnInitializer();
                    }
                    currentTurn++;
                    Debug.LogWarning("turn: " + currentTurn);
                }
            }
            boardManager.TurnInitializer();
        }
    }

    public void LaunchSimulation()
    {
        if (!simulationIsRunning)
        {
            simulationHasBeenLaunched = true;
            simulationIsRunning = true;

            player_GreenArrows = GameObject.FindGameObjectsWithTag("Player Green Arrow");
            player_GreenArrowsControllers = new Player_GreenArrow[player_GreenArrows.Length];
            for (int i = 0; i < player_GreenArrowsControllers.Length; i++)
            {
                player_GreenArrowsControllers[i] = player_GreenArrows[i].GetComponent<Player_GreenArrow>();
            }
        }
        else
        {
            simulationIsRunning = false;
        }
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
                    levelIsCompleted = true;
                    Time.timeScale = simulationSpeed = 1f;
                    break;
                }
            }
        }
        else if (endTilesControllers.Length == 1)
        {
            foreach (EndTile endTile in endTilesControllers)
            {
                if (endTile.isValidated)
                {
                    Time.timeScale = simulationSpeed = 1f;
                    allEndTilesAreValidated = true;
                    levelIsCompleted = true;
                    simulationIsRunning = false;
                }
                else
                    allEndTilesAreValidated = false;
            }
        }
    }

    public void CheckIfCubeIsStuck()
    {
        foreach (CubeController cube in cubesControllers)
        {
            if (cube.willNotMoveAnymore)
            {
                cube.willMove = false;
                cube.cubeAnimator.SetBool("will move", false);
                cube.cubeAnimator.SetBool("move forward", false);
                cube.cubeAnimator.SetBool("move back", false);
                cube.cubeAnimator.SetBool("move right", false);
                cube.cubeAnimator.SetBool("move left", false);
                if (cube.yPos != 0f && cube.TileCheck(cube.below_AdjacentPos).gameObject.tag != "End Tile")
                {
                    cube.TriggerStuckParticleStytem();
                }
                else if (cube.yPos == 0f)
                    cube.TriggerStuckParticleStytem();
            }
        }
    }

    public void CheckIfCubeIsOnEndTile()
    {
        foreach (CubeController cube in cubesControllers)
        {
            if (cube.willNotMoveAnymore)
            {
                if (cube.yPos != 0f && cube.TileCheck(cube.below_AdjacentPos).gameObject.tag == "End Tile")
                {
                    cube.TriggerIsOnEndTileParticleStytem();
                }
            }
        }
    }

    public void PredictCubesNextTurnPos()
    {
        foreach (CubeController cube in cubesControllers)
        {
            if (!cube.willNotMoveAnymore)
            {
                if (cube.yPos == 0)
                {
                    cube.willNotMoveAnymore = true;
                }
                else if (cube.yPos != 0)
                {
                    cube.predicted_NextTurnPos = cube.PredictNextTurnPos(cube.TileCheck(cube.below_AdjacentPos));

                    if (cube.predicted_NextTurnPos == cube.currentTurnPos)
                    {
                        cube.willNotMoveAnymore = true;
                    }
                }
            }
        }
    }

    public void CompareCubesPredictedNextTurnPositions()
    {
        foreach (CubeController cube in cubesControllers)
        {
            Debug.Log(cube.name + " predicted_NextTurnPos= " + cube.predicted_NextTurnPos);

            if (cube.willMove && cubesControllers.Length > 1) // (1)
            {
                for (int i = 0; i < cubesControllers.Length; i++)
                {
                    // Debug.Log(i + " " + cubesControllers.Length);

                    if (cube.willMove && cube.cubeNumber != cubesControllers[i].cubeNumber)
                    {
                        if (cube.predicted_NextTurnPos != cubesControllers[i].predicted_NextTurnPos)
                        {
                            // cube will do the predicted movement --> meaning that it will eather move or not but will not do a round-trip
                            cube.willRoundTrip = false;
                            cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
                            // if (i == cubesControllers.Length - 1)
                            Debug.Log(cube.name + " do the predicted movement.");
                            continue;
                        }
                        else if (cube.predicted_NextTurnPos == cubesControllers[i].predicted_NextTurnPos) // (2)
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
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.aboveLeft_AdjacentPos);   // above left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.frontLeft_DiagonalPos);   // front left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willMove = cube.willMoveLeft = false;
                                    cube.willRoundTrip = cube.willRoundTripLeft = false;
                                    cube.lastMovement = Vector3.zero;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.backLeft_DiagonalPos);    // back left
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
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
                            if (cube.willMoveDown)
                            {
                                Transform tile = cube.TileCheck(cube.belowFront_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.belowBack_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.belowRight_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.belowLeft_AdjacentPos);
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willRoundTrip = cube.willRoundTripDown = false;
                                    cube.willMove = cube.willMoveDown = true;
                                    cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
                                    tile = null;
                                    break;
                                }
                                tile = cube.TileCheck(cube.below_TwoTilesAwayPos);     // below two tiles away
                                if (tile && tile.tag == "Cube" && tile.GetComponent<CubeController>().cubeNumber == cubesControllers[i].cubeNumber)
                                {
                                    cube.willRoundTrip = cube.willRoundTripDown = true;
                                    cube.willMove = cube.willMoveDown = false;
                                    cube.confirmed_NextTurnPos = cube.predicted_NextTurnPos;
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
                // if (i == cubesControllers.Length - 1)
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

    public void StopSimulation()
    {       
        if (simulationIsRunning && simulationHasBeenLaunched)
        {
            levelIsStopped = true;
            simulationIsRunning = false;
            _mainCamera.backgroundColorSwap();
        }
        playerIsStuck = false;
        simulationHasBeenLaunched = false;
        allEndTilesAreValidated = false;
        turnIsFinished = true;
        turnStart = false;
        currentTurnTime = turnTime;
        turnCount = 0;
        currentTurn = 0;

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

        if (greenArrowsControllers.Length > 0)
        {
            foreach (GreenArrow greenArrow in greenArrowsControllers)
            {
                greenArrow.SetInitialState();
            }
        }

        if (liftTilesControllers.Length > 0)
        {
            foreach (LiftTile liftTile in liftTilesControllers)
            {
                liftTile.SetInitialState();
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
        foreach (EphemereTile ephemereTile in ephemereTilesControllers)
        {
            ephemereTile.SetInitialState();
        }

        levelIsCompleted = false;
        playerCanModifyBoard = true;
        _mainCamera.levelCompletedColorSwap = false;

        boardManager.SetInitialState();
        //_mainCamera.backgroundColorSwap();
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
