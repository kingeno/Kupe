using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    public Texture2D defaultCursorTexture;
    public Texture2D cameraCursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Cursor.SetCursor(defaultCursorTexture, hotSpot, cursorMode);
    }

    private void Update()
    {
        if (!GameManager.gameIsPaused)
        {
            if (MainCamera.isFreeLookActive)
                Cursor.SetCursor(cameraCursorTexture, hotSpot, cursorMode);
            else if (!MainCamera.isFreeLookActive)
                Cursor.SetCursor(defaultCursorTexture, hotSpot, cursorMode);
        }
    }
}