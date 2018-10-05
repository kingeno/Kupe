using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankTile : MonoBehaviour {

    public Transform greenArrowPrefab;

    private Renderer _renderer;
    public Texture blankTileTexture;
    public Texture greenArrowSelectedTexture;
    public int xPosInArray;
    public int zPosInArray;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        xPosInArray = (int)transform.position.x;
        zPosInArray = (int)transform.position.z;


    }

    private void OnMouseOver()
    {
        if (TileUIManager.isGreenArrowSelected)
        {
            _renderer.material.SetTexture("_MainTex", greenArrowSelectedTexture);

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(greenArrowPrefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                _renderer.material.SetTexture("_MainTex", blankTileTexture);
            }
        }
    }

    private void OnMouseExit()
    {
        if (TileUIManager.isGreenArrowSelected)
        {
            _renderer.material.SetTexture("_MainTex", blankTileTexture);
        }
    }
}
