using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUIManager : MonoBehaviour {

    public Transform GreenArrowPrefab;
    public static bool isGreenArrowSelected;
    public static bool unselectAll;
    public static bool restartCurrentLevel;


    private void Start()
    {
        unselectAll = false;
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
            Debug.Log("unselect all tiles");
        }
    }

    public void TileSelected()
    {
        if (!isGreenArrowSelected)
        {
            isGreenArrowSelected = true;
            Debug.Log("green arrow tile selected = " + isGreenArrowSelected);
        }
    }

    public void RestartCurrentLevel()
    {
        if (!restartCurrentLevel)
            restartCurrentLevel = true;
        else
            restartCurrentLevel = false;
    }
}