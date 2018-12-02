﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public int cubeNumber;

    public Transform[,,] tilesBoard;
    public int xPos, yPos, zPos;

    public Vector3 startPos;

    [HideInInspector] public Vector3 currentTurnPos;
    [HideInInspector] public Vector3 lastMovement;
    [HideInInspector] public Vector3 predicted_NextTurnPos, predicted_NextTurnMovement;
    [HideInInspector] public Vector3 confirmed_NextTurnPos, confirmed_NextTurnMovement;


    // Used to store tiles positions and use them
    public Vector3 above_AdjacentPos, below_AdjacentPos;        // adjacent positions
    public Vector3 front_AdjacentPos, back_AdjacentPos, right_AdjacentPos, left_AdjacentPos;
    public Vector3 frontRight_DiagonalPos, frontLeft_DiagonalPos, backRight_DiagonalPos, backLeft_DiagonalPos;
    public Vector3 aboveFront_AdjacentPos, aboveBack_AdjacentPos, aboveRight_AdjacentPos, aboveLeft_AdjacentPos;
    public Vector3 belowFront_AdjacentPos, belowBack_AdjacentPos, belowRight_AdjacentPos, belowLeft_AdjacentPos;

    public Vector3 above_TwoTilesAwayPos, below_TwoTilesAwayPos;    // two tiles away positions
    public Vector3 front_TwoTilesAwayPos, back_TwoTilesAwayPos, right_TwoTilesAwayPos, left_TwoTilesAwayPos;


    // Used to store Tiles and use them (don't forget to use GetComponent)
    // public Transform above_AdjacentTile, below_AdjacentTile;
    // public Transform front_AdjacentTile, back_AdjacentTile, right_AdjacentTile, left_AdjacentTile;
    // public Transform front_Right_DiagonalTile, frontLeft_DiagonalTile, backRight_DiagonalTile, backLeft_DiagonalTile;
    // public Transform aboveFront_AdjacentTile, aboveBack_AdjacentTile, aboveRight_AdjacentTile, aboveLeft_AdjacentTile;
    // public Transform belowFront_AdjacentTile, belowBack_AdjacentTile, belowRight_AdjacentTile, belowLeft_AdjacentTile;

    // public Transform above_TwoTilesAway, below_TwoTilesAway;
    // public Transform front_TwoTilesAway, back_TwoTilesAway, right_TwoTilesAway, left_TwoTilesAway;


    // Used to determine if the cube will move and where will it moves based on predicted futur position
    public bool willMove;
    public bool willMoveForward, willMoveBack, willMoveRight, willMoveLeft, willMoveUp, willMoveDown;

    public bool willRoundTrip;
    public bool willRoundTripForward, willRoundTripBack, willRoundTripRight, willRoundTripLeft, willRoundTripUp, willRoundTripDown;

    public bool[] movementBoolArray;

    public Animator cubeAnimator;

    public Color _opaqueCubeColor;
    public Color _transparentCubeColor;
    // public Animation forward_animation, back_Animation, right_Animation, left_Animation, moveDown_Animation, moveUp_Animation;
    // public Animation forward_RoundTripAnimation, back_RoundTripAnimation, right_RoundTripAnimation, left_RoundTripAnimation;
    // public Animation notMoving_Animation;

    public GameObject cubeAvatar;

    private void Start()
    {
        tilesBoard = BoardManager.original_3DBoard;
        _opaqueCubeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        _transparentCubeColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        startPos = transform.position;

        movementBoolArray = new bool[] {
            willMove, willRoundTrip,
            willMoveForward, willMoveBack, willMoveRight, willMoveLeft, willMoveUp, willMoveDown,
            willRoundTripForward, willRoundTripBack, willRoundTripRight, willRoundTripLeft, willRoundTripUp, willRoundTripDown };
    }

    private void Update()
    {
        if (GameManager.simulationIsRunning)
        {
            cubeAvatar.GetComponent<MeshRenderer>().material.color = _opaqueCubeColor;
        }
        else
            cubeAvatar.GetComponent<MeshRenderer>().material.color = _transparentCubeColor;
    }

    public void SetInitialState()
    {
        willMove =
        willMoveForward =
        willMoveBack =
        willMoveRight =
        willMoveLeft =
        willMoveUp =
        willMoveDown = false;

        willRoundTrip =
        willRoundTripForward =
        willRoundTripBack =
        willRoundTripRight =
        willRoundTripLeft =
        willRoundTripUp =
        willRoundTripDown = false;

        transform.position = startPos;
        tilesBoard = BoardManager.original_3DBoard;
    }

    //TurnInitialazer must be run at the beginning of each turn to ensure that the cube position is correct
    public void TurnInitializer()
    {
        //Debug.Log("rune " + name + " Initializer.");
        tilesBoard = BoardManager.updated_3DBoard;

        currentTurnPos = transform.position;
        xPos = (int)currentTurnPos.x;
        yPos = (int)currentTurnPos.y;
        zPos = (int)currentTurnPos.z;

        if (!willMove)
        {
            willMoveForward =
            willMoveBack =
            willMoveRight =
            willMoveLeft =
            willMoveUp =
            willMoveDown = false;
        }

        if (!willRoundTrip)
        {
            willRoundTripForward =
            willRoundTripBack =
            willRoundTripRight =
            willRoundTripLeft =
            willRoundTripUp =
            willRoundTripDown = false;
        }


        //Debug.Log(name + " willMove=" + willMove + ";   willRoundTrip=" + willRoundTrip);

        above_AdjacentPos = (currentTurnPos + new Vector3(0, 1, 0));
        below_AdjacentPos = (currentTurnPos + new Vector3(0, -1, 0));

        front_AdjacentPos = (currentTurnPos + new Vector3(0, 0, 1));
        back_AdjacentPos = (currentTurnPos + new Vector3(0, 0, -1));
        right_AdjacentPos = (currentTurnPos + new Vector3(1, 0, 0));
        left_AdjacentPos = (currentTurnPos + new Vector3(-1, 0, 0));

        frontRight_DiagonalPos = (currentTurnPos + new Vector3(1, 0, 1));
        frontLeft_DiagonalPos = (currentTurnPos + new Vector3(-1, 0, 1));
        backRight_DiagonalPos = (currentTurnPos + new Vector3(1, 0, -1));
        backLeft_DiagonalPos = (currentTurnPos + new Vector3(-1, 0, -1));

        aboveFront_AdjacentPos = (currentTurnPos + new Vector3(0, 1, 1));
        aboveBack_AdjacentPos = (currentTurnPos + new Vector3(0, 1, -1));
        aboveRight_AdjacentPos = (currentTurnPos + new Vector3(1, 1, 0));
        aboveLeft_AdjacentPos = (currentTurnPos + new Vector3(-1, 1, 0));

        // two tiles away positions
        above_TwoTilesAwayPos = (currentTurnPos + new Vector3(0, 2, 0));
        below_TwoTilesAwayPos = (currentTurnPos + new Vector3(0, -2, 0));

        front_TwoTilesAwayPos = (currentTurnPos + new Vector3(0, 0, 2));
        back_TwoTilesAwayPos = (currentTurnPos + new Vector3(0, 0, -2));
        right_TwoTilesAwayPos = (currentTurnPos + new Vector3(2, 0, 0));
        left_TwoTilesAwayPos = (currentTurnPos + new Vector3(-2, 0, 0));
    }

    //check the position of the tile relatively to the current cube position
    public Transform TileCheck(Vector3 tilePos)
    {
        //Debug.Log("run " + name + " Tile Check");
        Transform tile;
        if (tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z])
        {
            tile = tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z];
            return tile;
        }
        else
        {
            return null;
        }
    }

    public Vector3 PredictNextTurnPos(Transform belowTile) // assign the value returned by TileCheck() to know whitch type of tile the cube has below it(or not)
    {
        // This function also checks if there is an obstacle at the predicted next turn position of the cube
        //Debug.Log("Function()   " + name + " Predict Next Turn Pos");
        Vector3 predictedPos = Vector3.zero;

        if (!belowTile) //if the cube has no tile below it
        {
            Debug.Log(name + " has no tile below it");
            predictedPos = (currentTurnPos + new Vector3(0, -1, 0));
            willMove = willMoveDown = true;
        }
        else if (belowTile.tag == "Green Arrow" && belowTile.GetComponent<GreenArrow>())
        {
            if (belowTile.GetComponent<GreenArrow>().tileOrientation == "Forward")
            {
                predictedPos = (currentTurnPos + new Vector3(0, 0, 1)); // determine the predicted next turn position of the cube
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(0, 0, 1);    // store the movement so that it can be used if the cube is on a blank tile or another cube
                    willMove = willMoveForward = true;
                    willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z]) // if there is already something a the predicted position
                {
                    predictedPos = currentTurnPos; // set the next turn position as the current so the cube doesn't move
                    lastMovement = new Vector3(0, 0, 0);    // idem for the last movement
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
            else if (belowTile.GetComponent<GreenArrow>().tileOrientation == "Back")
            {
                predictedPos = (currentTurnPos + new Vector3(0, 0, -1));
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(0, 0, -1);
                    willMove = willMoveBack = true;
                    willMoveForward = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    predictedPos = currentTurnPos;
                    lastMovement = new Vector3(0, 0, 0);
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
            else if (belowTile.GetComponent<GreenArrow>().tileOrientation == "Right")
            {
                predictedPos = (currentTurnPos + new Vector3(1, 0, 0));
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(1, 0, 0);
                    willMove = willMoveRight = true;
                    willMoveForward = willMoveBack = willMoveLeft = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    predictedPos = currentTurnPos;
                    lastMovement = new Vector3(0, 0, 0);
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
            else if (belowTile.GetComponent<GreenArrow>().tileOrientation == "Left")
            {
                predictedPos = (currentTurnPos + new Vector3(-1, 0, 0));
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(-1, 0, 0);
                    willMove = willMoveLeft = true;
                    willMoveForward = willMoveBack = willMoveRight = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    predictedPos = currentTurnPos;
                    lastMovement = new Vector3(0, 0, 0);
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
        }
        else if (belowTile.tag == "Player Green Arrow" && belowTile.GetComponent<Player_GreenArrow>())
        {
            if (belowTile.GetComponent<Player_GreenArrow>().tileOrientation == "Forward")
            {
                predictedPos = (currentTurnPos + new Vector3(0, 0, 1)); // determine the predicted next turn position of the cube
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(0, 0, 1);    // store the movement so that it can be used if the cube is on a blank tile or another cube
                    willMove = willMoveForward = true;
                    willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z]) // if there is already something a the predicted position
                {
                    predictedPos = currentTurnPos; // set the next turn position as the current so the cube doesn't move
                    lastMovement = new Vector3(0, 0, 0);    // idem for the last movement
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
            else if (belowTile.GetComponent<Player_GreenArrow>().tileOrientation == "Back")
            {
                predictedPos = (currentTurnPos + new Vector3(0, 0, -1));
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(0, 0, -1);
                    willMove = willMoveBack = true;
                    willMoveForward = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    predictedPos = currentTurnPos;
                    lastMovement = new Vector3(0, 0, 0);
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
            else if (belowTile.GetComponent<Player_GreenArrow>().tileOrientation == "Right")
            {
                predictedPos = (currentTurnPos + new Vector3(1, 0, 0));
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(1, 0, 0);
                    willMove = willMoveRight = true;
                    willMoveForward = willMoveBack = willMoveLeft = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    predictedPos = currentTurnPos;
                    lastMovement = new Vector3(0, 0, 0);
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
            else if (belowTile.GetComponent<Player_GreenArrow>().tileOrientation == "Left")
            {
                predictedPos = (currentTurnPos + new Vector3(-1, 0, 0));
                if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    lastMovement = new Vector3(-1, 0, 0);
                    willMove = willMoveLeft = true;
                    willMoveForward = willMoveBack = willMoveRight = willMoveUp = willMoveDown = false;
                }
                else if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
                {
                    predictedPos = currentTurnPos;
                    lastMovement = new Vector3(0, 0, 0);
                    willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
                }
            }
        }
        else if (belowTile.tag == "Blank Tile" || belowTile.tag == "Cube")
        {
            predictedPos = (currentTurnPos + lastMovement);
            willMove = true;
            if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = currentTurnPos;
                lastMovement = new Vector3(0, 0, 0);
                willMove = false;
            }
        }

        else if (belowTile.tag == "End Tile")
        {
            predictedPos = currentTurnPos;
            willMove = willRoundTrip = false;
            lastMovement = new Vector3(0, 0, 0);
            //if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            //{
            //    predictedPos = currentTurnPos;
            //    lastMovement = new Vector3(0, 0, 0);
            //    willMove = false;
            //}
        }

        //Vector3 is a value type (opposed to reference type) so it needs to be asigned to an extern Vector3 (here predicted_NextTurnPos) so it can be used outside of the function 
        //--> It seems that it doesn't need to, maybe because the function is called in antoher function (Update())
        //Debug.Log(name + " predicted pos= " + predictedPos);
        return predictedPos;
    }

    public void AnimationBooleanChecker()
    {
        for (int i = 0; i < movementBoolArray.Length; i++)
        {
            if (movementBoolArray[i])
            {
                Debug.Log(name + " will do " + movementBoolArray[i].ToString());
                break;
            }
            else if (!movementBoolArray[i] && i == movementBoolArray.Length)
            {
                Debug.LogError(name + " has no movement set. It will be considered as not moving.");
                willMove = false;
                break;
            }
            else
            {
                continue;
            }
        }
    }

    public void TriggerAnimation()
    {
        //Debug.Log("run " + name + " Trigger Animation.");
        if (!willMove && !willRoundTrip)
        {
            //Debug.Log(name + " will NOT move");
            // no animation needed -> maybe some effects or a blink of the cube
            transform.position = currentTurnPos;
        }
        else if (willMove)
        {
            //Debug.Log(name + " will move coroutine");
            StartCoroutine(MoveOverSeconds(this.gameObject, confirmed_NextTurnPos, 0.2f));
        }
        else if (willRoundTrip)
        {
            StartCoroutine(RoundTripOverSeconds(this.gameObject, predicted_NextTurnPos, 0.20f));
        }
    }


    IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 endPos, float seconds)     // original code : https://answers.unity.com/questions/296347/move-transform-to-target-in-x-seconds.html
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPos, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            if (!GameManager.simulationIsRunning)
                break;
            yield return null;
        }
        if (GameManager.simulationIsRunning)
            objectToMove.transform.position = endPos;
        else
            transform.position = startPos;
    }

    IEnumerator RoundTripOverSeconds(GameObject objectToMove, Vector3 halfWayPos, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds && GameManager.simulationIsRunning)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, halfWayPos, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            if (!GameManager.simulationIsRunning)
                break;
            yield return null;
        }
        elapsedTime = 0;
        halfWayPos = objectToMove.transform.position;
        while (elapsedTime < seconds && GameManager.simulationIsRunning)
        {
            objectToMove.transform.position = Vector3.Lerp(halfWayPos, startingPos, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            if (!GameManager.simulationIsRunning)
                break;
            yield return null;
        }
        if (GameManager.simulationIsRunning)
            objectToMove.transform.position = startingPos;
        else
            transform.position = startPos;
    }
}