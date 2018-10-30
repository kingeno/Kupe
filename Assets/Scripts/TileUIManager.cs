using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileUIManager : MonoBehaviour
{

    public Transform blankTilePrefab;
    public Transform GreenArrowPrefab;
    public static bool isDeleteTileSelected;
    public static bool isGreenArrowSelected;
    public static bool unselectAll;

    public static bool levelIsReset;

    private void Start()
    {
        unselectAll = false;
        levelIsReset = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("right mouse input");
            unselectAll = true;
        }
        else
            unselectAll = false;

        if (unselectAll)
        {
            isGreenArrowSelected = false;
            isDeleteTileSelected = false;
            Debug.Log("unselect all tiles");
        }
    }

    public void DeleteSelection()
    {
        unselectAll = true;
        if (unselectAll)
        {
            isGreenArrowSelected = false;
            isDeleteTileSelected = false;
        }
        if (!isDeleteTileSelected)
        {
            isDeleteTileSelected = true;
            Debug.Log("delete button selected = " + isDeleteTileSelected);
        }
    }

    public void GreenArrowSelection()
    {
        unselectAll = true;
        if (unselectAll)
        {
            isGreenArrowSelected = false;
            isDeleteTileSelected = false;
        }
        if (!CurrentLevelManager.isGreenArrowStockEmpty && !isGreenArrowSelected)
        {
            isGreenArrowSelected = true;
            Debug.Log("green arrow tile selected = " + isGreenArrowSelected);
        }
    }

    public void RestartCurrentLevel()
    {
        levelIsReset = true;
    }
}