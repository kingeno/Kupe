using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int turnCount;
    public int currentTurn;
    public int activeTurn;

    public bool hasReachedEndTile;
    public int tileNumber;

    private Vector3 moveForward;
    private Vector3 moveBack;
    private Vector3 moveLeft;
    private Vector3 moveRight;

    private Vector3 startPos;
    private Vector3 currentPos;

    public GameObject startTile;

    public Vector3 lastNonBlankTileType;

    public Texture2D icon;


    void Start()
    {
        turnCount = 0;
        tileNumber = 1;
        hasReachedEndTile = false;

        moveForward = new Vector3(0, 0, 1);
        moveBack = new Vector3(0, 0, -1);
        moveLeft = new Vector3(-1, 0, 0);
        moveRight = new Vector3(1, 0, 0);
    }

    void Update()
    {
        // look for the start tile in the scene and place the Player cube upon it if it find it
        if (startTile == null)
        {
            startTile = GameObject.FindGameObjectWithTag("Start Tile");
            startPos += (startTile.transform.position + new Vector3(0, 1, 0));
            transform.position = startPos;
        }

        if (TileUIManager.restartCurrentLevel)
        {
            turnCount = 0;
            transform.position = startPos;
        }
        switch (tileNumber)
        {
            case 1:
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && PlayerController_V2.canMoveForward)
                {
                    Debug.Log("has left Start Tile");
                    transform.position += moveForward;
                    lastNonBlankTileType = moveForward;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                else if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && !PlayerController_V2.canMoveForward)
                {
                    Debug.Log("The player can't move forward");
                }
                break;
            case 2:
                Debug.Log("End Tile");
                hasReachedEndTile = true;
                break;
            case 3:
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space))
                {
                    if (lastNonBlankTileType == moveForward && PlayerController_V2.canMoveForward)
                    {
                        Debug.Log("Blank Tile");
                        transform.position += lastNonBlankTileType;
                        turnCount += 1;
                        Debug.Log("turn = " + turnCount);
                    }
                    else if (lastNonBlankTileType == moveBack && PlayerController_V2.canMoveBack)
                    {
                        Debug.Log("Blank Tile");
                        transform.position += lastNonBlankTileType;
                        turnCount += 1;
                        Debug.Log("turn = " + turnCount);
                    }
                    else if (lastNonBlankTileType == moveRight && PlayerController_V2.canMoveRight)
                    {
                        Debug.Log("Blank Tile");
                        transform.position += lastNonBlankTileType;
                        turnCount += 1;
                        Debug.Log("turn = " + turnCount);
                    }
                    else if (lastNonBlankTileType == moveLeft && PlayerController_V2.canMoveLeft)
                    {
                        Debug.Log("Blank Tile");
                        transform.position += lastNonBlankTileType;
                        turnCount += 1;
                        Debug.Log("turn = " + turnCount);
                    }
                    else
                    {
                        Debug.Log("The player can't move in the direction he is supposed to");
                    }
                }
                break;
            case 4:
                if (!hasReachedEndTile)
                {
                    Debug.Log("Hole Tile");
                }
                break;
            case 5:
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && PlayerController_V2.canMoveForward)
                {
                    Debug.Log("Forward Arrow Tile");
                    transform.position += moveForward;
                    lastNonBlankTileType = moveForward;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                else if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && !PlayerController_V2.canMoveForward)
                {
                    Debug.Log("the player can't move forward");
                }
                break;
            case 6:
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && PlayerController_V2.canMoveRight)
                {
                    transform.position += moveRight;
                    lastNonBlankTileType = moveRight;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                else if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && !PlayerController_V2.canMoveRight)
                {
                    Debug.Log("the player can't move right");
                }
                break;
            case 7:
                Debug.Log("Back Arrow Tile");
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && PlayerController_V2.canMoveBack)
                {
                    transform.position += moveBack;
                    lastNonBlankTileType = moveBack;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                else if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && !PlayerController_V2.canMoveBack)
                {
                    Debug.Log("the player can't move back");
                }
                break;
            case 8:
                Debug.Log("Left Arrow Tile");
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && PlayerController_V2.canMoveLeft)
                {
                    transform.position += moveLeft;
                    lastNonBlankTileType = moveLeft;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                else if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space) && !PlayerController_V2.canMoveLeft)
                    Debug.Log("the player can't move left");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Start Tile")
            tileNumber = 1;

        if (other.tag == "End Tile")
            tileNumber = 2;

        if (other.tag == "Blank Tile")
            tileNumber = 3;

        if (other.tag == "Hole Tile")
            tileNumber = 4;

        if (other.tag == "Forward Arrow")
            tileNumber = 5;

        if (other.tag == "Right Arrow")
        {
            Debug.Log("Right Arrow Tile");
            tileNumber = 6;
        }

        if (other.tag == "Back Arrow")
            tileNumber = 7;

        if (other.tag == "Left Arrow")
            tileNumber = 8;
    }


    void OnGUI()
    {
        GUI.Button(new Rect(10, 10, 100, 20), new GUIContent("Click me", icon, "This is the tooltip"));
        GUI.Label(new Rect(10, 40, 100, 20), GUI.tooltip);
    }
}

    //void OnGUI()
    //{
    //    guiStyle.fontSize = 14;
    //    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
    //    float x = screenPos.x;
    //    float y = Screen.height - screenPos.y;

    //    GUI.Label(new Rect(x - 50.0f, y - 100.0f, 20.0f, 50.0f),
    //        "turn " + turnCount.ToString()
    //        //+ "\n" + "energy decrease = " + energyDecrease.ToString()
    //        //+ "\n" + "energy = " + debugDisplayedEnergy.ToString()
    //        , guiStyle);
    //}
