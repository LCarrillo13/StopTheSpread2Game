using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    private TextMeshPro tmpHealth;

    public Camera playerCam;
    // Start is called before the first frame update
    void Start()
    {
        
        //tm = GetComponent<TextMesh>();
        tmpHealth = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = playerCam.transform.forward;
    }
    
    // Return the current Health by counting the '-'
    public int Current() {
        return tmpHealth.text.Length;
    }

// Decrease the current Health by removing one '-'
    public void Decrease() {
        if (Current() > 1)
            tmpHealth.text = tmpHealth.text.Remove(tmpHealth.text.Length - 1);
        else
            Destroy(transform.parent.gameObject);
    }
}
