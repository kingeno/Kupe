using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Pivots Child Game Objects
    [Header("Pivots")]
    public Transform downFront; //on Z axis
    public Transform downBack;  //on Z axis
    public Transform downRight; //on X axis
    public Transform downLeft;  //on X axis

    public Transform pivotToRotateArround;
    public float timeToRotate;

    public int turnCount;
    public int currentTurn;
    public int activeTurn;

    public bool hasReachedEndTile;
    public int tileNumber;

    private Vector3 moveForward;
    private Vector3 moveBack;
    private Vector3 moveLeft;
    private Vector3 moveRight;

    private bool hasFallInAHole;

    private Vector3 startPos;
    private Vector3 currentPos;

    public GameObject startTile;

    public Vector3 lastNonBlankTileType;

    private GUIStyle guiStyle = new GUIStyle();

    void Start()
    {
        turnCount = 0;
        tileNumber = 1;
        hasReachedEndTile = false;
        hasFallInAHole = false;

        moveForward = new Vector3(0, 0, 1);
        moveBack = new Vector3(0, 0, -1);
        moveLeft = new Vector3(-1, 0, 0);
        moveRight = new Vector3(1, 0, 0);

        guiStyle.normal.textColor = Color.white;
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
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("has left Start Tile");
                    transform.position += moveForward;
                    lastNonBlankTileType = moveForward;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                break;
            case 2:
                Debug.Log("End Tile");
                hasReachedEndTile = true;
                break;
            case 3:
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Blank Tile");
                    transform.position += lastNonBlankTileType;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                break;
            case 4:
                if (!hasReachedEndTile)
                {
                    Debug.Log("Hole Tile");
                    hasFallInAHole = true;
                }
                break;
            case 5:
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Forward Tile");
                    transform.position += moveForward;
                    lastNonBlankTileType = moveForward;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                break;
            case 6:
                Debug.Log("Right Tile");
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space))
                {
                    transform.position += moveRight;
                    lastNonBlankTileType = moveRight;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                break;
            case 7:
                Debug.Log("Back Tile");
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space))
                {
                    transform.position += moveBack;
                    lastNonBlankTileType = moveBack;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
                break;
            case 8:
                Debug.Log("Left Tile");
                if (!hasReachedEndTile && Input.GetKeyDown(KeyCode.Space))
                {
                    transform.position += moveLeft;
                    lastNonBlankTileType = moveLeft;
                    turnCount += 1;
                    Debug.Log("turn = " + turnCount);
                }
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
            tileNumber = 6;

        if (other.tag == "Back Arrow")
            tileNumber = 7;

        if (other.tag == "Left Arrow")
            tileNumber = 8;
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
}
