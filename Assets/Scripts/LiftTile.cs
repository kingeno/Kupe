using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftTile : MonoBehaviour
{
    private Transform[,,] tilesBoard;
    private TileSelectionSquare tileSelectionSquare;

    private Vector3 initialPos;
    private Vector3 above_AdjacentPos;
    private Transform above_AdjacentTile;

    [HideInInspector] public string tagWhenActive;

    private Renderer _renderer;

    private bool isActive;

    private float unactiveTimeColorSwap = 0.3f;
    private float reactiveTimeColorSwap = 0.2f;

    private Color opaqueColor = new Color(1f, 1, 1, 1f);
    private Color transparantColor = new Color(1f, 1, 1, 0f);

    private bool disappearingAnimation_isFinished = false;

    [Header("States Parameters")]
    public bool canBeActivatedAgain;
    public bool goBackDownAfterUse;
    public int unactiveTurns;
    private int nextActiveTurn;

    [Space]
    [Header("Textures")]
    public Texture active_lift;
    public Texture unactive_lift;
    public Texture liftTileDeleteImpossible;

    private float initialVerticalPos;
    private float lastVerticalPos;
    private float appearingDelay_RelativeToPos;
    private float disappearingDelay_RelativeToPos;

    void Start()
    {
        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        isActive = true;
        _renderer = GetComponent<Renderer>();

        _renderer.material.SetTexture("_MainTex", active_lift);
        gameObject.tag = tagWhenActive = "LiftTile";

        initialVerticalPos = transform.position.y;
        appearingDelay_RelativeToPos = 0.1f + (initialVerticalPos * GameManager._timeBetweenWaves);
        StartCoroutine(AppearingAnimation(GameManager._startingOffset, GameManager._duration, appearingDelay_RelativeToPos, appearingDelay_RelativeToPos + GameManager._appearingWaveDuration, GameManager._timeToWaitBeforeFirstInitialization));
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
        if (!GameManager.gameIsPaused && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive && GameManager.playerCanModifyBoard)
        {
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
        if (!GameManager.gameIsPaused && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive && GameManager.playerCanModifyBoard)
        {
            if (tileSelectionSquare.transform.rotation != transform.rotation)
                tileSelectionSquare.transform.rotation = transform.rotation;
            if (tileSelectionSquare.transform.position != transform.position)
                tileSelectionSquare.transform.position = transform.position;

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                AudioManager.instance.Play("ig tile grey hovering");

            tileSelectionSquare.material.color = tileSelectionSquare.onGreyTileColor;

            if (isActive)
                _renderer.material.SetTexture("_MainTex", active_lift);
            else
                _renderer.material.SetTexture("_MainTex", unactive_lift);
        }
    }

    private void OnMouseExit()
    {
        if (tileSelectionSquare.canBeMoved)
            tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;
        if (!GameManager.simulationHasBeenLaunched)
        {
            if (isActive)
                _renderer.material.SetTexture("_MainTex", active_lift);
            else
                _renderer.material.SetTexture("_MainTex", unactive_lift);
        }
    }

    public void SetInitialState()
    {
        transform.position = initialPos;
        isActive = true;
        nextActiveTurn = 0;
        _renderer.material.SetTexture("_MainTex", active_lift);
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
                _renderer.material.SetTexture("_MainTex", unactive_lift);
                gameObject.tag = "Blank Tile";
                StartCoroutine(BlinkOverSeconds(Color.grey, unactiveTimeColorSwap, true));
                StartCoroutine(MoveOverSeconds(gameObject, above_AdjacentPos, 0.2f));
                isActive = false;
            }
            else if (unactiveTurns == 0)
            {
                StartCoroutine(BlinkOverSeconds(Color.green, unactiveTimeColorSwap, true));
                StartCoroutine(MoveOverSeconds(gameObject, above_AdjacentPos, 0.2f));
            }
        }
        else if (!isActive)
        {
            if (canBeActivatedAgain && GameManager.currentTurn >= nextActiveTurn)
            {
                nextActiveTurn = 0;
                _renderer.material.SetTexture("_MainTex", active_lift);
                gameObject.tag = tagWhenActive;
                StartCoroutine(BlinkOverSeconds(Color.green, reactiveTimeColorSwap, true));
                isActive = true;
                if (goBackDownAfterUse)
                {
                    StartCoroutine(MoveOverSeconds(gameObject, initialPos, 0.2f));
                }
            }
            else if (!canBeActivatedAgain && goBackDownAfterUse)
            {
                StartCoroutine(MoveOverSeconds(gameObject, initialPos, 0.2f));
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

    IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 endPos, float seconds)     // original code : https://answers.unity.com/questions/296347/move-transform-to-target-in-x-seconds.html
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        //AudioManager.instance.Play("ig cube move mounted");
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPos, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            if (!GameManager.simulationIsRunning)
                objectToMove.transform.position = endPos;
            yield return null;
        }
        if (GameManager.simulationIsRunning)
            objectToMove.transform.position = endPos;
        else
            transform.position = endPos;
    }

    public void FirstInitialization()
    {
        initialPos = transform.position;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
    }

    IEnumerator AppearingAnimation(float startingOffset, float duration, float minDelay, float maxDelay, float timeToWaitBeforeFirstInitialization)
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
