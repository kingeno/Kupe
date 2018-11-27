using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelManager : MonoBehaviour {

    public int greenArrowStock;
    public static int greenArrowStock_static;
    public static bool isGreenArrowStockEmpty;
    
    private void Awake()
    {
        greenArrowStock_static = greenArrowStock;
        isGreenArrowStockEmpty = false;
    }

    private void Update()
    {
        if (greenArrowStock_static <= 0)
        {
            isGreenArrowStockEmpty = true;
        }
        else
        {
                isGreenArrowStockEmpty = false;
        }
    }
}
