using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public int cubeNumber;

    public Transform[,,] tilesBoard;
    public int xPos, yPos, zPos;

    public Vector3 currentTurnPos;
    public Vector3 lastMovement;
    public Vector3 predicted_NextTurnPos, predicted_NextTurnMovement;
    public Vector3 confirmed_NextTurnPos, confirmed_NextTurnMovement;
    

    // Used to store tiles positions and use them
    public Vector3 aboveAdjacentPos, belowAdjacentPos;        // adjacent positions
    public Vector3 frontAdjacentPos, backAdjacentPos, rightAdjacentPos, lefttAdjacentPos;
    public Vector3 aboveFront_AdjacentPos, aboveBack_AdjacentPos, aboveRight_AdjacentPos, aboveLeft_AdjacentPos;
    public Vector3 belowFront_AdjacentPos, belowBack_AdjacentPos, belowRight_AdjacentPos, belowLeft_AdjacentPos;

    public Vector3 aboveTwoTilesAwayPos, belowTwoTilesAwayPos;    // two tiles away position
    public Vector3 frontTwoTilesAwayPos, backTwoTilesAwayPos, rightTwoTilesAwayPos, lefttTwoTilesAwayPos;

    
    // Used to store Tiles and use them (don't forget to use GetComponent)
    public Transform aboveAdjacentTile, belowAdjacentTile;     // adjacent tiles
    public Transform frontAdjacentTile, backAdjacentTile, rightAdjacentTile, lefttAdjacentTile;
    public Transform aboveFront_AdjacentTile, aboveBack_AdjacentTile, aboveRight_AdjacentTile, aboveLeft_AdjacentTile;
    public Transform belowFront_AdjacentTile, belowBack_AdjacentTile, belowRight_AdjacentTile, belowLeft_AdjacentTile;

    public Transform aboveTwoTilesAway, belowTwoTilesAway;     // two tiles away tiles
    public Transform frontTwoTilesAway, backTwoTilesAway, rightTwoTilesAway, lefttTwoTilesAway;


    // Used to determine if the cube will move and where will it moves based on predicted futur position
    public bool willMove, willNotMove;
    public bool willMoveForward, willMoveBack, willMoveRight, willMoveLeft, willMoveUp, willMoveDown;

    public bool willRoundTrip, willNotRoundTrip;
    public bool willRoundTripForward, willRoundTripBack, willRoundTripRight, willRoundTripLeft, willRoundTripUp, willRoundTripDown;


    private void Start()
    {
        tilesBoard = BoardManager.original_3DBoard;
    }

    //TurnInitialazer must be run at the beginning of each turn to ensure that the cube position is correct
    public void TurnInitializer()
    {
        tilesBoard = BoardManager.updated_3DBoard;

        currentTurnPos = transform.position;
        xPos = (int)currentTurnPos.x;
        yPos = (int)currentTurnPos.y;
        zPos = (int)currentTurnPos.z;

        if (willNotMove)
        {
            willMove =
            willNotMove =
            willMoveForward =
            willMoveBack =
            willMoveRight =
            willMoveLeft =
            willMoveUp = 
            willMoveDown = false;
        }
        if (willNotRoundTrip)
        {
            willRoundTrip =
            willRoundTripForward =
            willRoundTripBack =
            willRoundTripRight =
            willRoundTripLeft =
            willRoundTripUp =
            willRoundTripDown = false;
        }

        aboveAdjacentPos = (currentTurnPos + new Vector3(0, 0, 0));
        belowAdjacentPos = (currentTurnPos + new Vector3(0, -1, 0));

        frontAdjacentPos = (currentTurnPos + new Vector3(0, 0, 1));
        backAdjacentPos = (currentTurnPos + new Vector3(0, 0, -1));
        rightAdjacentPos = (currentTurnPos + new Vector3(1, 0, 0));
        lefttAdjacentPos = (currentTurnPos + new Vector3(-1, 0, 0));

        aboveFront_AdjacentPos = (currentTurnPos + new Vector3(0, 1, 1));
        aboveBack_AdjacentPos = (currentTurnPos + new Vector3(0, 1, -1));
        aboveRight_AdjacentPos = (currentTurnPos + new Vector3(1, 0, 1));
        aboveLeft_AdjacentPos = (currentTurnPos + new Vector3(-1, 0, 1));

        aboveTwoTilesAwayPos = (currentTurnPos + new Vector3(0, 2, 0));    // two tiles away positions
        belowTwoTilesAwayPos = (currentTurnPos + new Vector3(0, -2, 0));

        frontTwoTilesAwayPos = (currentTurnPos + new Vector3(0, 0, 2));
        backTwoTilesAwayPos = (currentTurnPos + new Vector3(0, 0, -2));
        rightTwoTilesAwayPos = (currentTurnPos + new Vector3(2, 0, 0));
        lefttTwoTilesAwayPos = (currentTurnPos + new Vector3(-2, 0, 0));
    }

    //check the position of the tile relatively to the current cube position
    public Transform TileCheck(Vector3 tilePos)
    {
        Transform tile;

        if (tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z])
        {
            tile = tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z];
            Debug.Log("Tile Check -- is full");
            return tile;
        }
        else
        {
            Debug.Log("Tile Check -- is empty");
            return null;
        }
    }

    public Vector3 PredictNextTurnPos(Transform belowTile) // assign the value returned by TileCheck() to know whitch type of tile the cube has below it(or not)
    {
        // This function also checks if there is an obstacle at the predicted next turn position of the cube
        Vector3 predictedPos = Vector3.zero;


        if (belowTile.tag == "Forward Arrow")
        {
            if(!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = (currentTurnPos + new Vector3(0, 0, 1)); // determine the predicted next turn position of the cube
                lastMovement = new Vector3(0, 0, 1);    // store the movement so that it can be used if the cube is on a blank tile or another cube
                willMove = willMoveForward = true;
                willNotMove = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
            }
            if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z]) // if there is already something a the predicted position
            {
                predictedPos = currentTurnPos; // set the next turn position as the current so the cube doesn't move
                lastMovement = new Vector3(0, 0, 0);    // idem for the last movement
                willNotMove = true;
                willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
            }
        }
        else if (belowTile.tag == "Back Arrow")
        {
            if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = (currentTurnPos + new Vector3(0, 0, -1));
                lastMovement = new Vector3(0, 0, -1);
                willMove = willMoveBack = true;
                willNotMove = willMoveForward = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
            }
            if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = currentTurnPos;
                lastMovement = new Vector3(0, 0, 0);
                willNotMove = true;
                willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
            }
        }
        else if (belowTile.tag == "Right Arrow")
        {
            if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = (currentTurnPos + new Vector3(1, 0, 0));
                lastMovement = new Vector3(1, 0, 0);
                willMove = willMoveRight = true;
                willNotMove = willMoveForward = willMoveBack = willMoveLeft = willMoveUp = willMoveDown = false;
            }
            if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = currentTurnPos;
                lastMovement = new Vector3(0, 0, 0);
                willNotMove = true;
                willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
            }
        }
        else if (belowTile.tag == "Left Arrow")
        {
            if (!tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = (currentTurnPos + new Vector3(-1, 0, 0));
                lastMovement = new Vector3(-1, 0, 0);
                willMove = willMoveLeft = true;
                willNotMove = willMoveForward = willMoveBack = willMoveRight = willMoveUp = willMoveDown = false;
            }
            if (tilesBoard[(int)predictedPos.x, (int)predictedPos.y, (int)predictedPos.z])
            {
                predictedPos = currentTurnPos;
                lastMovement = new Vector3(0, 0, 0);
                willNotMove = true;
                willMove = willMoveForward = willMoveBack = willMoveRight = willMoveLeft = willMoveUp = willMoveDown = false;
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
                willNotMove = true;
            }
        }
        else if (!belowTile) //if the cube has no tile below it
        {
            predictedPos = (currentTurnPos + new Vector3(0, -1, 0));
        }

        //Vector3 is a value type (opposed to reference type) so it needs to be asigned to an extern Vector3 (here predicted_NextTurnPos) so it can be used outside of the function
        return predictedPos;
    }
}
