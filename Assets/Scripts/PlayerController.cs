using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool hasReachedEndTile; // public because used in the GamManager
    public Transform playerModel;
    public Animator playerModelAnimator;

    private int tileNumber;

    private GreenArrow greenArrowScript;
    private POL_GreenArrow pOL_greenArrowScript;

    private Vector3 moveForward;
    private Vector3 moveBack;
    private Vector3 moveLeft;
    private Vector3 moveRight;

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

        GameManager.turnCount = 0;
        tileNumber = 1;
        hasReachedEndTile = false;

        moveForward = new Vector3(0, 0, 1);
        moveBack = new Vector3(0, 0, -1);
        moveLeft = new Vector3(-1, 0, 0);
        moveRight = new Vector3(1, 0, 0);

        facingForward = Quaternion.Euler(0, 0, 0);
        facingBack = Quaternion.Euler(0, 180, 0);
        facingRight = Quaternion.Euler(0, 90, 0);
        facingLeft = Quaternion.Euler(0, 270, 0);

        tilesBoard = BoardManager.original_3DBoard;
        if (tilesBoard != null)
            Debug.Log(tilesBoard.Length);

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
        if (GameManager.playerHasLaunchedSimulation)
        {
            GameManager.TurnTimer();
            if (GameManager.turnIsFinished /*&& Input.GetKeyDown(KeyCode.Space)*/)
            {
                switch (tileNumber)
                {
                    case 1: //Start Tile
                        if (!hasReachedEndTile)
                        {
                            if (canMoveForward)
                            {
                                transform.position += moveForward;
                                lastNonBlankTileType = moveForward;
                                GameManager.turnCount += 1;
                                Debug.Log("turn = " + GameManager.turnCount);
                                isOnStartTile = false;
                            }
                            else if (!canMoveForward)
                            {
                                Debug.Log("The player can't move forward");
                            }
                            CheckAdjacentTiles();
                        }
                        break;
                    case 2: //End Tile
                        if (!hasReachedEndTile)
                        {
                            hasReachedEndTile = true;
                            CheckAdjacentTiles();
                        }
                        break;
                    case 3: //Blank Tile
                        if (!hasReachedEndTile)
                        {
                            if (lastNonBlankTileType == moveForward && canMoveForward)
                            {
                                transform.position += lastNonBlankTileType;
                                GameManager.turnCount += 1;
                                Debug.Log("turn = " + GameManager.turnCount);
                            }
                            else if (lastNonBlankTileType == moveBack && canMoveBack)
                            {
                                transform.position += lastNonBlankTileType;
                                GameManager.turnCount += 1;
                                Debug.Log("turn = " + GameManager.turnCount);
                            }
                            else if (lastNonBlankTileType == moveRight && canMoveRight)
                            {
                                transform.position += lastNonBlankTileType;
                                GameManager.turnCount += 1;
                                Debug.Log("turn = " + GameManager.turnCount);
                            }
                            else if (lastNonBlankTileType == moveLeft && canMoveLeft)
                            {
                                transform.position += lastNonBlankTileType;
                                GameManager.turnCount += 1;
                                Debug.Log("turn = " + GameManager.turnCount);
                            }
                            else
                            {
                                Debug.Log(name + " can't move in the direction he is supposed to");
                            }
                            CheckAdjacentTiles();
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
                            try { OnGreenArrow(moveForward, canMoveForward, facingForward); }
                            catch { OnPOLGreenArrow(moveForward, canMoveForward, facingForward); }
                            CheckAdjacentTiles();
                        }
                        break;
                    case 6: //Right Arrow
                        if (!hasReachedEndTile)
                        {
                            try { OnGreenArrow(moveRight, canMoveRight, facingRight); }
                            catch { OnPOLGreenArrow(moveRight, canMoveRight, facingRight); }
                            CheckAdjacentTiles();
                        }
                        break;
                    case 7: //Back Arrow
                        if (!hasReachedEndTile)
                        {
                            try { OnGreenArrow(moveBack, canMoveBack, facingBack); }
                            catch { OnPOLGreenArrow(moveBack, canMoveBack, facingBack); }
                            CheckAdjacentTiles();
                        }
                        break;
                    case 8: //Left Arrow
                        if (!hasReachedEndTile)
                        {
                            try { OnGreenArrow(moveLeft, canMoveLeft, facingLeft); }
                            catch { OnPOLGreenArrow(moveLeft, canMoveLeft, facingLeft); }
                            CheckAdjacentTiles();
                        }
                        break;
                }
                GameManager.turnIsFinished = false;
                tilesBoard = BoardManager.updated_3DBoard;

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
            Debug.Log("FRONT tile name: " + frontTile.name + "; " + "front tile position = " + frontTile.position);
            canMoveForward = false;
        }
        catch { canMoveForward = true; }

        try
        {
            backTile = tilesBoard[xPos, yPos, zPos - 1];
            Debug.Log("BACK tile name: " + backTile.name + "; " + "position = " + backTile.position);
            canMoveBack = false;
        }
        catch { canMoveBack = true; }

        try
        {
            rightTile = tilesBoard[xPos + 1, yPos, zPos];
            Debug.Log("RIGHT tile name: " + rightTile.name + "; " + "position = " + rightTile.position);
            canMoveRight = false;
        }
        catch { canMoveRight = true; }

        try
        {
            leftTile = tilesBoard[xPos - 1, yPos, zPos];
            Debug.Log("LEFT tile name: " + leftTile.name + "; " + "position = " + leftTile.position);
            canMoveLeft = false;
        }
        catch { canMoveLeft = true; }

        try
        {
            if (tilesBoard[xPos, yPos + 1, zPos])
            {
                aboveTile = tilesBoard[xPos, yPos + 1, zPos];
                Debug.Log("ABOVE tile name: " + aboveTile.name + "; " + "position = " + aboveTile.position);
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
                Debug.Log("BELOW tile name: " + belowTile.name + "; " + "position = " + belowTile.position);
            }
        }
        catch
        {
            canMoveForward = false;
            canMoveBack = false;
            canMoveRight = false;
            canMoveLeft = false;
        }
    }


    public void OnGreenArrow(Vector3 moveToDirection, bool canMoveToDirection, Quaternion facingDirection)
    {
        if (canMoveToDirection && greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += moveToDirection;
            lastNonBlankTileType = moveToDirection;
            GameManager.turnCount += 1;
            Debug.Log("turn = " + GameManager.turnCount);
            greenArrowScript.StateSwitch();
        }
        else if (canMoveToDirection && !greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += lastNonBlankTileType;
            GameManager.turnCount += 1;
            Debug.Log("turn = " + GameManager.turnCount);
            greenArrowScript.StateSwitch();
        }
        else if (!canMoveToDirection)
        {
            Debug.Log("the player can't move");
        }
    }


    public void OnPOLGreenArrow(Vector3 moveToDirection, bool canMoveToDirection, Quaternion facingDirection)
    {
        if (canMoveToDirection && pOL_greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += moveToDirection;
            lastNonBlankTileType = moveToDirection;
            GameManager.turnCount += 1;
            Debug.Log("turn = " + GameManager.turnCount);
            pOL_greenArrowScript.StateSwitch();
        }
        else if (canMoveToDirection && !pOL_greenArrowScript.isActive)
        {
            transform.rotation = facingDirection;
            transform.position += lastNonBlankTileType;
            GameManager.turnCount += 1;
            Debug.Log("turn = " + GameManager.turnCount);
            pOL_greenArrowScript.StateSwitch();
        }
        else if (!canMoveToDirection)
        {
            Debug.Log("the player can't move");
        }
    }
}
