using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyTile : MonoBehaviour
{
    [HideInInspector] public GameObject boardManager;
    [HideInInspector] public TileSelectionSquare tileSelectionSquare;

    [HideInInspector] public Vector3 above_AdjacentPos;
    [HideInInspector] public Transform[,,] tilesBoard;
    [HideInInspector] public Transform above_AdjacentTile;

    //public float randomGreyValue;
    [HideInInspector] public Color _color;

    private Renderer _renderer;
    public Texture greyTileTexture;
    public Texture greyTileDeleteImpossibleTexture;

    private void Start()
    {
        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        boardManager = GameObject.FindGameObjectWithTag("Board Manager");
        _renderer = GetComponent<Renderer>();

        tilesBoard = BoardManager.original_3DBoard;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
        above_AdjacentTile = TileCheck(above_AdjacentPos);
    }

    private void Update()
    {
        if (GameManager.simulationIsRunning || Input.GetMouseButtonDown(1) || MainCamera.isFreeLookActive)
        {
            _renderer.material.SetTexture("_MainTex", greyTileTexture);
        }
    }

    private void OnMouseOver()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (tileSelectionSquare.transform.position != transform.position)
            {
                AudioManager.instance.Play("ig tile hovering");
                tileSelectionSquare.transform.position = transform.position;
            }

            if (!InGameUIManager.isGreenArrowSelected && !InGameUIManager.isDeleteTileSelected)
            {
                tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
                AudioManager.instance.Play("ig tile delete impossible");
            }
        }
    }

    private void OnMouseExit()
    {
        if (!GameManager.simulationIsRunning && tileSelectionSquare.canBeMoved)
            tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;

        _renderer.material.SetTexture("_MainTex", greyTileTexture);
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
}
