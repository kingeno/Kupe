using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POL_GreenArrow : MonoBehaviour {

    public bool isActive;
    public int unactiveTurns;
    private int nextActiveTurn;

    public bool startState;

    public GameObject boardManager;

    private Quaternion forwardArrow;
    private Quaternion backArrow;
    private Quaternion leftArrow;
    private Quaternion rightArrow;

    private Renderer _renderer;
    public Texture partOfLevel_active_greenArrow;
    public Texture partOfLevel_unactive_greenArrow;

    public string tagWhenActive;

    void Start()
    {

        boardManager = GameObject.FindGameObjectWithTag("BoardManager");
        _renderer = GetComponent<Renderer>();

        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);

        startState = true;

        if (transform.rotation == forwardArrow)
        {
            gameObject.tag = "Forward Arrow";
            tagWhenActive = "Forward Arrow";
        }
        if (transform.rotation == rightArrow)
        {
            gameObject.tag = "Right Arrow";
            tagWhenActive = "Right Arrow";
        }
        if (transform.rotation == backArrow)
        {
            gameObject.tag = "Back Arrow";
            tagWhenActive = "Back Arrow";
        }
        if (transform.rotation == leftArrow)
        {
            gameObject.tag = "Left Arrow";
            tagWhenActive = "Left Arrow";
        }
    }

    private void Update()
    {
        if (isActive)
        {
            _renderer.material.SetTexture("_MainTex", partOfLevel_active_greenArrow);
            gameObject.tag = tagWhenActive;
        }
        else if (!isActive)
        {
            StateSwitch();
            _renderer.material.SetTexture("_MainTex", partOfLevel_unactive_greenArrow);
            gameObject.tag = "Blank Tile";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StateSwitch();
        }
    }

    public void StateSwitch()
    {
        if (isActive)
        {
            nextActiveTurn = GameManager.currentTurn + unactiveTurns;
            isActive = false;
            Debug.Log(name + ": state switch function / is deactivated");
        }
        else if (!isActive && GameManager.currentTurn >= nextActiveTurn)
        {
            isActive = true;
            nextActiveTurn = 0;
        }
    }

    void OnGUI()
    {
        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;
        redStyle.fontSize = 18;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        int _nextActiveTurn = (nextActiveTurn - GameManager.currentTurn);

        if (!isActive && _nextActiveTurn <= 10)
        {   
            GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
            /*"active in " + */(_nextActiveTurn - 1).ToString()
            , redStyle);
        }
        else if (isActive && unactiveTurns <= 10)
        {
            GUI.Box(new Rect(x - 20.0f, y - 10.0f, 20.0f, 50.0f),
            /*"active in " + */(unactiveTurns - 1).ToString()
            , redStyle);
        }
    }
}
