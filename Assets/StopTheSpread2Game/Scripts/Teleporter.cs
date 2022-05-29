using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // if origin spot is needed
    //public Transform originSpot;

    public Transform destinationSpot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    //other.transform.position = destinationSpot.transform.position;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = destinationSpot.transform.position;
    }
}
