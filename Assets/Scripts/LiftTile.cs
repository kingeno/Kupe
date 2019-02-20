using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftTile : MonoBehaviour {

    public bool isActive;
    public bool canBeActivatedAgain;
    public bool goBackDownAfterUse;
    public int unactiveTurns;
    public int nextActiveTurn;

    public Transform[,,] tilesBoard;
    public TileSelectionSquare tileSelectionSquare;

    public Vector3 initialPos;
    public Vector3 above_AdjacentPos;
    public Transform above_AdjacentTile;

    public string tagWhenActive;

    private Renderer _renderer;
    public Texture active_lift;
    public Texture unactive_lift;
    public Texture liftTileDeleteImpossible;

    public float unactiveTimeColorSwap;
    public float reactiveTimeColorSwap;

    void Start () {

        if (!tileSelectionSquare)
            tileSelectionSquare = GameObject.FindGameObjectWithTag("TileSelectionSquare").GetComponent<TileSelectionSquare>();

        isActive = true;
        _renderer = GetComponent<Renderer>();

        tagWhenActive = "LiftTile";

        _renderer.material.SetTexture("_MainTex", active_lift);
        gameObject.tag = tagWhenActive;

        initialPos = transform.position;
        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));

        if (reactiveTimeColorSwap == 0)
            reactiveTimeColorSwap = 0.2f;
        if (unactiveTimeColorSwap == 0)
            unactiveTimeColorSwap = 0.3f;
    }

    private void OnMouseOver()
    {
        if (tileSelectionSquare.canBeMoved)
        {
            tileSelectionSquare.transform.position = transform.position;
            if (InGameUIManager.isDeleteTileSelected)
            {
                tileSelectionSquare.material.color = tileSelectionSquare.deleteColor;
                _renderer.material.SetTexture("_MainTex", liftTileDeleteImpossible);
            }
            else
            {
                tileSelectionSquare.material.color = tileSelectionSquare.defaultColor;
                if (isActive)
                    _renderer.material.SetTexture("_MainTex", active_lift);
                else
                    _renderer.material.SetTexture("_MainTex", unactive_lift);
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
}
