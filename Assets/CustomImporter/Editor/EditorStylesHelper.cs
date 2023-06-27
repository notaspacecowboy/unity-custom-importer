using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public static class EditorStylesHelper
{
    public static Color BackGroundColor = new Color32(38, 50, 56, 255);
    public static Color LightBlue = new Color32(79, 195, 247, 255);
    public static Color DarkBlue = new Color32(38, 50, 56, 255);
    public static Color LightTurquoise = new Color32(64, 224, 208, 255);
    public static Color DarkTurquoise = new Color32(0, 206, 209, 255);
    public static Color LightGray = new Color32(236, 239, 241, 255);
    public static Color DarkGray = new Color32(128, 128, 128, 255);
    public static Color LightGreen = new Color32(118, 201, 97, 255);
    public static Color DarkGreen = new Color32(61, 133, 64, 255);
    public static Color SuccessColor = new Color32(70, 171, 104, 255);
    public static Color LightRed = new Color32(255, 87, 87, 255);
    public static Color DarkRed = new Color32(200, 50, 50, 255);


    public static GUIStyle TitleStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            return style;
        }
    }

    public static GUIStyle LabelStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = LightGray;
            return style;
        }
    }

    public static GUIStyle TextFieldStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            style.fontSize = 12;
            style.normal.textColor = LightRed;
            return style;
        }
    }

    public static GUIStyle ToggleStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.toggle);
            style.normal.textColor = LightGray;
            return style;
        }
    }

    public static GUIStyle WarnButtonStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = 12;
            style.normal.textColor = LightRed;
            style.hover.textColor = DarkRed;
            return style;
        }
    }


    public static GUIStyle RegularButtonStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontSize = 12;
            style.normal.textColor = LightGreen;
            style.hover.textColor = DarkGreen;
            return style;
        }
    }

    public static GUIStyle ErrorLabelStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.red;
            return style;
        }
    }
    public static GUIStyle SuccessLabelStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = SuccessColor;
            return style;
        }
    }

    public static void HorizontalCenteredLabel(string label, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.Label(label, style, options);

        GUILayout.Space(10);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }


    public static void HorizontalCenteredButton(UnityAction callback, string text, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(text, style, options))
        {
            callback?.Invoke();
        }

        GUILayout.Space(10);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}