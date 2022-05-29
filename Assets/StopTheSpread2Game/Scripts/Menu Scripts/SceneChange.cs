using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
	public void LoadScene(string SceneName)
	{
		SceneManager.LoadScene(SceneName);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			LoadScene("EndGame");
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		
	}
}
