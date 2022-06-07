using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCount : MonoBehaviour
{
    public static KillCount instance;

    public Enemy enemy;
    public int temp;
    public int count;
    public bool perfectScore;


    public GameObject scoreText;
    // or 
    public Text scoreTextFormat;





    //TODO
    // pseudocode
    // count is 0 on start
    // every time Enemy hp hits 0, count++
    // end game, temp = count, count = 0, display temp


    //
    //
    //

    // null check
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
        // if(count > 1)
        // {
        //     temp = count;
        // }

        

        if(count == 57)
        {
            perfectScore = true;
        }
    }



#region Counter Functions

    public void EnemyKilled()
    {
        count++;
        scoreTextFormat.text = count.ToString();
    }
    
    
#endregion
}
