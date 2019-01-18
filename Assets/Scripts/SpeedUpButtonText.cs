using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedUpButtonText : MonoBehaviour {

    public FontStyles defaultStyle;
    public FontStyles highlightedStyle;

    public Color defaultColor;
    public Color highlightedColor;
    public GameManager gameManager;
    public TextMeshProUGUI textmeshPro;
    public int multiplier; // this has to be set in the inspector as 1, 2 or 3 depending on the text it is attached to.
                            // 1 is for "x1" text, 2 is for "x2 text", etc.

    private void Awake()
    {
        if (!gameManager)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (!textmeshPro)
            textmeshPro = GetComponent<TextMeshProUGUI>();
    }


    void Update()
    {
        if (textmeshPro && gameManager)
        {
            if (gameManager.simulationSpeed == multiplier)
            {
                textmeshPro.fontSize = 18;
                textmeshPro.color = highlightedColor;
                textmeshPro.fontStyle = highlightedStyle;
            }
            else
            {
                textmeshPro.fontSize = 12;
                textmeshPro.color = defaultColor;
                textmeshPro.fontStyle = defaultStyle;
            }
        }
    }
}
