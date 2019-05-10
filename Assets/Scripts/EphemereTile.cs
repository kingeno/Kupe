using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EphemereTile : MonoBehaviour
{

    private bool isActive;
    [Header("States Parameters")]
    public bool canBeActivatedAgain;
    public int unactiveTurns;
    private int nextActiveTurn;

    private Transform[,,] tilesBoard;
    private TileSelectionSquare tileSelectionSquare;

    private Vector3 above_AdjacentPos;
    private Transform above_AdjacentTile;

    private string tagWhenActive;

    private Renderer _renderer;

    private Color opaqueColor = new Color(1f, 1, 1, 1f);
    private Color transparantColor = new Color(1f, 1, 1, 0f);

    [Header("Textures")]
    public Texture active;
    public Texture impossibleToDelete;

    private float unactiveTimeColorSwap = 0.3f;
    private float reactiveTimeColorSwap = 0.2f;
    private float fadeOutTime = 0.3f;

    private Color emptyColor = new Color(0f, 0f, 0f, 0f);

    private bool disappearingAnimation_isFinished;

    private float initialVerticalPos;
    private float lastVerticalPos;
    private float appearingDelay_RelativeToPos;
    private float disappearingDelay_RelativeToPos;

    void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        isActive = true;
        gameObject.tag = tagWhenActive = "EphemereTile"; ;

        _renderer.material.SetTexture("_MainTex", active);

        initialVerticalPos = transform.position.y;
        appearingDelay_RelativeToPos = 0.1f + (initialVerticalPos * GameManager._timeBetweenWaves);

        StartCoroutine(AppearingAnimation(GameManager._startingOffset, GameManager._duration, appearingDelay_RelativeToPos, appearingDelay_RelativeToPos + GameManager._appearingWaveDuration, GameManager._timeToWaitBeforeFirstInitialization));
    }

    private void Update()
    {
        if (InGameUIManager.nextLevelIsLoading && !disappearingAnimation_isFinished && gameObject.tag != "EmptyTile")
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

            _renderer.material.SetTexture("_MainTex", active);
        }
    }

    private void OnMouseExit()
    {
        if (tileSelectionSquare.canBeMoved)
            tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;
        if (!GameManager.simulationHasBeenLaunched)
            _renderer.material.SetTexture("_MainTex", active);
    }

    public void SetInitialState()
    {
        isActive = true;
        nextActiveTurn = 0;
        _renderer.material.color = Color.white;
        _renderer.material.SetTexture("_MainTex", active);
        tag = tagWhenActive;
        transform.parent = GameObject.FindGameObjectWithTag("Board Manager").GetComponent<Transform>();
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
                //AudioManager.instance.Play("ig tile ephemere tile disappear");
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
        AudioManager.instance.Play("ig tile ephemere tile disappear");
        //Debug.Log("tile disappear sound");
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
        transform.parent = null;
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
