using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GreenArrow : MonoBehaviour
{

    [HideInInspector] public bool canBeRotated; // disable rotation when the player has start simulation (space pressed)

    public bool isActive;

    [Header("States Parameter(s)")]
    public bool canBeActivatedAgain;
    public int unactiveTurns;
    private int nextActiveTurn;
    private bool mouseIsOver;

    private Renderer _renderer;

    private GameObject boardManager;
    private TileSelectionSquare tileSelectionSquare;
    private InGameUIManager _inGameUIManager;

    private Transform[,,] tilesBoard;
    private Vector3 above_AdjacentPos;
    private Transform above_AdjacentTile;

    [Header("Rotation Parameter(s)")]
    public float rotationDuration;

    private Quaternion forwardArrow, backArrow, leftArrow, rightArrow;

    [HideInInspector] public string tileOrientation;

    [HideInInspector] public string tagWhenActive;

    private float unactiveTimeColorSwap = 0.3f;
    private float reactiveTimeColorSwap = 0.2f;

    private Color opaqueColor = new Color(1f, 1, 1, 1f);
    private Color transparantColor = new Color(1f, 1, 1, 0f);

    [Header("Appearing animation")]
    public float startingOffset;
    public float duration;

    [Space]
    [Header("Textures & Prefabs")]
    public Texture player_active_greenArrow;
    public Texture player_unactive_greenArrow;
    public Texture deleteGreenArrowTileTexture;
    [Space]
    public Transform blankTilePrefab;

    private bool disappearingAnimation_isFinished;

    private float lastVerticalPos;
    private float disappearingDelay_RelativeToPos;

    void Start()
    {
        mouseIsOver = false;

        if (!_inGameUIManager)
            _inGameUIManager = GameObject.FindGameObjectWithTag("InGameUIManager").GetComponent<InGameUIManager>();

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        boardManager = GameObject.FindGameObjectWithTag("Board Manager");
        isActive = true;
        canBeRotated = true;
        _renderer = GetComponent<Renderer>();

        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);

        if (transform.rotation == forwardArrow)
            tileOrientation = "Forward";
        else if (transform.rotation == rightArrow)
            tileOrientation = "Right";
        else if (transform.rotation == backArrow)
            tileOrientation = "Back";
        else if (transform.rotation == leftArrow)
            tileOrientation = "Left";

        _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
        gameObject.tag = tagWhenActive = "Player Green Arrow";

        disappearingAnimation_isFinished = false;

        StartCoroutine(AppearingAnimation(startingOffset, duration, 0f, 0f, 0.3f));
    }


    private void Update()
    {
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
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            if (tileSelectionSquare.transform.position != transform.position)
            {
                AudioManager.instance.Play("ig tile hovering");
                tileSelectionSquare.transform.position = transform.position;

                if (tileSelectionSquare.transform.rotation != transform.rotation)
                    tileSelectionSquare.transform.rotation = transform.rotation;
            }
        }
    }

    private void OnMouseOver()
    {
        if (!GameManager.gameIsPaused && !GameManager.simulationIsRunning && GameManager.playerCanModifyBoard && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive)
        {
            canBeRotated = true;
            mouseIsOver = true;

            if (tileSelectionSquare.transform.rotation != transform.rotation)
                tileSelectionSquare.transform.rotation = transform.rotation;
            if (tileSelectionSquare.transform.position != transform.position)
                tileSelectionSquare.transform.position = transform.position;

            tileSelectionSquare.material.color = tileSelectionSquare.editTileColor;
            _inGameUIManager.isOverPlayerArrowTile = true;
            if (canBeRotated && mouseIsOver)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.R) || Input.mouseScrollDelta.y < 0f)
                {
                    if (transform.rotation == forwardArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, rightArrow, rotationDuration));
                        tileOrientation = "Right";
                    }
                    else if (transform.rotation == rightArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, backArrow, rotationDuration));
                        tileOrientation = "Back";
                    }
                    else if (transform.rotation == backArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, leftArrow, rotationDuration));
                        tileOrientation = "Left";
                    }
                    else if (transform.rotation == leftArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, forwardArrow, rotationDuration));
                        tileOrientation = "Forward";
                    }
                }
                else if (Input.mouseScrollDelta.y > 0f || Input.GetKeyDown(KeyCode.L))
                {
                    if (transform.rotation == forwardArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, leftArrow, rotationDuration));
                        tileOrientation = "Left";
                    }
                    else if (transform.rotation == rightArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, forwardArrow, rotationDuration));
                        tileOrientation = "Forward";
                    }
                    else if (transform.rotation == backArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, rightArrow, rotationDuration));
                        tileOrientation = "Right";
                    }
                    else if (transform.rotation == leftArrow)
                    {
                        StartCoroutine(TileRotation(transform.rotation, backArrow, rotationDuration));
                        tileOrientation = "Back";
                    }
                }
            }
            else
                _inGameUIManager.isOverPlayerArrowTile = false;

            if (Input.GetMouseButtonDown(1))
            {
                AudioManager.instance.Play("ig tile deleted");
                tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
                int hierarchyIndex = transform.GetSiblingIndex();                                                                        //Store the current hierarchy index of the blank tile.
                Destroy(gameObject);                                                                                                     //Destroy green arrow tile.
                Transform newTile = Instantiate(blankTilePrefab, transform.position, Quaternion.identity, boardManager.transform);        //Instantiate and store the new tile type at the end of the BoardManager.
                newTile.SetSiblingIndex(hierarchyIndex);                                                                                 //Use the stored hierarchy index to put the new tile in place of the deleted one.
                BoardManager.playerHasChangedATile = true;
                CurrentLevelManager.greenArrowStock_static++;
            }
        }
    }

    private void OnMouseExit()
    {
        _inGameUIManager.isOverPlayerArrowTile = false;
        canBeRotated = false;
        mouseIsOver = false;
        tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;
        tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
        _inGameUIManager.isOverPlayerArrowTile = false;
        if (GameManager.playerCanModifyBoard)
        {
            _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
        }
    }

    public void SetInitialState()
    {
        isActive = true;
        canBeRotated = true;
        mouseIsOver = false;
        nextActiveTurn = 0;
        _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
        gameObject.tag = tagWhenActive;
    }

    public void TurnInitializer()
    {
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        StateCheck();
    }

    public void StateCheck()
    {
        if (isActive && above_AdjacentTile && above_AdjacentTile.tag == "Cube" && unactiveTurns != 0)
        {
            StateSwitch();
        }
        else if (canBeActivatedAgain && !isActive && GameManager.currentTurn >= nextActiveTurn)
        {
            StateSwitch();
        }
    }

    public void StateSwitch()
    {
        if (isActive)
        {
            nextActiveTurn = GameManager.currentTurn + unactiveTurns;
            _renderer.material.SetTexture("_MainTex", player_unactive_greenArrow);
            gameObject.tag = "Blank Tile";
            StartCoroutine(BlinkOverSeconds(Color.grey, unactiveTimeColorSwap, true));
            isActive = false;
        }
        else if (canBeActivatedAgain && !isActive)
        {
            nextActiveTurn = 0;
            _renderer.material.SetTexture("_MainTex", player_active_greenArrow);
            gameObject.tag = tagWhenActive;
            StartCoroutine(BlinkOverSeconds(Color.green, reactiveTimeColorSwap, true));
            isActive = true;
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
        AudioManager.instance.Play("ig tile green arrow tile blink");
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

    IEnumerator TileRotation(Quaternion currentRotation, Quaternion targetRotation, float duration)
    {
        AudioManager.instance.Play("ig tile rotation");
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, (elapsedTime / duration));
            elapsedTime += Time.unscaledDeltaTime;

            if (tileSelectionSquare.transform.rotation != transform.rotation)
                tileSelectionSquare.transform.rotation = transform.rotation;

            yield return null;
        }
        tileSelectionSquare.transform.rotation = transform.rotation;
        transform.rotation = targetRotation;
        yield return null;
    }

    public void FirstInitialization()
    {
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
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
