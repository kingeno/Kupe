using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_V2 : MonoBehaviour
{

    public static bool canMoveForward;
    public static bool canMoveBack;
    public static bool canMoveRight;
    public static bool canMoveLeft;

    public Transform[,,] tilesBoard;

    Vector3 currentPos;
    Transform frontTile;
    Transform backTile;
    Transform rightTile;
    Transform leftTile;
    Transform aboveTile;
    Transform belowTile;

    public int xPos;
    public int yPos;
    public int zPos;

    private void Start()
    {
        tilesBoard = Test_BoardManager.tilesMultiDimensionArray;
        Debug.Log(tilesBoard.Length);

        for (int z = 0; z < tilesBoard.GetLength(2); z++)
        {
            for (int y = 0; y < tilesBoard.GetLength(1); y++)
            {
                for (int x = 0; x < tilesBoard.GetLength(0); x++)
                {
                    try
                    {
                        Debug.Log(tilesBoard[x, y, z].name + " = ("
                     + x + ", "
                     + y + ", "
                     + z
                     + ")");
                    }
                    catch
                    {
                        //Debug.LogWarning("no tile at position = " + "("
                        //       + x
                        //       + ", " + y
                        //       + ", " + z
                        //       + ")");
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CheckAdjacentTiles();
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
            Debug.Log("front tile name: " + frontTile.name + "; " + "front tile position = " + frontTile.position);
            canMoveForward = false;
        }
        catch
        {
            //Debug.LogWarning("there is no front tile");
            canMoveForward = true;
        }

        try
        {
            backTile = tilesBoard[xPos, yPos, zPos - 1];
            Debug.Log("back tile name: " + backTile.name + "; " + "position = " + backTile.position);
            canMoveBack = false;
        }
        catch
        {
            //Debug.LogWarning("there is no back tile");
            canMoveBack = true;
        }

        try
        {
            rightTile = tilesBoard[xPos + 1, yPos, zPos];
            Debug.Log("right tile name: " + rightTile.name + "; " + "position = " + rightTile.position);
            canMoveRight = false;
        }
        catch
        {
            //Debug.LogWarning("there is no right tile");
            canMoveRight = true;
        }

        try
        {
            leftTile = tilesBoard[xPos - 1, yPos, zPos];
            Debug.Log("left tile name: " + leftTile.name + "; " + "position = " + leftTile.position);
            canMoveLeft = false;
        }
        catch
        {
            //Debug.LogWarning("there is no left tile");
            canMoveLeft = true;
        }

    }

    // not sur a coroutine is useful for this kind of check
    //private IEnumerable CheckAdjacentTiles()
    //{   
    //    yield return null;
    //}
}
