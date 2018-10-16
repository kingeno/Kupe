using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreenArrowButtonText : MonoBehaviour {

    public Text _text;

    private void Awake()
    {
        if(!_text)
            _text = this.GetComponent<Text>();
    }


    void Update () {
        if (_text)
        {
            _text.text = CurrentLevelManager.greenArrowStock_static.ToString();
        }
    }
}
