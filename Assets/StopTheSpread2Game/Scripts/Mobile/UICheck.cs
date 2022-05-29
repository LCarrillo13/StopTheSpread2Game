using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICheck : MonoBehaviour
{
    public GameObject mobileUI;
    // Start is called before the first frame update
    void Start()
    {
        mobileUI.SetActive(Application.isMobilePlatform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
