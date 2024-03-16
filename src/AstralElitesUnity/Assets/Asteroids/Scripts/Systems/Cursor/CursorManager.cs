﻿using System.Collections.Generic;
using UnityEngine;

public static class CursorManager
{
    public static List<CursorStyle> Styles;
    public static CursorStyle CurrentStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnRuntimeMethodLoad()
    {
        Styles = new List<CursorStyle>(Resources.LoadAll<CursorStyle>("Cursor Styles/"));

        SetCursor(GetStyle("Default"));
    }

    public static void SetCursor(string style)
    {
        SetCursor(GetStyle(style));
    }

    public static void SetCursor(CursorStyle style)
    {
        Cursor.SetCursor(style.Graphic, style.Hotspot, CursorMode.Auto);
        CurrentStyle = style;
    }

    public static CursorStyle GetStyle(string name)
    {
        Styles ??= new List<CursorStyle>(Resources.LoadAll<CursorStyle>("Cursor Styles/"));

        foreach (var style in Styles)
        {
            if (style.name == name)
            {
                return style;
            }
        }
        return null;
    }
}
