//Spawners can be placed anywhere. 
//Set the max number of enemies to be spawned
//set the enemy to be spawns from the spawner
//only activates if the player is close enough
/*Please give credit if you use or modify this code
 *Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    int numberOfSpawns = 0; //current spawn count
    public const int MAX_SPAWNS = 1; //exposed to engine, determine the max number of enemies which can spawn at a time
    public GameObject enemyToSpawn; //The entity we are spawning

    //Updates and Returns the current number of spawns
    public int NumberOfSpawnsActive()
    {
        GroundEnemy[] enemies = FindObjectsOfType<GroundEnemy>();
        numberOfSpawns = enemies.Length;
        return numberOfSpawns;
    }

    //Spawns the entity
    public void SpawnEnemy()
    {
        Debug.Log("SpawnEnemy()");
        if (NumberOfSpawnsActive() < MAX_SPAWNS)
        {
            Debug.Log("Now Spawning");
            Instantiate(enemyToSpawn, transform.position, Quaternion.identity, transform);
        }
    }

    //If the player enters the activation zone, spawn the enemy('s)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpawnEnemy();
    }

    //called in SpawnActivation.cs
    public void DestroySpawn()
    {
        Destroy(this.gameObject);
    }
}
