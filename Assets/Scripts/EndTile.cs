using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : MonoBehaviour
{

    public Transform[,,] tilesBoard;
    public Transform tileSelectionSquareTransform;
    public TileSelectionSquare tileSelectionSquare;

    public bool isValidated;
    public Vector3 above_AdjacentPos;
    public Transform above_AdjacentTile;
    public GameObject endTileParticleSystem;

    private Renderer _renderer;
    public Texture active;
    public Texture impossibleToDelete;

    public static int validatedTileNumber;

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
        _renderer = GetComponent<Renderer>();

        if (!tileSelectionSquareTransform)
            tileSelectionSquareTransform = GameObject.FindGameObjectWithTag("TileSelectionSquare").transform;

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        isValidated = false;
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
                _renderer.material.SetTexture("_MainTex", impossibleToDelete);
                if (Input.GetMouseButtonDown(0))
                    AudioManager.instance.Play("ig tile delete impossible");
            }
            else
            {
                tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
                _renderer.material.SetTexture("_MainTex", active);
            }
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log(validatedTileNumber);
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
    }

    public void TurnInitializer()
    {
        Debug.Log(name + " turn initializer");
        tilesBoard = BoardManager.updated_3DBoard;
        above_AdjacentTile = TileCheck(above_AdjacentPos);
        if (above_AdjacentTile && above_AdjacentTile.tag == "Cube")
        {
            isValidated = true;
            endTileParticleSystem.SetActive(false);
        }
        else if (!above_AdjacentTile)
        {
            isValidated = false;
            endTileParticleSystem.SetActive(true);
        }
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
