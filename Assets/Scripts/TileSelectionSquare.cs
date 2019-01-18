using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileSelectionSquare : MonoBehaviour {

    public Material material;
    public Vector3 hiddenPosition = new Vector3(0f, 20f, 0f);
    public Color defaultColor;
    public Color canPlaceTileColor1;
    public Color canPlaceTileColor2;
    public Color impossibleToPlaceColor;
    public Color editTileColor;
    public Color deleteColor;
    public Color canDeleteTileColor1;
    public Color canDeleteTileColor2;

    public GameObject contextualWindow;

    public float blinkingDuration;

    public bool canBeMoved;

    void Start () {
        if (blinkingDuration == 0)
            blinkingDuration = 0.7f;
        material = GetComponent<MeshRenderer>().material;
        transform.position = hiddenPosition;
        canBeMoved = true;
    }

    private void Update()
    {
        if (/*GameManager.simulationIsRunning && */!GameManager.playerCanModifyBoard)
        {
            transform.position = hiddenPosition;
        }
        Vector3 contextualWindowPos = Camera.main.WorldToScreenPoint(transform.position);
        contextualWindow.transform.position = contextualWindowPos;
    }
}
