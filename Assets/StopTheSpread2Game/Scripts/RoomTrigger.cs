using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public List<Enemy> roomEnemies = new List<Enemy>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            foreach(Enemy enemy in roomEnemies)
            {
                enemy.Alert();
            }
        }
    }
}
