﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EphemereTile : MonoBehaviour {

    public bool isActive;
    public bool canBeActivatedAgain;
    public int unactiveTurns;
    public int nextActiveTurn;

    public Transform[,,] tilesBoard;

    public Vector3 above_AdjacentPos;
    public Transform above_AdjacentTile;

    public string tagWhenActive;

    private Renderer _renderer;
    public Texture active;
    //public Texture unactive;

    public float unactiveTimeColorSwap;
    public float reactiveTimeColorSwap;
    public float fadeOutTime;

    public Color emptyColor;

    void Start()
    {
        isActive = true;
        _renderer = GetComponent<Renderer>();

        tagWhenActive = "EphemereTile";

        _renderer.material.SetTexture("_MainTex", active);
        gameObject.tag = tagWhenActive;

        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));

        if (reactiveTimeColorSwap == 0)
            reactiveTimeColorSwap = 0.2f;
        if (unactiveTimeColorSwap == 0)
            unactiveTimeColorSwap = 0.3f;
        if (fadeOutTime == 0)
            fadeOutTime = 0.3f;
    }

    private void OnMouseOver()
    {
        GameManager.mouseOverTile.transform.position = transform.position;
    }

    private void OnMouseExit()
    {
        GameManager.mouseOverTile.transform.position = new Vector3(-10f, 0f, -10f);
    }

    public void SetInitialState()
    {
        isActive = true;
        nextActiveTurn = 0;
        _renderer.material.color = Color.white;
        //_renderer.material.SetTexture("_MainTex", active);
        tag = tagWhenActive;
    }

    public void TurnInitializer()
    {
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        StateSwitch();
    }

    public void StateSwitch()
    {
        if (isActive && above_AdjacentTile && above_AdjacentTile.tag == "Cube")
        {
            if (unactiveTurns != 0)
            {
                nextActiveTurn = GameManager.currentTurn + unactiveTurns;
                //_renderer.material.SetTexture("_MainTex", unactive);
                gameObject.tag = "EmptyTile";
                StartCoroutine(FadeOverSeconds(emptyColor, fadeOutTime));
                isActive = false;
            }
            else if (unactiveTurns == 0)
            {
                StartCoroutine(FadeOverSeconds(emptyColor, fadeOutTime));
            }
        }
        else if (!isActive)
        {
            if (canBeActivatedAgain && GameManager.currentTurn >= nextActiveTurn)
            {
                nextActiveTurn = 0;
                //_renderer.material.SetTexture("_MainTex", active);
                gameObject.tag = tagWhenActive;
                StartCoroutine(FadeOverSeconds(Color.white, fadeOutTime));
                StartCoroutine(BlinkOverSeconds(Color.green, reactiveTimeColorSwap, true));
                isActive = true;
            }
        }
    }

    public Transform TileCheck(Vector3 tilePos)
    {
        Transform tile;
        if (tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z])
        {
            tile = tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z];
            return tile;
        }
        else
            return null;
    }

    IEnumerator BlinkOverSeconds(Color blinkColor, float seconds, bool isBlinking)
    {
        float elapsedTime = 0;
        //Color startColor = _renderer.material.GetColor("_Color");
        Color startColor = Color.white;
        while (elapsedTime < seconds)
        {
            _renderer.material.color = Color.Lerp(startColor, blinkColor, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (isBlinking)
        {
            elapsedTime = 0;
            while (elapsedTime < seconds)
            {
                _renderer.material.color = Color.Lerp(blinkColor, startColor, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _renderer.material.color = startColor;
        }
    }

    IEnumerator FadeOverSeconds(Color fadedColor, float seconds)
    {
        float elapsedTime = 0;
        Color startColor = Color.white;
        fadedColor = startColor;
        fadedColor.a = 0f;
        while (elapsedTime < seconds)
        {
            _renderer.material.color = Color.Lerp(startColor, fadedColor, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _renderer.material.color = fadedColor;
    }
}
