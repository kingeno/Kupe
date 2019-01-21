using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelManager : MonoBehaviour {

    public int greenArrowStock;
    public int currentGreenArrowStock;
    public static int greenArrowStock_static;
    public static bool isGreenArrowStockEmpty;
    public static bool greenArrowStockIsFull;
    
    private void Awake()
    {
        greenArrowStock_static = currentGreenArrowStock = greenArrowStock;
        greenArrowStockIsFull = true;
        isGreenArrowStockEmpty = false;
    }

    private void Update()
    {
        if (greenArrowStock_static != currentGreenArrowStock)
        {
            currentGreenArrowStock = greenArrowStock_static;
        }

        if (greenArrowStock_static <= 0)
        {
            isGreenArrowStockEmpty = true;
        }
        else
        {
            isGreenArrowStockEmpty = false;
        }

        if (currentGreenArrowStock == greenArrowStock)
            greenArrowStockIsFull = true;
        else if (currentGreenArrowStock < greenArrowStock)
            greenArrowStockIsFull = false;
    }
}
