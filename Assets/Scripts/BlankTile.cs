﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankTile : MonoBehaviour
{
    public GameObject boardManager;
    public TileSelectionSquare tileSelectionSquare;

    public Vector3 above_AdjacentPos;
    public Transform[,,] tilesBoard;
    public Transform above_AdjacentTile;

    public bool canOnlyBeBlankTile;

    public Transform greenArrowPrefab;
    public Transform mouseOverTilePrefab;
    public Transform mouseOverTile;

    //public float randomGreyValue;
    public Color _color;
    public Color transparantColor;
    public Color opaqueColor;

    private Renderer _renderer;
    public Texture blankTileTexture;
    public Texture greenArrowSelectedTexture;

    public float rotationDuration;
    private float rotationIsFinished;

    private Quaternion forwardArrow;
    private Quaternion backArrow;
    private Quaternion leftArrow;
    private Quaternion rightArrow;

    public static Quaternion staticCurrentRotation;

    private bool disappearingAnimation_isFinished;

    [Header("Appearing animation")]
    public float startingOffset;
    public float duration;
    public float minDelay;
    public float maxDelay;

    [Header("Disappearing animation")]
    public float dEndingOffset;
    public float dDuration;
    public float dMinDelay;
    public float dMaxDelay;

    [Header("When arrow deleted animation")]
    public float fromDeletedStartingOffset;
    public float fromDeletedDuration;

    private void Start()
    {
        disappearingAnimation_isFinished = false;
        _renderer = GetComponent<Renderer>();
        if (GameManager.currentSceneTime < 2f)
            StartCoroutine(AppearingAnimation(startingOffset, duration, minDelay, maxDelay));
        else
            StartCoroutine(AppearingAnimation(fromDeletedStartingOffset, fromDeletedDuration, 0f, 0f));

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        boardManager = GameObject.FindGameObjectWithTag("Board Manager");

        staticCurrentRotation = Quaternion.Euler(0, 0, 0);
        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);
    }

    private void Update()
    {
        if (GameManager.simulationIsRunning || Input.GetMouseButtonDown(1) || MainCamera.isFreeLookActive)
        {
            _renderer.material.SetTexture("_MainTex", blankTileTexture);
        }

        if (InGameUIManager.nextLevelIsLoading && !disappearingAnimation_isFinished)
        {
            StartCoroutine(DisappearingAnimation(dEndingOffset, dDuration, dMinDelay, dMaxDelay));
            disappearingAnimation_isFinished = true;
        }
    }

    private void OnMouseOver()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && !canOnlyBeBlankTile && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (tileSelectionSquare.transform.position != transform.position)
            {
                AudioManager.instance.Play("ig tile hovering");
                tileSelectionSquare.transform.position = transform.position;
            }

            if (!InGameUIManager.isGreenArrowSelected && !InGameUIManager.isDeleteTileSelected)
                tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;

            else if (!CurrentLevelManager.isGreenArrowStockEmpty && InGameUIManager.isGreenArrowSelected)
            {
                if (_renderer.material.GetTexture("_MainTex") != greenArrowSelectedTexture)
                {
                    _renderer.material.SetTexture("_MainTex", greenArrowSelectedTexture);
                }

                //if (transform.rotation != currentRotation)
                //    transform.rotation = currentRotation;

                if (Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(KeyCode.R))
                {
                    if (transform.rotation == forwardArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, rightArrow, rotationDuration));
                        staticCurrentRotation = rightArrow;
                    }
                    else if (transform.rotation == rightArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, backArrow, rotationDuration));
                        staticCurrentRotation = backArrow;
                    }
                    else if (transform.rotation == backArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, leftArrow, rotationDuration));
                        staticCurrentRotation = leftArrow;
                    }
                    else if (transform.rotation == leftArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, forwardArrow, rotationDuration));
                        staticCurrentRotation = forwardArrow;
                    }
                }
                else if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.L))
                {
                    if (transform.rotation == forwardArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, leftArrow, rotationDuration));
                        staticCurrentRotation = leftArrow;
                    }
                    else if (transform.rotation == rightArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, forwardArrow, rotationDuration));
                        staticCurrentRotation = forwardArrow;
                    }
                    else if (transform.rotation == backArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, rightArrow, rotationDuration));
                        staticCurrentRotation = rightArrow;
                    }
                    else if (transform.rotation == leftArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, backArrow, rotationDuration));
                        staticCurrentRotation = backArrow;
                    }
                }

                float lerp = Mathf.PingPong(Time.unscaledTime, tileSelectionSquare.blinkingDuration) / tileSelectionSquare.blinkingDuration;
                tileSelectionSquare.material.color = Color.Lerp(tileSelectionSquare.canPlaceTileColor1, tileSelectionSquare.canPlaceTileColor2, lerp);

                if (Input.GetMouseButtonDown(0))
                {
                    AudioManager.instance.Play("ig tile green arrow placed");
                    int hierarchyIndex = transform.GetSiblingIndex();                                                                               //Store the current hierarchy index of the blank tile.
                    Destroy(gameObject);                                                                                                            //Destroy the blank tile.
                    Transform newTile = Instantiate(greenArrowPrefab, transform.position, transform.rotation, boardManager.transform);        //Instantiate and store the new tile type at the end of the BoardManager.
                    newTile.SetSiblingIndex(hierarchyIndex);                                                                                  //Use the stored hierarchy index to put the new tile in place of the deleted one.
                    BoardManager.playerHasChangedATile = true;
                    CurrentLevelManager.greenArrowStock_static--;
                }
            }
            else if (InGameUIManager.isDeleteTileSelected)
            {
                tileSelectionSquare.material.color = tileSelectionSquare.deleteColor;
                _renderer.material.SetTexture("_MainTex", blankTileTexture);
            }

        }
    }

    private void OnMouseEnter()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && !canOnlyBeBlankTile && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (transform.rotation != staticCurrentRotation)
                transform.rotation = staticCurrentRotation;
        }
    }

    private void OnMouseExit()
    {
        if (!GameManager.simulationIsRunning && !canOnlyBeBlankTile && tileSelectionSquare.canBeMoved)
        {
            tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;
        }

        if (!GameManager.simulationIsRunning && !canOnlyBeBlankTile && InGameUIManager.isGreenArrowSelected)
        {
            _renderer.material.SetTexture("_MainTex", blankTileTexture);
        }
        _renderer.material.SetTexture("_MainTex", blankTileTexture);
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

    public void FirstInitialization()
    {
        tilesBoard = BoardManager.original_3DBoard;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        if (above_AdjacentTile && above_AdjacentTile.tag != "Cube")
        {
            canOnlyBeBlankTile = true;
        }
        else
        {
            canOnlyBeBlankTile = false;
        }
    }

    IEnumerator TileRotation(Quaternion currentRotation, Quaternion targetRotation, float duration)
    {
        AudioManager.instance.Play("ig tile rotation");
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, (elapsedTime/duration));
            elapsedTime += Time.unscaledDeltaTime;

            tileSelectionSquare.transform.rotation = transform.rotation;

            yield return null;
        }
        tileSelectionSquare.transform.rotation = transform.rotation;
        transform.rotation = targetRotation;
        yield return null;
    }

    IEnumerator AppearingAnimation(float startingOffset, float duration, float minDelay, float maxDelay)
    {
        float elapsedTime = 0;

        Vector3 endPos = transform.position;

        transform.position = new Vector3(transform.position.x, transform.position.y + startingOffset, transform.position.z);
        Vector3 startingPos = transform.position;

        _renderer.material.color = transparantColor;

        yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));

        while (elapsedTime < duration)
        {
            _renderer.material.color = Color.Lerp(transparantColor, opaqueColor, (elapsedTime / duration));
            float newYPos = EasingFunction.EaseOutCirc(startingPos.y, endPos.y, (elapsedTime / duration));
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _renderer.material.color = opaqueColor;
        transform.position = endPos;
        yield return new WaitForSecondsRealtime(1f);
        FirstInitialization();
    }

    IEnumerator DisappearingAnimation(float endingOffset, float duration, float minDelay, float maxDelay)
    {
        float elapsedTime = 0;

        Vector3 startingPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y + endingOffset, transform.position.z);

        _renderer.material.color = opaqueColor;

        yield return new WaitForSecondsRealtime(Random.Range(minDelay, maxDelay));

        while (elapsedTime < duration)
        {
            _renderer.material.color = Color.Lerp(opaqueColor, transparantColor, (elapsedTime / duration));
            float newYPos = EasingFunction.EaseInCirc(startingPos.y, endPos.y, (elapsedTime / duration));
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        _renderer.material.color = transparantColor;
        transform.position = endPos;
    }
}
