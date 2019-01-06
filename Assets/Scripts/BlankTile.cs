using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankTile : MonoBehaviour {

    public GameObject boardManager;

    public Vector3 above_AdjacentPos;
    public Transform[,,] tilesBoard;
    public Transform above_AdjacentTile;

    public bool canOnlyBeBlankTile;

    public Transform greenArrowPrefab;
    public Transform mouseOverTilePrefab;
    public Transform mouseOverTile;

    //public float randomGreyValue;
    public Color _color;

    private Renderer _renderer;
    public Texture blankTileTexture;
    public Texture greenArrowSelectedTexture;

    private void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("Board Manager");
        _renderer = GetComponent<Renderer>();

        tilesBoard = BoardManager.original_3DBoard;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        if (above_AdjacentTile && above_AdjacentTile.tag != "Cube")
        {
            canOnlyBeBlankTile = true;
        }
        else
        {
            canOnlyBeBlankTile = false;
        }



        //randomGreyValue = Random.Range(0.80f, 0.95f);

        //_color = new Color(randomGreyValue, randomGreyValue, randomGreyValue);    

        _renderer.material.color = _color;
    }

    private void OnMouseOver()
    {
        if (!canOnlyBeBlankTile)
        {
            GameManager.mouseOverTile.transform.position = transform.position;
            if (!GameManager.simulationIsRunning && !CurrentLevelManager.isGreenArrowStockEmpty && InGameUIManager.isGreenArrowSelected && GameManager.playerCanModifyBoard)
            {
                _renderer.material.SetTexture("_MainTex", greenArrowSelectedTexture);

                if (Input.GetMouseButtonDown(0))
                {
                    int hierarchyIndex = transform.GetSiblingIndex();                                                                               //Store the current hierarchy index of the blank tile.
                    Destroy(gameObject);                                                                                                            //Destroy the blank tile.
                    Transform newTile = Instantiate(greenArrowPrefab, transform.position, Quaternion.identity, boardManager.transform);        //Instantiate and store the new tile type at the end of the BoardManager.
                    newTile.SetSiblingIndex(hierarchyIndex);                                                                                  //Use the stored hierarchy index to put the new tile in place of the deleted one.
                    BoardManager.playerHasChangedATile = true;
                    CurrentLevelManager.greenArrowStock_static--;
                    //Debug.Log("stock is empty = " + CurrentLevelManager.isGreenArrowStockEmpty.ToString());
                }
                else if (GameManager.simulationIsRunning || Input.GetMouseButtonDown(1))
                {
                    _renderer.material.SetTexture("_MainTex", blankTileTexture);
                }
            }
            
        }
    }

    private void OnMouseExit()
    {
        GameManager.mouseOverTile.transform.position = new Vector3(-10f, 0f, -10f);
        if (InGameUIManager.isGreenArrowSelected)
        {
            _renderer.material.SetTexture("_MainTex", blankTileTexture);
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
}
