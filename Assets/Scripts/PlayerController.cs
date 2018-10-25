using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool hasReachedEndTile; // public because used in the GamManager

    private int tileNumber;

    private GreenArrow greenArrowScript;
    private POL_GreenArrow pOL_greenArrowScript;

    private Vector3 moveForward;
    private Vector3 moveBack;
    private Vector3 moveLeft;
    private Vector3 moveRight;
    private Vector3 moveUp;
    private Vector3 moveDown;

    private Vector3 startPos;
    private Vector3 currentPos;

    private GameObject startTile;
    private Vector3 lastNonBlankTileType;

    //------------------

    public bool isOnStartTile;
    public bool canMoveForward;
    public bool canMoveBack;
    public bool canMoveRight;
    public bool canMoveLeft;

    public bool hasFinishItsTurn;
    public bool isOutOfTheBoard;

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

    void Start()
    {
        hasFinishItsTurn = false;
        isOutOfTheBoard = false;
        tileNumber = 1;
        hasReachedEndTile = false;

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

        //if (startTile == null)
        //{
        //    startTile = GameObject.FindGameObjectWithTag("Start Tile");
        //    startPos += (startTile.transform.position + new Vector3(0, 1, 0));
        //    transform.position = startPos;
        //}
        //else
        //{ Debug.LogError("Does not find start tile in scene"); }
        CheckAdjacentTiles();

        isOnStartTile = true;
    }

    void Update()
    {
        if (GameManager.playerHasLaunchedSimulation /*&& !GameManager.simulationHasEnded*/)
        {
            tilesBoard = BoardManager.updated_3DBoard;
            if (!hasFinishItsTurn /*GameManager.turnIsFinished*/ /*&& Input.GetKeyDown(KeyCode.Space)*/)
            {
                Debug.Log(name + " moves");
                switch (tileNumber)
                {
                    case 1: //Start Tile
                        if (!hasReachedEndTile)
                        {
                            CheckAdjacentTiles();
                            if (canMoveForward)
                            {
                                transform.position += moveForward;
                                lastNonBlankTileType = moveForward;
                                Debug.Log(name + " last non blank tile type: " + lastNonBlankTileType);
                                isOnStartTile = false;
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
                            CheckAdjacentTiles();
                            hasReachedEndTile = true;
                        }
                        break;
                    case 3: //Blank Tile
                        if (!hasReachedEndTile)
                        {
                            CheckAdjacentTiles();
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
                            CheckAdjacentTiles();
                        }
                        break;
                    case 5: //Forward Arrow
                        if (!hasReachedEndTile)
                        {
                            CheckAdjacentTiles();
                            try { OnGreenArrow(moveForward, canMoveForward, facingForward); }
                            catch { OnPOLGreenArrow(moveForward, canMoveForward, facingForward); }
                        }
                        break;
                    case 6: //Right Arrow
                        if (!hasReachedEndTile)
                        {
                            CheckAdjacentTiles();
                            try { OnGreenArrow(moveRight, canMoveRight, facingRight); }
                            catch { OnPOLGreenArrow(moveRight, canMoveRight, facingRight); }
                        }
                        break;
                    case 7: //Back Arrow
                        if (!hasReachedEndTile)
                        {
                            CheckAdjacentTiles();
                            try { OnGreenArrow(moveBack, canMoveBack, facingBack); }
                            catch { OnPOLGreenArrow(moveBack, canMoveBack, facingBack); }
                        }
                        break;
                    case 8: //Left Arrow
                        if (!hasReachedEndTile)
                        {
                            CheckAdjacentTiles();
                            try { OnGreenArrow(moveLeft, canMoveLeft, facingLeft); }
                            catch { OnPOLGreenArrow(moveLeft, canMoveLeft, facingLeft); }
                        }
                        break;
                }
                hasFinishItsTurn = true;
                //tilesBoard = BoardManager.updated_3DBoard;
                Debug.Log(name + " has finished moving");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Start Tile")
            tileNumber = 1;

        if (other.tag == "End Tile")
            tileNumber = 2;

        if (other.tag == "Blank Tile" || other.tag == "Player")
            tileNumber = 3;

        if (other.tag == "Hole Tile")
            tileNumber = 4;

        if (other.tag == "Forward Arrow")
        {
            tileNumber = 5;
            try { greenArrowScript = other.GetComponent<GreenArrow>(); transform.rotation = facingForward; }
            catch { }
            try { pOL_greenArrowScript = other.GetComponent<POL_GreenArrow>(); transform.rotation = facingForward; }
            catch { }
        }

        if (other.tag == "Right Arrow")
        {
            tileNumber = 6;
            try { greenArrowScript = other.GetComponent<GreenArrow>(); transform.rotation = facingRight; }
            catch { }
            try { pOL_greenArrowScript = other.GetComponent<POL_GreenArrow>(); transform.rotation = facingRight; }
            catch { }
        }

        if (other.tag == "Back Arrow")
        {
            tileNumber = 7;
            try { greenArrowScript = other.GetComponent<GreenArrow>(); transform.rotation = facingBack; }
            catch { }
            try { pOL_greenArrowScript = other.GetComponent<POL_GreenArrow>(); transform.rotation = facingBack; }
            catch { }
        }

        if (other.tag == "Left Arrow")
        {
            tileNumber = 8;
            try { greenArrowScript = other.GetComponent<GreenArrow>(); transform.rotation = facingLeft; }
            catch { }
            try { pOL_greenArrowScript = other.GetComponent<POL_GreenArrow>(); transform.rotation = facingLeft; }
            catch { }
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
                Debug.Log("there is no tile under " + name);
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
            isOutOfTheBoard = true;
        }
    }


    public void OnGreenArrow(Vector3 moveToDirection, bool canMoveToDirection, Quaternion facingDirection)
    {
        if (canMoveToDirection && greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += moveToDirection;
            lastNonBlankTileType = moveToDirection;
            greenArrowScript.StateSwitch();
        }
        else if (canMoveToDirection && !greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += lastNonBlankTileType;
            greenArrowScript.StateSwitch();
        }
        //else if (!canMoveToDirection)
        //{
        //    Debug.Log(name + " can't move");
        //}
    }


    public void OnPOLGreenArrow(Vector3 moveToDirection, bool canMoveToDirection, Quaternion facingDirection)
    {
        if (canMoveToDirection && pOL_greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += moveToDirection;
            lastNonBlankTileType = moveToDirection;
            pOL_greenArrowScript.StateSwitch();
        }
        else if (canMoveToDirection && !pOL_greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += lastNonBlankTileType;
            pOL_greenArrowScript.StateSwitch();
        }
        else if (!canMoveToDirection)
        {
            Debug.Log(name + " can't move");
        }
    }
}
