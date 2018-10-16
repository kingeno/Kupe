using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GreenArrow greenArrowScript;

    public int original_boardWidth;  //width of the orignal 3D array (x axis)
    public int original_boardHeight; //depth of the orignal 3D array (y axis)
    public int original_boardDepth;  //depth of the orignal 3D array (z axis)

    private int xPos;
    private int yPos;
    private int zPos;

    public Transform[] original_boardManagerArray;
    public Transform[] updated_boardManagerArray;
    public static Transform[,,] original_3DBoard;
    public static Transform[,,] updated_3DBoard;

    public static bool playerHasChangedATile;


    private void Awake()
    {
        original_boardManagerArray = LoopThrough1DArray(original_boardManagerArray);
        original_3DBoard = LoopThroughOriginal3DBoard(original_boardManagerArray, original_3DBoard, original_boardWidth, original_boardHeight, original_boardDepth);
        updated_boardManagerArray = original_boardManagerArray;
    }

    private void LateUpdate()
    {
        if (playerHasChangedATile) // this has to be placed in LateUpdate because unity can't destroy and replace an array's empty index in one frame.
        {
            updated_boardManagerArray = LoopThrough1DArray(updated_boardManagerArray);
            updated_3DBoard = LoopThroughOriginal3DBoard(updated_boardManagerArray, updated_3DBoard, original_boardWidth, original_boardHeight, original_boardDepth);
            playerHasChangedATile = false;
        }
    }

    public Transform[,,] LoopThroughOriginal3DBoard(Transform[] boardManagerArray, Transform[,,] board3D, int boardWidth, int boardHeight, int boardDepth)
    {
        board3D = new Transform[boardWidth, boardHeight, boardDepth];   //concerver cette manière de calculer le ratio et créer un nouveau type de tile "out of field tile" pour simuler des tableau non carrés

        //Loops through the original_3DBoard and put the tiles in the array's cells according to their position in the board.
        // Exemple : if the tile has Vector3(1(x), 0(y), 2(z)) for position, it will be stored in the original_3DBoard[1, 0, 2] cell.
        int n = 0;  //used to loop through the uni-dimension array original_boardManagerArray.
        for (int z = 0; z < board3D.GetLength(2); z++)
        {
            for (int y = 0; y < board3D.GetLength(1); y++)
            {
                for (int x = 0; x < board3D.GetLength(0); x++)
                {
                    try
                    {
                        xPos = (int)boardManagerArray[n].transform.position.x; //store the position of the current tile in an int.
                        yPos = (int)boardManagerArray[n].transform.position.y; //store the position of the current tile in an int.
                        zPos = (int)boardManagerArray[n].transform.position.z; //store the position of the current tile in an int.
                        board3D[xPos, yPos, zPos] = boardManagerArray[n];   //use the int where x, y and z positions were stored before to put the tile in the corresponding index.
                    }
                    catch
                    {
                        if (!board3D[xPos, yPos, zPos])
                            board3D[xPos, yPos, zPos] = null;
                    }
                    n++;
                }
            }
        }
        Debug.LogWarning("has looped through 3D board");

        ////(Optional-- > For Debug.Log Display) Loops through original_3DBoard to display the name and the position of all the tiles contained in it.
        //Debug.Log(board3D.Length);
        //for (int z = 0; z < board3D.GetLength(2); z++)
        //{
        //    for (int y = 0; y < board3D.GetLength(1); y++)
        //    {
        //        for (int x = 0; x < board3D.GetLength(0); x++)
        //        {
        //            try
        //            {
        //                Debug.Log(board3D[x, y, z].name + " = ("
        //                 + x + ", "
        //                 + y + ", "
        //                 + z
        //                 + ")");
        //            }
        //            catch
        //            {
        //                Debug.LogWarning("no tile at position = " + "("
        //                    + x
        //                    + ", " + y
        //                    + ", " + z
        //                    + ")");
        //            }
        //        }
        //    }
        //}

        return board3D;
    }

    public Transform[] LoopThrough1DArray(Transform[] boardManagerArray)
    {
        boardManagerArray = new Transform[transform.childCount];
        //Debug.Log(boardManagerArray.Length);

        int i = 0;
        foreach (Transform child in transform)      // Loop through the BoardManager children in the scene to put every tile in an array so it can be sorted out in multidimensional array after that.
        {
            if (!boardManagerArray[i])
            {
                boardManagerArray[i] = child.transform;
                i++;
            }
            else
                boardManagerArray[boardManagerArray.Length] = child.transform;
        }
        return boardManagerArray;
    }
}
