using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : MonoBehaviour {

    public Transform[,,] tilesBoard;
    public Transform tileSelectionSquareTransform;
    public TileSelectionSquare tileSelectionSquare;

    public bool isValidated;
    public Vector3 above_AdjacentPos;
    public Transform above_AdjacentTile;
    public GameObject endTileParticleSystem;

    void Start () {
        if (!tileSelectionSquareTransform)
            tileSelectionSquareTransform = GameObject.FindGameObjectWithTag("TileSelectionSquare").transform;
        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();
        isValidated = false;
        tilesBoard = BoardManager.original_3DBoard;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
    }

    private void OnMouseOver()
    {
        tileSelectionSquareTransform.position = transform.position;
    }

    private void OnMouseExit()
    {
        tileSelectionSquareTransform.position = tileSelectionSquare.hiddenPosition;
    }

    public void SetInitialState()
    {
        isValidated = false;
        tilesBoard = BoardManager.updated_3DBoard;
        endTileParticleSystem.SetActive(true);
    }

    public void TurnInitializer()
    {
        Debug.Log(name + " turn initializer");
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        if (above_AdjacentTile && above_AdjacentTile.tag == "Cube")
        {
            isValidated = true;
            endTileParticleSystem.SetActive(false);
        }
        else if (!above_AdjacentTile)
        {
            isValidated = false;
            endTileParticleSystem.SetActive(true);
        }
    }

        //check the position of the tile relatively to the current cube position
        public Transform TileCheck(Vector3 tilePos)
    {
        //Debug.Log("run " + name + " Tile Check");
        Transform tile;
        if (tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z])
        {
            tile = tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z];
            return tile;
        }
        else
        {
            return null;
        }
    }
}
