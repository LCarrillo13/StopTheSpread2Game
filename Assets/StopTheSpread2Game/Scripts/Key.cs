using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

    [SerializeField]private GameObject door;
    [SerializeField]private GameObject thisKey;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     if(other.gameObject.CompareTag("Player"))
    //     {
    //         Destroy(door);
    //         Destroy(thisKey);
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(door);
            Destroy(thisKey);
        }
    }
}
