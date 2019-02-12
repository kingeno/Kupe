using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GreenArrowStockText : MonoBehaviour {

    public TextMeshProUGUI textmeshPro;
    private int greenArrowStockToDisplay;

    private void Start()
    {
        textmeshPro = GetComponent<TextMeshProUGUI>();
        SetArrowStockToDisplay();
    }

    void Update()
    {
        if (BoardManager.playerHasChangedATile)
            SetArrowStockToDisplay();
    }

    public void SetArrowStockToDisplay()
    {
        greenArrowStockToDisplay = CurrentLevelManager.greenArrowStock_static;

        if (textmeshPro)
            textmeshPro.SetText(greenArrowStockToDisplay.ToString());
    }
}
