using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test_BoardManager : MonoBehaviour
{

    public int boardWidth;  //width of the array (x axis)
    public int boardHeight; //depth of the array (y axis)
    public int boardDepth;  //depth of the array (z axis)

    private int xPos;
    private int yPos;
    private int zPos;

    public Transform[] tilesArray;
    public static Transform[,,] tilesMultiDimensionArray;

#if UNITY_EDITOR
    private void Start()
    {
        tilesArray = new Transform[transform.childCount];

        tilesMultiDimensionArray = new Transform[boardWidth, boardHeight, boardDepth];     //concerver cette manière de calculer le ratio et créer un nouveau type de tile "out of field tile" pour simuler des tableau non carrés
        int i = 0;


        foreach (Transform child in transform)      // Loop through the BoardManager children in the scene to put every tile in an array so it can be sorted out in multidimensional array after that.
        {
            tilesArray[i] = child.transform;
            i++;
        }

        //Loops through the tilesMultiDimensionArray and put the tiles in the array's cells according to their position in the board.
        // Exemple : if the tile has Vector3(1(x), 0(y), 2(z)) for position, it will be stored in the tilesMultiDimensionArray[1, 2] cells.
        int n = 0;  //used to loop through the uni-dimension array tilesArray.
        for (int z = 0; z < tilesMultiDimensionArray.GetLength(2); z++)
        {
            for (int y = 0; y < tilesMultiDimensionArray.GetLength(1); y++)
            {
                for (int x = 0; x < tilesMultiDimensionArray.GetLength(0); x++)
                {
                    try
                    {
                        xPos = (int)tilesArray[n].transform.position.x; //store the position of the current tile in an int.
                        yPos = (int)tilesArray[n].transform.position.y; //store the position of the current tile in an int.
                        zPos = (int)tilesArray[n].transform.position.z; //store the position of the current tile in an int.
                        tilesMultiDimensionArray[xPos, yPos, zPos] = tilesArray[n];   //use the int where x and z positions were stored before to put the tile in the corresponding cell.
                    }
                    catch
                    {
                        tilesMultiDimensionArray[xPos, yPos, zPos] = null;
                    }
                    n++;
                }
            }
        }

        //(Optional --> For Debug.Log Display) Loops through tilesMultiDimensionArray to display the name and the position of all the tiles contained in it.
        for (int z = 0; z < tilesMultiDimensionArray.GetLength(2); z++)
        {
            for (int y = 0; y < tilesMultiDimensionArray.GetLength(1); y++)
            {
                for (int x = 0; x < tilesMultiDimensionArray.GetLength(0); x++)
                {
                    //Debug.Log(tilesMultiDimensionArray[x, y, z].name + " = ("
                    //     + tilesMultiDimensionArray[x, y, z].transform.position.x
                    //     + ", " + tilesMultiDimensionArray[x, y, z].transform.position.y
                    //     + ", " + tilesMultiDimensionArray[x, y, z].transform.position.z
                    //     + ")");
                    try
                    {
                        Debug.Log(tilesMultiDimensionArray[x, y, z].name + " = ("
                         + x + ", "
                         + y + ", "
                         + z
                         + ")");
                    }
                    catch
                    {
                        Debug.LogWarning("no tile at position = " + "("
                            + x
                            + ", " + y
                            + ", " + z
                            + ")");
                    }
                }
            }
        }

        Debug.Log(tilesMultiDimensionArray.Length);
    }
#endif
}
