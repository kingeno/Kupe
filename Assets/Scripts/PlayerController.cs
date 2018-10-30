using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool hasReachedEndTile; // public because used in the GamManager

    private int tileNumber;
    public Vector3 startPos;

    private Vector3 moveForward;
    private Vector3 moveBack;
    private Vector3 moveLeft;
    private Vector3 moveRight;
    private Vector3 moveUp;
    private Vector3 moveDown;

    private Vector3 currentPos;

    private Vector3 lastNonBlankTileType;

    //------------------

    public bool canMoveForward;
    public bool canMoveBack;
    public bool canMoveRight;
    public bool canMoveLeft;

    public bool hasFinishItsTurn;
    public bool isOutOfBoardRange;

    private Quaternion facingForward;
    private Quaternion facingBack;
    private Quaternion facingRight;
    private Quaternion facingLeft;

    public Transform[,,] tilesBoard;

    private Transform frontTile;
    private Transform backTile;
    private Transform rightTile;
    private Transform leftTile;
    private Transform aboveTile;
    private Transform belowTile;

    public int xPos;
    public int yPos;
    public int zPos;

    public bool canPlayerMoveInLastNonBlankTileDirection;

    void Start()
    {
        hasFinishItsTurn = false;
        isOutOfBoardRange = false;
        hasReachedEndTile = false;

        startPos = transform.position;

        moveForward = new Vector3(0, 0, 1);
        moveBack = new Vector3(0, 0, -1);
        moveLeft = new Vector3(-1, 0, 0);
        moveRight = new Vector3(1, 0, 0);
        moveUp = new Vector3(0, 1, 0);
        moveDown = new Vector3(0, -1, 0);

        facingForward = Quaternion.Euler(0, 0, 0);
        facingBack = Quaternion.Euler(0, 180, 0);
        facingRight = Quaternion.Euler(0, 90, 0);
        facingLeft = Quaternion.Euler(0, 270, 0);

        tilesBoard = BoardManager.original_3DBoard;

        //CheckAdjacentTiles();
    }

    void Update()
    {
        if (GameManager.playerHasLaunchedSimulation /*&& !GameManager.simulationHasEnded*/)
        {
            tilesBoard = BoardManager.updated_3DBoard;
            if (!hasFinishItsTurn && !hasReachedEndTile/*GameManager.turnIsFinished*/ /*&& Input.GetKeyDown(KeyCode.Space)*/)
            {
                CheckAdjacentTiles();
                Debug.Log(name + " moves");
                switch (tileNumber)
                {
                    case 1: //Start Tile
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                            if (canMoveForward)
                            {
                                transform.position += moveForward;
                                lastNonBlankTileType = moveForward;
                                Debug.Log(name + " last non blank tile type: " + lastNonBlankTileType);
                            }
                            else if (!canMoveForward)
                            {
                                Debug.Log(name + " can't move forward");
                            }
                        }
                        break;
                    case 2: //End Tile
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                            hasReachedEndTile = true;
                        }
                        break;
                    case 3: //Blank Tile
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                            if (lastNonBlankTileType == moveForward && canMoveForward)
                            {
                                transform.position += lastNonBlankTileType;
                            }
                            else if (lastNonBlankTileType == moveBack && canMoveBack)
                            {
                                transform.position += lastNonBlankTileType;
                            }
                            else if (lastNonBlankTileType == moveRight && canMoveRight)
                            {
                                transform.position += lastNonBlankTileType;
                            }
                            else if (lastNonBlankTileType == moveLeft && canMoveLeft)
                            {
                                transform.position += lastNonBlankTileType;
                            }
                            else
                            {
                                Debug.Log(name + " can't move in the direction he is supposed to");
                            }
                            Debug.Log(name + " last non blank tile type: " + lastNonBlankTileType);
                        }
                        break;
                    case 4: //Hole Tile
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                        }
                        break;
                    case 5: //Forward Arrow
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                            try { OnGreenArrow(moveForward, canMoveForward, facingForward); }
                            catch { OnPOLGreenArrow(moveForward, canMoveForward, facingForward); }
                        }
                        break;
                    case 6: //Right Arrow
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                            try { OnGreenArrow(moveRight, canMoveRight, facingRight); }
                            catch { OnPOLGreenArrow(moveRight, canMoveRight, facingRight); }
                        }
                        break;
                    case 7: //Back Arrow
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                            try { OnGreenArrow(moveBack, canMoveBack, facingBack); }
                            catch { OnPOLGreenArrow(moveBack, canMoveBack, facingBack); }
                        }
                        break;
                    case 8: //Left Arrow
                        if (!hasReachedEndTile)
                        {
                            //CheckAdjacentTiles();
                            try { OnGreenArrow(moveLeft, canMoveLeft, facingLeft); }
                            catch { OnPOLGreenArrow(moveLeft, canMoveLeft, facingLeft); }
                        }
                        break;
                }
                hasFinishItsTurn = true;
                Debug.Log(name + " has finished moving");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Start Tile")
        {
            tileNumber = 1;
        }

        if (other.tag == "End Tile")
        {
            tileNumber = 2;
            Debug.Log(name + " is touching " + other.tag);
        }

        if (other.tag == "Blank Tile" || other.tag == "Player")
        {
            tileNumber = 3;
            Debug.Log(name + " is touching " + other.tag);
        }

        if (other.tag == "Hole Tile")
        {
            tileNumber = 4;
            Debug.Log(name + " is touching " + other.tag);
        }

        if (other.tag == "Forward Arrow")
        {
            tileNumber = 5;
            Debug.Log(name + " is touching " + other.tag);
        }

        if (other.tag == "Right Arrow")
        {
            tileNumber = 6;
            Debug.Log(name + " is touching " + other.tag);
        }

        if (other.tag == "Back Arrow")
        {
            tileNumber = 7;
            Debug.Log(name + " is touching " + other.tag);
        }

        if (other.tag == "Left Arrow")
        {
            tileNumber = 8;
            Debug.Log(name + " is touching " + other.tag);
        }
    }

    private void CheckAdjacentTiles()
    {
        currentPos = transform.position;
        xPos = (int)currentPos.x;
        yPos = (int)currentPos.y;
        zPos = (int)currentPos.z;

        try
        {
            frontTile = tilesBoard[xPos, yPos, zPos + 1];
            Debug.Log(name + "\nFRONT tile    name: " + frontTile.name + "        " + "front tile position = " + frontTile.position);
            canMoveForward = false;
        }
        catch { canMoveForward = true; }

        try
        {
            backTile = tilesBoard[xPos, yPos, zPos - 1];
            Debug.Log(name + "\nBACK tile    name: " + backTile.name + "        " + "position = " + backTile.position);
            canMoveBack = false;
        }
        catch { canMoveBack = true; }

        try
        {
            rightTile = tilesBoard[xPos + 1, yPos, zPos];
            Debug.Log(name + "\nRIGHT tile    name: " + rightTile.name + "        " + "position = " + rightTile.position);
            canMoveRight = false;
        }
        catch { canMoveRight = true; }

        try
        {
            leftTile = tilesBoard[xPos - 1, yPos, zPos];
            Debug.Log(name + "\nLEFT tile    name: " + leftTile.name + "        " + "position = " + leftTile.position);
            canMoveLeft = false;
        }
        catch { canMoveLeft = true; }

        try
        {
            if (tilesBoard[xPos, yPos + 1, zPos])
            {
                aboveTile = tilesBoard[xPos, yPos + 1, zPos];
                Debug.Log(name + "\nABOVE tile  -  name: " + aboveTile.name + "        " + "position = " + aboveTile.position);
                canMoveForward = false;
                canMoveBack = false;
                canMoveRight = false;
                canMoveLeft = false;
            }
        }
        catch
        { }

        try
        {
            if (tilesBoard[xPos, yPos - 1, zPos])
            {
                belowTile = tilesBoard[xPos, yPos - 1, zPos];
                Debug.Log(name + "\nBELOW tile    name: " + belowTile.name + "        " + "position = " + belowTile.position);
            }
            else
            {
                //Debug.Log("there is no tile under " + name);
                canMoveForward = false;
                canMoveBack = false;
                canMoveRight = false;
                canMoveLeft = false;
                transform.position += moveDown;
            }
        }
        catch
        {
            Debug.Log(name + " is out of board range");
            canMoveForward = false;
            canMoveBack = false;
            canMoveRight = false;
            canMoveLeft = false;
            isOutOfBoardRange = true;
        }
    }


    public void OnGreenArrow(Vector3 moveToDirection, bool canMoveToDirection, Quaternion facingDirection)
    {
        if (canMoveToDirection)
        {
            transform.rotation = facingDirection;
            if (canMoveToDirection)
            {
                transform.position += moveToDirection;
                lastNonBlankTileType = moveToDirection;
            }
        }
        else if (!canMoveToDirection)
            Debug.Log(name + " can't move to direction (active tile)");
        else
            Debug.Log(name + " can't move to direction (unactive tile)");
    }


    public void OnPOLGreenArrow(Vector3 moveToDirection, bool canMoveToDirection, Quaternion facingDirection)
    {
        if (canMoveToDirection)
        {
            transform.rotation = facingDirection;
            if (canMoveToDirection)
            {
                transform.position += moveToDirection;
                lastNonBlankTileType = moveToDirection;
            }
        }
        else if (!canMoveToDirection)
            Debug.Log(name + "can't move to direction (active tile)");
        else
            Debug.Log(name + " can't move to direction (unactive tile)");
    }
}
