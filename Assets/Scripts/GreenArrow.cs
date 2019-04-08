using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenArrow : MonoBehaviour
{

    public bool isActive;
    public bool canBeActivatedAgain;
    public int unactiveTurns;
    public int nextActiveTurn;

    public Transform[,,] tilesBoard;
    public TileSelectionSquare tileSelectionSquare;

    public Vector3 above_AdjacentPos;
    public Transform above_AdjacentTile;

    private Quaternion forwardArrow;
    private Quaternion backArrow;
    private Quaternion leftArrow;
    private Quaternion rightArrow;
    public string tileOrientation;
    public string tagWhenActive;

    private Renderer _renderer;
    public Texture active_greenArrow;
    public Texture unactive_greenArrow;
    public Texture arrowTileDeleteImpossible;

    public float unactiveTimeColorSwap;
    public float reactiveTimeColorSwap;

    public Color opaqueColor;
    public Color transparantColor;

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

    private bool disappearingAnimation_isFinished;

    void Start()
    {
        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        isActive = true;
        _renderer = GetComponent<Renderer>();

        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);

        tagWhenActive = "Green Arrow";

        if (transform.rotation == forwardArrow)
            tileOrientation = "Forward";
        else if (transform.rotation == rightArrow)
            tileOrientation = "Right";
        else if (transform.rotation == backArrow)
            tileOrientation = "Back";
        else if (transform.rotation == leftArrow)
            tileOrientation = "Left";

        _renderer.material.SetTexture("_MainTex", active_greenArrow);
        gameObject.tag = tagWhenActive;

        if (reactiveTimeColorSwap == 0)
            reactiveTimeColorSwap = 0.2f;
        if (unactiveTimeColorSwap == 0)
            unactiveTimeColorSwap = 0.3f;

        disappearingAnimation_isFinished = false;

        StartCoroutine(AppearingAnimation(startingOffset, duration, minDelay, maxDelay));
    }

    private void Update()
    {
        if (InGameUIManager.nextLevelIsLoading && !disappearingAnimation_isFinished)
        {
            StartCoroutine(DisappearingAnimation(dEndingOffset, dDuration, dMinDelay, dMaxDelay));
            disappearingAnimation_isFinished = true;
        }
    }

    private void OnMouseOver()
    {
        if (!GameManager.gameIsPaused && tileSelectionSquare.canBeMoved && !MainCamera.isFreeLookActive && GameManager.playerCanModifyBoard)
        {
            if (tileSelectionSquare.transform.position != transform.position)
            {
                AudioManager.instance.Play("ig tile hovering");
                tileSelectionSquare.transform.position = transform.position;
            }

            if (InGameUIManager.isDeleteTileSelected)
            {
                tileSelectionSquare.material.color = tileSelectionSquare.deleteColor;
                _renderer.material.SetTexture("_MainTex", arrowTileDeleteImpossible);
                if (Input.GetMouseButtonDown(0))
                    AudioManager.instance.Play("ig tile delete impossible");
            }
            else
            {
                tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
                if (isActive)
                    _renderer.material.SetTexture("_MainTex", active_greenArrow);
                else
                    _renderer.material.SetTexture("_MainTex", unactive_greenArrow);
            }
        }
    }

    private void OnMouseExit()
    {
        if (tileSelectionSquare.canBeMoved)
            tileSelectionSquare.transform.position = tileSelectionSquare.hiddenPosition;
        if (!GameManager.simulationHasBeenLaunched)
        {
            if (isActive)
                _renderer.material.SetTexture("_MainTex", active_greenArrow);
            else
                _renderer.material.SetTexture("_MainTex", unactive_greenArrow);
        }
    }

    public void SetInitialState()
    {
        isActive = true;
        nextActiveTurn = 0;
        _renderer.material.SetTexture("_MainTex", active_greenArrow);
        tag = tagWhenActive;
    }

    public void TurnInitializer()
    {
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        StateCheck();
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
            _renderer.material.SetTexture("_MainTex", unactive_greenArrow);
            gameObject.tag = "Blank Tile";
            StartCoroutine(BlinkOverSeconds(Color.grey, unactiveTimeColorSwap, true));
            isActive = false;
        }
        else if (canBeActivatedAgain && !isActive)
        {
            nextActiveTurn = 0;
            _renderer.material.SetTexture("_MainTex", active_greenArrow);
            gameObject.tag = tagWhenActive;
            StartCoroutine(BlinkOverSeconds(Color.green, reactiveTimeColorSwap, true));
            isActive = true;
        }
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

    public void FirstInitialization()
    {
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));
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
