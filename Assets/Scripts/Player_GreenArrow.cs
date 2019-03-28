﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GreenArrow : MonoBehaviour
{
    public bool canBeRotated; // disable rotation when the player has start simulation (space pressed)

    public bool isActive;
    public bool canBeActivatedAgain;
    public int unactiveTurns;
    private int nextActiveTurn;
    private bool mouseIsOver;

    public GameObject boardManager;
    public TileSelectionSquare tileSelectionSquare;
    private InGameUIManager _inGameUIManager;
    public Transform blankTilePrefab;

    public Transform[,,] tilesBoard;
    public Vector3 above_AdjacentPos;
    public Transform above_AdjacentTile;

    public float rotationDuration;

    private Quaternion forwardArrow;
    private Quaternion backArrow;
    private Quaternion leftArrow;
    private Quaternion rightArrow;
    public string tileOrientation;

    private Renderer _renderer;
    public Texture player_active_greenArrow;
    public Texture player_unactive_greenArrow;

    public Texture deleteGreenArrowTileTexture;

    public string tagWhenActive;

    public float unactiveTimeColorSwap;
    public float reactiveTimeColorSwap;

    void Start()
    {
        mouseIsOver = false;
        if (!_inGameUIManager)
        {
            _inGameUIManager = GameObject.FindGameObjectWithTag("InGameUIManager").GetComponent<InGameUIManager>();
        }

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        boardManager = GameObject.FindGameObjectWithTag("Board Manager");
        isActive = true;
        canBeRotated = true;
        _renderer = GetComponent<Renderer>();

        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);

        tagWhenActive = "Player Green Arrow";

        if (transform.rotation == forwardArrow)
            tileOrientation = "Forward";
        else if (transform.rotation == rightArrow)
            tileOrientation = "Right";
        else if (transform.rotation == backArrow)
            tileOrientation = "Back";
        else if (transform.rotation == leftArrow)
            tileOrientation = "Left";

        _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
        gameObject.tag = tagWhenActive;

        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));

        if (reactiveTimeColorSwap == 0)
            reactiveTimeColorSwap = 0.2f;
        if (unactiveTimeColorSwap == 0)
            unactiveTimeColorSwap = 0.3f;
    }


    private void OnMouseOver()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            canBeRotated = true;
            mouseIsOver = true;
            if (tileSelectionSquare.transform.position != transform.position)
            {
                AudioManager.instance.Play("ig tile hovering");
                tileSelectionSquare.transform.position = transform.position;
            }

            if (!InGameUIManager.isDeleteTileSelected)
            {
                tileSelectionSquare.material.color = tileSelectionSquare.editTileColor;
                _inGameUIManager.isOverPlayerArrowTile = true;
                if (canBeRotated && mouseIsOver)
                {
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.R) || Input.mouseScrollDelta.y < 0f)
                    {
                        AudioManager.instance.Play("ig tile rotation");
                        if (transform.rotation == forwardArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, rightArrow, rotationDuration));
                            tileOrientation = "Right";
                        }
                        else if (transform.rotation == rightArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, backArrow, rotationDuration));
                            tileOrientation = "Back";
                        }
                        else if (transform.rotation == backArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, leftArrow, rotationDuration));
                            tileOrientation = "Left";
                        }
                        else if (transform.rotation == leftArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, forwardArrow, rotationDuration));
                            tileOrientation = "Forward";
                        }
                    }
                    else if (Input.mouseScrollDelta.y > 0f || Input.GetKeyDown(KeyCode.L))
                    {
                        AudioManager.instance.Play("ig tile rotation");
                        if (transform.rotation == forwardArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, leftArrow, rotationDuration));
                            tileOrientation = "Left";
                        }
                        else if (transform.rotation == rightArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, forwardArrow, rotationDuration));
                            tileOrientation = "Forward";
                        }
                        else if (transform.rotation == backArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, rightArrow, rotationDuration));
                            tileOrientation = "Right";
                        }
                        else if (transform.rotation == leftArrow)
                        {
                            StartCoroutine(TileRotation(transform.rotation, backArrow, rotationDuration));
                            tileOrientation = "Back";
                        }
                    }
                }
            }
            else
                _inGameUIManager.isOverPlayerArrowTile = false;

            if (InGameUIManager.isDeleteTileSelected)
            {
                float lerp = Mathf.PingPong(Time.unscaledTime, tileSelectionSquare.blinkingDuration) / tileSelectionSquare.blinkingDuration;
                tileSelectionSquare.material.color = Color.Lerp(tileSelectionSquare.canDeleteTileColor1, tileSelectionSquare.canDeleteTileColor2, lerp);
                _renderer.material.SetTexture("_MainTex", deleteGreenArrowTileTexture);

                if (Input.GetMouseButtonDown(0))
                {
                    AudioManager.instance.Play("ig tile deleted");
                    tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
                    int hierarchyIndex = transform.GetSiblingIndex();                                                                        //Store the current hierarchy index of the blank tile.
                    Destroy(gameObject);                                                                                                     //Destroy green arrow tile.
                    Transform newTile = Instantiate(blankTilePrefab, transform.position, Quaternion.identity, boardManager.transform);        //Instantiate and store the new tile type at the end of the BoardManager.
                    newTile.SetSiblingIndex(hierarchyIndex);                                                                                 //Use the stored hierarchy index to put the new tile in place of the deleted one.
                    BoardManager.playerHasChangedATile = true;
                    CurrentLevelManager.greenArrowStock_static++;
                    Debug.Log("stock is empty = " + CurrentLevelManager.isGreenArrowStockEmpty.ToString());
                }
            }
        }
        else
            _inGameUIManager.isOverPlayerArrowTile = false;
    }

    private void OnMouseEnter()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (tileSelectionSquare.transform.rotation != transform.rotation)
                tileSelectionSquare.transform.rotation = transform.rotation;
        }
    }

    private void OnMouseExit()
    {
        canBeRotated = false;
        mouseIsOver = false;
        tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;
        tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
        _inGameUIManager.isOverPlayerArrowTile = false;
        if (GameManager.playerCanModifyBoard)
        {
            _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
        }
    }

    public void SetInitialState()
    {
        isActive = true;
        canBeRotated = true;
        mouseIsOver = false;
        nextActiveTurn = 0;
        _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
        gameObject.tag = tagWhenActive;
    }

    public void TurnInitializer()
    {
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        StateCheck();
    }

    public void StateCheck()
    {
        if (isActive && above_AdjacentTile && above_AdjacentTile.tag == "Cube" && unactiveTurns != 0)
        {
            StateSwitch();
        }
        else if (canBeActivatedAgain && !isActive && GameManager.currentTurn >= nextActiveTurn)
        {
            StateSwitch();
        }
    }

    public void StateSwitch()
    {
        if (isActive)
        {
            nextActiveTurn = GameManager.currentTurn + unactiveTurns;
            _renderer.material.SetTexture("_MainTex", player_unactive_greenArrow);
            gameObject.tag = "Blank Tile";
            StartCoroutine(BlinkOverSeconds(Color.grey, unactiveTimeColorSwap, true));
            isActive = false;
        }
        else if (canBeActivatedAgain && !isActive)
        {
            nextActiveTurn = 0;
            _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
            gameObject.tag = tagWhenActive;
            StartCoroutine(BlinkOverSeconds(Color.green, reactiveTimeColorSwap, true));
            isActive = true;
        }
    }

    public Transform TileCheck(Vector3 tilePos)
    {
        Transform tile;
        if (tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z])
        {
            tile = tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z];
            return tile;
        }
        else
            return null;
    }

    IEnumerator BlinkOverSeconds(Color blinkColor, float seconds, bool isBlinking)
    {
        float elapsedTime = 0;
        //Color startColor = _renderer.material.GetColor("_Color");
        Color startColor = Color.white;
        while (elapsedTime < seconds)
        {
            _renderer.material.color = Color.Lerp(startColor, blinkColor, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (isBlinking)
        {
            elapsedTime = 0;
            while (elapsedTime < seconds)
            {
                _renderer.material.color = Color.Lerp(blinkColor, startColor, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _renderer.material.color = startColor;
        }
    }

    IEnumerator TileRotation(Quaternion currentRotation, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, (elapsedTime / duration));
            elapsedTime += Time.unscaledDeltaTime;

            if (tileSelectionSquare.transform.rotation != transform.rotation)
                tileSelectionSquare.transform.rotation = transform.rotation;

            yield return null;
        }
        tileSelectionSquare.transform.rotation = transform.rotation;
        transform.rotation = targetRotation;
        yield return null;
    }

    //void OnGUI()
    //{
    //    GUIStyle redStyle = new GUIStyle();
    //    redStyle.normal.textColor = Color.red;
    //    redStyle.fontSize = 18;
    //    Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
    //    float x = screenPos.x;
    //    float y = Screen.height - screenPos.y;

    //    int _nextActiveTurn = (nextActiveTurn - GameManager.currentTurn);

    //    if (!isActive && _nextActiveTurn <= 10 && nextActiveTurn > 0)
    //    {
    //        GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
    //        /*"active in " + */(_nextActiveTurn - 1).ToString()
    //        , redStyle);
    //    }
    //    else if (isActive && unactiveTurns > 0 && unactiveTurns <= 10)
    //    {
    //        GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
    //        /*"active in " + */(unactiveTurns - 1).ToString()
    //        , redStyle);
    //    }

    //    if (mouseIsOver && canBeRotated && !_inGameUIManager.isDeleteTileSelected)
    //    {
    //        GUI.Box(new Rect(x - 50f, y - 80.0f, 20.0f, 50.0f),
    //        "press R to rotate", redStyle);
    //    }
    //}
}
