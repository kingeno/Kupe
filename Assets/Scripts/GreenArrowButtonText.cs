using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GreenArrowButtonText : MonoBehaviour {

    public Text _text;
    public TextMeshProUGUI textmeshPro;

    private void Awake()
    {
        textmeshPro = GetComponent<TextMeshProUGUI>();
        
        if (!_text)
            _text = this.GetComponent<Text>();
    }


    void Update () {
        if (textmeshPro)
            textmeshPro.SetText(CurrentLevelManager.greenArrowStock_static.ToString());
        if (_text)
        {
            _text.text = CurrentLevelManager.greenArrowStock_static.ToString();
        }
    }
}
