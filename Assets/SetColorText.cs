using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetColorText : MonoBehaviour
{
    private TextMeshProUGUI textmeshPro;
    private MainMenuUIManager mainMenuUIManager;

    //public float lerpSpeed;

    private void Start()
    {
        if (!mainMenuUIManager)
            mainMenuUIManager = GameObject.FindGameObjectWithTag("MainMenuUIManager").GetComponent<MainMenuUIManager>();

        if (!textmeshPro)
            textmeshPro = this.GetComponent<TextMeshProUGUI>();
    }

    public void SetDefaultTextColor()
    {
        //textmeshPro.color = Color.Lerp(mainMenuUIManager.defaultColor, mainMenuUIManager.highlightColor, speed /** Time.deltaTime*/);
        textmeshPro.color = mainMenuUIManager.defaultColor;
    }

    public void SetHiglightTextColor()
    {
        //textmeshPro.color = Color.Lerp(mainMenuUIManager.highlightColor, mainMenuUIManager.defaultColor, speed /** Time.deltaTime*/);
        textmeshPro.color = mainMenuUIManager.highlightColor;
    }
}
