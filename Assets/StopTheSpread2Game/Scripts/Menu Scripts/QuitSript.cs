using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitSript : MonoBehaviour
{
	public void Close()
	{
		Application.Quit();
	#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
	#endif
	}
}
