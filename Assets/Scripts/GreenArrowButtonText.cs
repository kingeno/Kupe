using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GreenArrowButtonText : MonoBehaviour {

    public TextMeshProUGUI textmeshPro;

    private void Awake()
    {
        textmeshPro = GetComponent<TextMeshProUGUI>();
    }


    void Update()
    {
        if (textmeshPro)
            textmeshPro.SetText(CurrentLevelManager.greenArrowStock_static.ToString());
    }
}
