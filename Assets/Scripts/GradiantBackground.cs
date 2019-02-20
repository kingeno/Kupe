using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradiantBackground : MonoBehaviour
{

    public RawImage img;
    private Texture2D backgroundTexture;
    public Color topColor;
    public Color bottomColor;

    void Awake()
    {
        backgroundTexture = new Texture2D(1, 2);
        backgroundTexture.wrapMode = TextureWrapMode.Clamp;
        backgroundTexture.filterMode = FilterMode.Trilinear;
        SetColor(Color.blue, Color.red);
    }

    private void Update()
    {
        SetColor(bottomColor, topColor);
    }

    public void SetColor(Color color1, Color color2)
    {
        backgroundTexture.SetPixels(new Color[] { color1, color2 });
        backgroundTexture.Apply();
        img.texture = backgroundTexture;
    }
}
