using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	    Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private void Awake() => DontDestroyOnLoad(this.gameObject);
    

    // Update is called once per frame
    void Update()
    {
	    Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    
    
}
