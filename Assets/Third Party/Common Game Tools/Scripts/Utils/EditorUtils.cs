using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class EditorUtils
{
	public static void ThinLine(int i_height = 1)
	{
#if UNITY_EDITOR
		GUIStyle style = new GUIStyle(EditorStyles.label);
		style.fontSize = 4;
		GUILayout.Label("", style);
		Rect rect = EditorGUILayout.GetControlRect(false, i_height);
		rect.height = i_height;
		EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
		GUILayout.Label("", style);
#endif
	}
}
