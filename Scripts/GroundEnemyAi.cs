/*
 * This script is responsible for the AI of ground based enemies
 * checks and updates state
 * Please give credit if you use or modify this code
 * 
 * Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyAi : MonoBehaviour
{
    public Transform playerDetectionOrigin;
    public GameObject lineOfSight;
    float attackRange;
    float attackRangeBuffer;
    public float detectionRange;
    public float enemyWalkSpeed;
    public float searchTime;
    public float waitTime;
    bool facingLeft;
    bool isPlayerSighted = false;
    bool isMoving = false;
    public bool isWaiting = false;
    bool canBeHit = true;
    LayerMask playerLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        CheckFacingDirection();
        attackRange = GetComponent<GroundEnemy>().attackRange;
        attackRangeBuffer = GetComponent<GroundEnemy>().attackRangeBuffer;
        lineOfSight.GetComponent<CircleCollider2D>().radius = detectionRange;
    }

    //Returns whether facing left
    public bool GetIsFacingLeft()
    {
        return facingLeft;
    }

    //Check the facing direction. Initially called in Start()
    //Checks direction based on transform
    void CheckFacingDirection()
    {
        if(transform.localScale.x < 0)
        {
            facingLeft = true;
        }

        if(transform.localScale.x > 0)
        {
            facingLeft = false;
        }
        //Debug.Log("facing left: " + facingLeft);
    }

    //Check and update if enemy can be hit
    void CheckHitStatus()
    {
        canBeHit = GetComponent<GroundEnemy>().GetHitStatus();
    }

    //changes the facing direction
    public void TurnAround()
    {
        Debug.Log("Turn around");
        transform.localScale = new Vector3(-transform.localScale.x,
                                            transform.localScale.y,
                                            transform.localScale.z);
    }

    //Returns whether enemy is moving
    public bool GetIsEnemyMoving()
    {
        return isMoving;
    }

    //Moves the enemy in the specified direction
    public void MoveInDirection(Vector2 direction)
    {
        Debug.Log("Moving");
        transform.Translate(direction * enemyWalkSpeed * Time.deltaTime);
        isMoving = true;
    }

    //Returns whether enemy sees a player or not.
    public bool GetIsPlayerSighted()
    {
        return isPlayerSighted;
    }

    //While the player is in the enemy sight
    private void OnTriggerStay2D(Collider2D collision)
    {
        //If the enemy has been hit, return
        if (GetComponent<GroundEnemy>().GetIsEnemyHit())
        {
            return;
        }

        //if what is in sight is a player
        if(collision.tag == "Player")
        {
            
            Debug.Log("Player Found");
            isPlayerSighted = true;
            //If the enemy is already attacking, return
            if (GetComponent<GroundEnemy>().GetIsEnemyAttacking())
            {
                return;
            }
            StopAllCoroutines();
            Vector2 playerPosition = collision.GetComponent<Player>().transform.position;
            bool isInLineOfSight = lineOfSight.GetComponent<CircleCollider2D>().IsTouchingLayers(playerLayer);
            //if player is in sight, move toward the player
            if (isInLineOfSight)
            {
                playerPosition = collision.GetComponent<Player>().transform.position;
                //is the player to the left or right
                if (playerPosition.x < transform.position.x)
                {
                    CheckFacingDirection();
                    if (facingLeft)
                    {
                        MoveInDirection(Vector2.left);
                    }
                    else
                    {
                        TurnAround();
                    }
                }
                else if (playerPosition.x > transform.position.x)
                {
                    CheckFacingDirection();
                    if (!facingLeft)
                    {
                        MoveInDirection(Vector2.right);
                    }
                    else
                    {
                        TurnAround();
                    }
                }
            }
            else
            {
                isMoving = false;
            }

            //IF the enemy is close enough to attack the player
            if (Mathf.Abs(playerPosition.x - transform.position.x) < attackRange - attackRangeBuffer/* && Mathf.Abs(playerPosition.y - transform.position.y) < attackRange*/)
            {
                StartCoroutine(GetComponent<GroundEnemy>().EnemyAttack());
            }
        }
        else
        {
            Debug.Log("Player Not Found");
            isPlayerSighted = false;
        }
    }

    //Returns whether enemy is waiting
    public bool GetIsEnemyWaiting()
    {
        return isWaiting;
    }

    //Player exits enemy detection zone
    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerSighted = false;
    }

    //destroys this enemy
    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    // called once per physics update.
    void FixedUpdate()
    {
        CheckFacingDirection();
        CheckHitStatus();
        if (!isPlayerSighted)
        {
            if (!isWaiting)
            {
                Debug.Log("1: move forward");
                StartCoroutine(GetComponent<GroundEnemy>().MoveForwardForTime(searchTime));
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }
}
