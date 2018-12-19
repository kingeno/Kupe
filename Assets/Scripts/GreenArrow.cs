using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenArrow : MonoBehaviour {

    public bool isActive;
    public bool canBeActivatedAgain;
    public int unactiveTurns;
    public int nextActiveTurn;

    public Transform[,,] tilesBoard;

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

    public float unactiveTimeColorSwap;
    public float reactiveTimeColorSwap;

    void Start()
    {
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

        above_AdjacentPos = (transform.position + new Vector3(0, 1, 0));

        if (reactiveTimeColorSwap == 0)
            reactiveTimeColorSwap = 0.2f;
        if (unactiveTimeColorSwap == 0)
            unactiveTimeColorSwap = 0.3f;
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
            StartCoroutine(BlinkOverSeconds(Color.grey, unactiveTimeColorSwap, false));
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

    //void OnGUI()
    //{
    //    GUIStyle redStyle = new GUIStyle();
    //    redStyle.normal.textColor = Color.red;
    //    redStyle.fontSize = 18;
    //    Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
    //    float x = screenPos.x;
    //    float y = Screen.height - screenPos.y;

    //    int _nextActiveTurn = (nextActiveTurn - GameManager.currentTurn);

    //    if (!isActive && _nextActiveTurn <= 10)
    //    {
    //        GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
    //        /*"active in " + */(_nextActiveTurn /*- 1*/).ToString()
    //        , redStyle);
    //    }
    //    else if (isActive && unactiveTurns <= 10)
    //    {
    //        GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
    //        /*"active in " + */(unactiveTurns /*- 1*/).ToString()
    //        , redStyle);
    //    }
    //}
}
