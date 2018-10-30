﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenArrow : MonoBehaviour
{
    public bool canBeRotated; // disable rotation when the player has start simulation (space pressed)

    public bool isActive;
    public int unactiveTurns;
    private int nextActiveTurn;

    public bool startState;

    public GameObject boardManager;
    public Transform blankTilePrefab;

    private Quaternion forwardArrow;
    private Quaternion backArrow;
    private Quaternion leftArrow;
    private Quaternion rightArrow;

    private Renderer _renderer;
    public Texture player_active_greenArrow;
    public Texture player_unactive_greenArrow;

    public Texture deleteGreenArrowTileTexture;

    public string tagWhenActive;

    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BoardManager");
        _renderer = GetComponent<Renderer>();

        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);

        startState = true;

        if (transform.rotation == forwardArrow)
        {
            gameObject.tag = "Forward Arrow";
            tagWhenActive = "Forward Arrow";
        }
        if (transform.rotation == rightArrow)
        {
            gameObject.tag = "Right Arrow";
            tagWhenActive = "Right Arrow";
        }
        if (transform.rotation == backArrow)
        {
            gameObject.tag = "Back Arrow";
            tagWhenActive = "Right Arrow";
        }
        if (transform.rotation == leftArrow)
        {
            gameObject.tag = "Left Arrow";
            tagWhenActive = "Right Arrow";
        }
    }

    private void Update()
    {
        if (GameManager.playerHasLaunchedSimulation)
        {
            if (isActive && !TileUIManager.isDeleteTileSelected)
            {
                _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
                gameObject.tag = tagWhenActive;
            }
            else if (!isActive)
            {
                StateSwitch();
                _renderer.material.SetTexture("_MainTex", player_unactive_greenArrow);
                gameObject.tag = "Blank Tile";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StateSwitch();
        }
    }

    public void StateSwitch()
    {
        if (isActive)
        {
            nextActiveTurn = GameManager.currentTurn + unactiveTurns;
            isActive = false;
            Debug.Log(name + ": state switch function / is deactivated");
        }
        else if (!isActive && GameManager.currentTurn >= nextActiveTurn)
        {
            isActive = true;
            nextActiveTurn = 0;
            Debug.Log(name + ": state switch function / is activated");
        }
    }

    private void OnMouseOver()
    {
        if (!GameManager.playerHasLaunchedSimulation)
        {
            if (TileUIManager.isDeleteTileSelected)
            {
                _renderer.material.SetTexture("_MainTex", deleteGreenArrowTileTexture);

                if (Input.GetMouseButtonDown(0))
                {
                    int hierarchyIndex = transform.GetSiblingIndex();                                                                        //Store the current hierarchy index of the blank tile.
                    Destroy(gameObject);                                                                                                     //Destroy green arrow tile.
                    Transform newTile = Instantiate(blankTilePrefab, transform.position, transform.rotation, boardManager.transform);        //Instantiate and store the new tile type at the end of the BoardManager.
                    newTile.SetSiblingIndex(hierarchyIndex);                                                                                 //Use the stored hierarchy index to put the new tile in place of the deleted one.
                    BoardManager.playerHasChangedATile = true;
                    CurrentLevelManager.greenArrowStock_static++;
                    Debug.Log("stock is empty = " + CurrentLevelManager.isGreenArrowStockEmpty.ToString());
                }
            }
            if (canBeRotated && Input.GetKeyDown(KeyCode.R))
            {
                if (transform.rotation == forwardArrow)
                {
                    transform.rotation = rightArrow;
                    gameObject.tag = "Right Arrow";
                }
                else if (transform.rotation == rightArrow)
                {
                    transform.rotation = backArrow;
                    gameObject.tag = "Back Arrow";
                }
                else if (transform.rotation == backArrow)
                {
                    transform.rotation = leftArrow;
                    gameObject.tag = "Left Arrow";
                }
                else if (transform.rotation == leftArrow)
                {
                    transform.rotation = forwardArrow;
                    gameObject.tag = "Forward Arrow";
                }
            }
        }
    }

    private void OnMouseExit()
    {
        if (TileUIManager.isDeleteTileSelected || GameManager.playerHasLaunchedSimulation)
        {
            _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
        }
    }

    void OnGUI()
    {
        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;
        redStyle.fontSize = 18;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        int _nextActiveTurn = (nextActiveTurn - GameManager.currentTurn);

        if (!isActive && _nextActiveTurn <= 10 && nextActiveTurn > 0)
        {
            GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
            /*"active in " + */(_nextActiveTurn - 1).ToString()
            , redStyle);
        }
        else if (isActive && unactiveTurns > 0 && unactiveTurns <= 10)
        {
            GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
            /*"active in " + */(unactiveTurns - 1).ToString()
            , redStyle);
        }
    }
}
