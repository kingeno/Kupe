using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankTile : MonoBehaviour
{       
    private GameObject boardManager;
    private TileSelectionSquare tileSelectionSquare;

    private Vector3 above_AdjacentPos;
    private Transform[,,] tilesBoard;
    private Transform above_AdjacentTile;

    private bool canOnlyBeBlankTile;

    private Renderer _renderer;

    private Color opaqueColor = new Color(1f, 1, 1, 1f);
    private Color transparantColor = new Color(1f, 1, 1, 0f);

    private Quaternion forwardArrow, backArrow, leftArrow, rightArrow;

    public static Quaternion staticCurrentRotation;

    private bool disappearingAnimation_isFinished = false;

    [Header("Rotation Parameters")]
    public float rotationDuration;

    [Header("After arrow deleted animation")]
    public float fromDeletedStartingOffset;
    public float fromDeletedDuration;

    [Space]
    [Header("Textures & Prefabs")]
    public Texture blankTileTexture;
    public Texture greenArrowSelectedTexture;
    [Space]
    public Transform greenArrowPrefab;
    public Transform mouseOverTilePrefab;

    private float initialVerticalPos;
    private float lastVerticalPos;
    private float appearingDelay_RelativeToPos;
    private float disappearingDelay_RelativeToPos;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        boardManager = GameObject.FindGameObjectWithTag("Board Manager");

        staticCurrentRotation = Quaternion.Euler(0, 0, 0);
        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);

        initialVerticalPos = transform.position.y;
        appearingDelay_RelativeToPos = 0.1f + (initialVerticalPos * GameManager._timeBetweenWaves);

        if (GameManager.currentSceneTime < 1f)
            StartCoroutine(AppearingAnimation(GameManager._startingOffset, GameManager._duration, appearingDelay_RelativeToPos, appearingDelay_RelativeToPos + GameManager._appearingWaveDuration, GameManager._timeToWaitBeforeFirstInitialization));
        else
            StartCoroutine(AppearingAnimation(fromDeletedStartingOffset, fromDeletedDuration, 0f, 0f, 0f));
    }

    private void Update()
    {
        if (GameManager.simulationIsRunning || Input.GetMouseButtonDown(1) || MainCamera.isFreeLookActive)
        {
            _renderer.material.SetTexture("_MainTex", blankTileTexture);
        }

        if (InGameUIManager.nextLevelIsLoading && !disappearingAnimation_isFinished)
        {
            lastVerticalPos = transform.position.y;
            disappearingDelay_RelativeToPos = lastVerticalPos * GameManager._timeBetweenWaves;
            StartCoroutine(DisappearingAnimation(GameManager._dEndingOffset, GameManager._dDuration, disappearingDelay_RelativeToPos, disappearingDelay_RelativeToPos + GameManager._disappearingWaveDuration));
            disappearingAnimation_isFinished = true;
        }
    }

    private void OnMouseEnter()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && !canOnlyBeBlankTile && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (transform.rotation != staticCurrentRotation)
                transform.rotation = staticCurrentRotation;

            if (tileSelectionSquare.transform.position != transform.position)
            {
                AudioManager.instance.Play("ig tile hovering");
                tileSelectionSquare.transform.position = transform.position;
                tileSelectionSquare.transform.rotation = transform.rotation;
            }
        }
    }

    private void OnMouseOver()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && !canOnlyBeBlankTile && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (tileSelectionSquare.transform.rotation != transform.rotation)
                tileSelectionSquare.transform.rotation = transform.rotation;
            if (tileSelectionSquare.transform.position != transform.position)
                tileSelectionSquare.transform.position = transform.position;

            //if (!InGameUIManager.isGreenArrowSelected && !InGameUIManager.isDeleteTileSelected)
            //    tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;

            if (!CurrentLevelManager.isGreenArrowStockEmpty)
            {
                if (_renderer.material.GetTexture("_MainTex") != greenArrowSelectedTexture)
                {
                    _renderer.material.SetTexture("_MainTex", greenArrowSelectedTexture);
                }

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
                    CurrentLevelManager.greenArrowStock_static--;
                    BoardManager.playerHasChangedATile = true;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                    AudioManager.instance.Play("ig tile grey hovering");

                tileSelectionSquare.material.color = tileSelectionSquare.onGreyTileColor;
            }
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

    IEnumerator AppearingAnimation(float startingOffset, float duration, float minDelay, float maxDelay, float timeToWaitBeforeFirstInitialization)
    {
        float elapsedTime = 0;

        Vector3 endPos = transform.position;

        transform.position = new Vector3(transform.position.x, transform.position.y + startingOffset, transform.position.z);
        Vector3 startingPos = transform.position;

        _renderer.material.color = transparantColor;
        GameManager.playerCanModifyBoard = false;
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
        GameManager.playerCanModifyBoard = true;
        yield return new WaitForSecondsRealtime(timeToWaitBeforeFirstInitialization);
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
