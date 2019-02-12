using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    public Texture2D defaultCursorTexture;
    public Texture2D cameraCursorTexture;
    public Texture2D deleteTileCursorTexture;
    public Texture2D greenArrowTileCursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

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
            {
                if (InGameUIManager.isGreenArrowSelected)
                    Cursor.SetCursor(greenArrowTileCursorTexture, hotSpot, cursorMode);
                else if (InGameUIManager.isDeleteTileSelected)
                    Cursor.SetCursor(deleteTileCursorTexture, hotSpot, cursorMode);
                else
                    Cursor.SetCursor(defaultCursorTexture, hotSpot, cursorMode);
            }
        }
        else
            Cursor.SetCursor(defaultCursorTexture, hotSpot, cursorMode);
    }
}