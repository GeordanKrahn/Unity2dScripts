/*
 * This script is responsible for controlling the Enemy Hit Box
 * Please give credit if you use or modify this code
 * 
 * Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    BoxCollider2D enemyHitBox;
    public Transform rotationPoint;
    public Transform startPosition;
    public float rotationSpeed;
    [Range(0, 360)] public float degreesPerSecond;
    bool isEnabled = false;
    int damageToDeal = 1;

    // Start is called before the first frame update
    void Start()
    {
        enemyHitBox = GetComponent<BoxCollider2D>();
        DisableHitBox();
        damageToDeal = GetComponentInParent<Player>().damagePerHit;
    }

    //Disables the enemy attack box
    public void DisableHitBox()
    {
        enemyHitBox.enabled = false;
        isEnabled = false;
    }

    //Activates the enemy attack box and initialize rotation.
    public void EnableHitBox()
    {
        transform.localPosition = new Vector2(startPosition.localPosition.x, startPosition.localPosition.y);
        transform.rotation = Quaternion.identity;
        enemyHitBox.enabled = true;
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //If the hit box is enabled, rotate in appropriate direction.
        if (isEnabled == true)
        {
            bool facingDirection = GetComponentInParent<GroundEnemyAi>().GetIsFacingLeft();
            if (facingDirection)
            {
                transform.RotateAround(rotationPoint.position, Vector3.back, -degreesPerSecond * rotationSpeed * Time.deltaTime);
            }
            if (!facingDirection)
            {
                transform.RotateAround(rotationPoint.position, Vector3.back, degreesPerSecond * rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //make sure what is being hit is the player
        if (collision.tag == "Player")
        {
            //verify its a player then attack
            if (collision.TryGetComponent<Player>(out Player player))
            {
                //apply damage
                bool facingDirection = GetComponentInParent<GroundEnemyAi>().GetIsFacingLeft();
                player.TakeDamage(damageToDeal, facingDirection);
            }
        }
    }
}
