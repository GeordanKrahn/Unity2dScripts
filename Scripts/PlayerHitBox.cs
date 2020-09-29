/*
 * This script is responsible for controlling the Player Hit Box
 * Please give credit if you use or modify this code
 * 
 * Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    BoxCollider2D playerHitBox;
    public Transform rotationPoint;
    public Transform startPosition;
    public float rotationSpeed;
    [Range(0, 360)] public float degreesPerSecond;
    bool isEnabled = false;
    int damageToDeal = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerHitBox = GetComponent<BoxCollider2D>();
        DisableHitBox();
        damageToDeal = GetComponentInParent<Player>().damagePerHit;
    }

    //Disables the hit box.
    public void DisableHitBox()
    {
        playerHitBox.enabled = false;
        isEnabled = false;
    }

    //Enables the hit box and rotation - this may change depending on the speed of the attack, the animation, etc...
    public void EnableHitBox()
    {
        transform.localPosition = new Vector2(startPosition.localPosition.x, startPosition.localPosition.y);
        transform.rotation = Quaternion.identity;
        playerHitBox.enabled = true;
        isEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //If the hitbox is enabled, rotates the hitbox.
        if(isEnabled == true)
        {
            //rotates in appropriate direction
            bool facingDirection = GetComponentInParent<PlayerControl>().GetIsPlayerFacingLeft();
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

    //Handles the overlap event
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Make sure what is his is the enemy
        if(collision.tag == "Enemy")
        {
            //Verify the type - add this check condition for each enemy type
            if(collision.TryGetComponent<GroundEnemy>(out GroundEnemy groundEnemy))
            {
                //Apply damage
                groundEnemy.TakeDamage(damageToDeal);
            }
        }
    }
}
