//If the enemy is too far away, kill it.
/*Please give credit if you use or modify this code
 * 
 *Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnActivation : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Spawner inside screen space");
    }

    //Called when Enemy is too far away
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Make sure its an enemy
        if (collision.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }
}
