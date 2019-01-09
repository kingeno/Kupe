using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelectionSquare : MonoBehaviour {

    public Material material;
    public Vector3 hiddenPosition = new Vector3(-10, 0, -10);
    public Color defaultColor;
    public Color canPlaceTileColor1;
    public Color canPlaceTileColor2;
    public Color impossibleToPlaceColor;
    public Color editTileColor;
    public Color deleteColor;

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
    }
}
