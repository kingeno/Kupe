using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : MonoBehaviour {

    public Transform[,,] tilesBoard;

    public bool isValidated;
    public Vector3 above_AdjacentPos;
    public Transform above_AdjacentTile;


    void Start () {
        isValidated = false;
        tilesBoard = BoardManager.original_3DBoard;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
    }

    public void SetInitialState()
    {
        isValidated = false;
        tilesBoard = BoardManager.updated_3DBoard;
    }

    public void TurnInitializer()
    {
        Debug.Log(name + " turn initializer");
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        if (above_AdjacentTile && above_AdjacentTile.tag == "Cube")
            isValidated = true;
        else if (!above_AdjacentTile)
            isValidated = false;
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
