using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// https://answers.unity.com/questions/780323/unity-ui-fading-canvaspanel.html --> about fading the canvas renderer

public class ButtonBlinking : MonoBehaviour
{
    public CanvasRenderer _cR;
    public bool blink;

    private void Start()
    {
        _cR = GetComponent<CanvasRenderer>();
        blink = true;
    }

    private void Update()
    {
        if (gameObject.activeSelf && blink)
        {
            _cR.SetAlpha(Mathf.PingPong(Time.unscaledTime, 1f));
        }
    }

    public void StartBlinking()
    {
        StartCoroutine(Wait());
    }

    public void StopBlinking()
    {
        StopCoroutine(Wait());
        blink = false;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        blink = true;
    }
}
