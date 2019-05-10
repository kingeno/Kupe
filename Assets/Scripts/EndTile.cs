using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : MonoBehaviour
{

    private Transform[,,] tilesBoard;
    private Transform tileSelectionSquareTransform;
    private TileSelectionSquare tileSelectionSquare;

    [HideInInspector] public bool isValidated;
    private Vector3 above_AdjacentPos;
    private Transform above_AdjacentTile;

    private Renderer _renderer;

    [Header("Particle System")]
    public GameObject endTileParticleSystem;
    public new ParticleSystem particleSystem;

    public static int validatedTileNumber;

    private Color opaqueColor = new Color(1f, 1, 1, 1f);
    private Color transparantColor = new Color(1f, 1, 1, 0f);

    private bool disappearingAnimation_isFinished;

    private float repeatTime;
    private int repeatCount;
    private bool firstTwinkleEmission;

    [Space]
    [Header("Textures")]
    public Texture active;
    public Texture impossibleToDelete;

    private float initialVerticalPos;
    private float lastVerticalPos;
    private float appearingDelay_RelativeToPos;
    private float disappearingDelay_RelativeToPos;


    void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        if (!tileSelectionSquareTransform)
            tileSelectionSquareTransform = GameObject.FindGameObjectWithTag("TileSelectionSquare").transform;

        isValidated = false;
        disappearingAnimation_isFinished = false;

        repeatTime = particleSystem.emission.GetBurst(0).repeatInterval;
        repeatCount = 0;

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

        if (!GameManager.gameIsPaused && endTileParticleSystem.activeSelf && GameManager.currentSceneTime > particleSystem.emission.GetBurst(0).time && repeatCount < particleSystem.emission.GetBurst(0).cycleCount)
        {
            repeatTime -= Time.unscaledDeltaTime;

            if (!firstTwinkleEmission)
            {
                AudioManager.instance.Play("ig tile end tile twinkle");
                firstTwinkleEmission = true;
                repeatCount++;
            }
            else if (firstTwinkleEmission && repeatTime <= 0f)
            {
                AudioManager.instance.Play("ig tile end tile twinkle");
                repeatTime = particleSystem.emission.GetBurst(0).repeatInterval;
                if (repeatCount == particleSystem.emission.GetBurst(0).cycleCount)
                    endTileParticleSystem.SetActive(false);
                else
                    repeatCount++;
            }
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
        isValidated = false;
        tilesBoard = BoardManager.updated_3DBoard;
        endTileParticleSystem.SetActive(true);
        //repeatCount = 0;
        //firstTwinkleEmission = false;
    }

    public void TurnInitializer()
    {
        //Debug.Log(name + " turn initializer");
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        if (above_AdjacentTile && above_AdjacentTile.tag == "Cube")
        {
            isValidated = true;
            endTileParticleSystem.SetActive(false);
        }
        else
            isValidated = false;
    }

    //check the position of the tile relatively to the current cube position
    public Transform TileCheck(Vector3 tilePos)
    {
        //Debug.Log("run " + name + " Tile Check");
        Transform tile;
        if (tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z])
        {
            tile = tilesBoard[(int)tilePos.x, (int)tilePos.y, (int)tilePos.z];
            return tile;
        }
        else
        {
            return null;
        }
    }

    public void FirstInitialization()
    {
        tilesBoard = BoardManager.original_3DBoard;
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
