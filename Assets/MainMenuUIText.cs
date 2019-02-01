using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuUIText : MonoBehaviour
{
    public TextMeshProUGUI textmeshPro;

    public Color defaultColor;
    public Color highlightedColor;

    public float speed;

    private void Start()
    {
        if (!textmeshPro)
            textmeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void SetDefaultTextColor()
    {
        //textmeshPro.color = Color.Lerp(highlightedColor, defaultColor, speed * Time.deltaTime);
        textmeshPro.color = defaultColor;
    }

    public void SetHiglightTextColor()
    {
        //textmeshPro.color = Color.Lerp(defaultColor, highlightedColor, speed * Time.deltaTime);
        textmeshPro.color = highlightedColor;
    }
}
