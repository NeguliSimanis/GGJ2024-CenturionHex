using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorAction
{
    walk,
    attack,
    error,
    undefined
}

[System.Serializable]
public class ActionCursor
{
    public CursorAction action;
    public Texture2D sprite;
}

public class CustomCursor_Simanis : MonoBehaviour
{
    public ActionCursor[] actionCursors;
    public static CustomCursor_Simanis instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetCursor(bool showCustomCursor, CursorAction cursorAction)
    {
        //Debug.Log("setting custom cursor");
        if (!showCustomCursor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true;
            //return;
        }
        foreach(ActionCursor cursor in actionCursors)
        {
            if (cursor.action == cursorAction && showCustomCursor)
            {
                Cursor.SetCursor(cursor.sprite, Vector2.zero, CursorMode.ForceSoftware);
            }
        }
    }
}
