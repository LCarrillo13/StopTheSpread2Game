using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int currentLevel;
    public Scene firstScene;

    public Scene nextScene;
    public String currentSceneStr;

    private void Start()
    {
        if(SceneManager.GetActiveScene() == firstScene)
        {
            currentLevel = 1;
        }
    }

    private void Update()
    {
        if(currentLevel < 1)
        {
            currentLevel = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("original level is: " + currentLevel);
            
            LevelChecker();
            LoadScene(currentSceneStr);
            if(currentSceneStr == "EndGame")
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
        } 
    }

    public void LevelChecker()
    {
        
        if(currentLevel == 1)
        {
            ChangeLevel("Level2");
        }
        else if(currentLevel == 2)
        {
            ChangeLevel("Level3");
        }
        else if(currentLevel == 3)
        {
            ChangeLevel("Level4");
        }
        else if(currentLevel == 4)
        {
            ChangeLevel("Level5");
        }
        else if(currentLevel == 5)
        {
            ChangeLevel("EndGame");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    public void ChangeLevel(string LevelNum)
    {
        currentLevel++;
        currentSceneStr = LevelNum;
    }
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
