using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;

    public int health;
    //public EnemyHealthBar enemyHealthBar;

    private NavMeshAgent navMeshAgent;
    private PlayerController player;

    //public KillCount killCount;

    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Dying
    }

    public EnemyState currentEnemyState = EnemyState.Idle;

    void Idle()
    {
        
    }

    void Chasing()
    {
        navMeshAgent.SetDestination(player.transform.position);
    }

    void Attacking()
    {
        
    }

    void Dying()
    {
        
    }

    void ChangeState(EnemyState newState)
    {
        switch(currentEnemyState)
        {
            case EnemyState.Dying :
                break;
            case EnemyState.Chasing :
               
                break;
            case EnemyState.Attacking :
                
                break;
            case EnemyState.Idle :
                
                break;
        }

        currentEnemyState = newState;
    }

    public void Alert()
    {
        if(currentEnemyState == EnemyState.Idle)
        {
            ChangeState(EnemyState.Chasing);
        }
    }
    


private void Awake()
    {
        health = maxHealth;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
        //enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    public void TakeDamage(int damageAmmount)
    {
        health -= damageAmmount;
        gameObject.GetComponentInChildren<EnemyHealthBar>().Decrease();
        if(health <= 0)
        {
            KillCount.instance.EnemyKilled();
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        switch(currentEnemyState)
        {
            case EnemyState.Dying : 
                Dying();
                break;
            case EnemyState.Chasing :
                Chasing();
                break;
            case EnemyState.Attacking :
                Attacking();
                break;
            case EnemyState.Idle :
                Idle();
                break;
        }
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     if(other.collider.CompareTag("Player"))
    //     {
    //         Debug.Log("touched player");
    //         player.playerHealth -= 10;
    //         player.healthText.text = player.playerHealth.ToString();
    //         if(player.playerHealth < 10)
    //         {
    //             player.transform.position = player.spawnPoint.position;
    //             player.transform.rotation = player.spawnPoint.rotation;
    //             
    //         }
    //     }
    // }
}
