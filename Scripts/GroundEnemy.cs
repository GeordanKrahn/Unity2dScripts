/*
 * This script is responsible for controlling the animation state for ground enemies
 * Please give credit if you use or modify this code
 * 
 * Created by Geordan Krahn
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public int health = 10;
    public float KnockbackForce;
    public Transform attackPosition;
    public float attackRange;
    public float attackRangeBuffer;
    public float attackTime;
    public float attackTimeDelayFactor;
    public int damagePerHit;
    public float deathTime = 1;
    public float hitTime;
    float waitDuration;
    bool canBeHit = true;
    bool isHit = false;
    bool isMoving = false;
    bool isAttacking = false;
    Animator enemyAnimator;
    LayerMask playerLayer;
    GroundEnemyAi enemyAi;

    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        playerLayer = LayerMask.GetMask("Player");
        waitDuration = GetComponent<GroundEnemyAi>().waitTime;
        enemyAi = GetComponent<GroundEnemyAi>();
    }
    
    //update the enemy state on late update
    void LateUpdate()
    {
        CheckIsEnemyAlive();
        if (!GetIsEnemyAttacking())
        {
            if (!isHit)
            {
                CheckIsEnemyMoving();
            }
        }
        else if (GetIsEnemyAttacking())
        {
            isMoving = false;
        }
    }

    //return whether enemy is hit
    public bool GetHitStatus()
    {
        return canBeHit;
    }

    //returns whether enemy is attacking
    public bool GetIsEnemyAttacking()
    {
        return isAttacking;
    }

    //applies damage
    public void TakeDamage(int damage)
    {
        if (canBeHit)
        {
            Debug.Log("Hit!");
            health -= damage;
            StartCoroutine(TakeHit());
        }
    }

    //Animates enemy hit
    IEnumerator TakeHit()
    {
        canBeHit = false;
        isHit = true;
        isAttacking = false;
        isMoving = false;
        enemyAnimator.SetBool("IsAttacking", false);
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetBool("IsDead", false);
        enemyAnimator.SetBool("IsHit", true);
        enemyAnimator.Play("Hit");
        if(enemyAi.GetIsFacingLeft())
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(KnockbackForce, 1), ForceMode2D.Impulse);
        }
        else if(!enemyAi.GetIsFacingLeft())
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-KnockbackForce, 1), ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(hitTime);
        canBeHit = true;
        enemyAnimator.SetBool("IsAttacking", false);
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetBool("IsDead", false);
        enemyAnimator.SetBool("IsHit", false);
        enemyAnimator.Play("Idle");
        isHit = false;
    }

    //Animates enemy death
    IEnumerator EnemyDeath()
    {
        isHit = false;
        isMoving = false;
        isAttacking = false;
        enemyAnimator.SetBool("IsAttacking", false);
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetBool("IsHit", false);
        enemyAnimator.SetBool("IsDead", true);
        enemyAnimator.Play("Death");
        yield return new WaitForSeconds(deathTime);
        enemyAi.DestroyEnemy();

    }

    //attacks at the position
    IEnumerator HitPlayer(Transform attackPos, float range, float attackTime)
    {
        yield return new WaitForSeconds(attackTime);
        if (!isHit)
        {
            if (Physics2D.OverlapCircleAll(attackPos.position, range, playerLayer).Length > 0)
            {
                Collider2D[] playersToDamage =
                    Physics2D.OverlapCircleAll(attackPos.position,
                    range, playerLayer);
                for (int i = 0; i < playersToDamage.Length; i++)
                {
                    playersToDamage[i].GetComponent<Player>().TakeDamage(damagePerHit, enemyAi.GetIsFacingLeft());
                }
            }
        }
    }

    //Animates Enemy Attack
    public IEnumerator EnemyAttack()
    {
        isMoving = false;
        isHit = false;
        isAttacking = true;
        enemyAnimator.SetBool("IsAttacking", true);
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetBool("IsHit", false);
        enemyAnimator.SetBool("IsDead", false);
        enemyAnimator.Play("Attack");
        StartCoroutine(HitPlayer(attackPosition, attackRange, attackTime * attackTimeDelayFactor));
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        enemyAnimator.SetBool("IsAttacking", false);
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.SetBool("IsHit", false);
        enemyAnimator.SetBool("IsDead", false);
        StopCoroutine("Attack");
    }

    //Animates enemy moving
    public IEnumerator MoveForwardForTime(float duration)
    {
        if (enemyAi.GetIsEnemyWaiting())
        {
            StopCoroutine("MoveForwardForTime");
        }
        isMoving = true;
        isHit = false;
        isAttacking = false;
        
        Debug.Log("2: Moving");
        enemyAnimator.SetBool("IsAttacking", false);
        enemyAnimator.SetBool("IsHit", false);
        enemyAnimator.SetBool("IsDead", false);
        enemyAnimator.SetBool("IsWalking", true);
        enemyAnimator.Play("Walk");
        if (enemyAi.GetIsPlayerSighted())
        {
            Debug.Log("3a: Player Found");
            StopCoroutine("MoveForwardForTime");
        }
        if (enemyAi.GetIsFacingLeft())
        {
            Debug.Log("3b.1: Left");
            enemyAi.MoveInDirection(Vector2.left);
        }
        else if (!GetComponent<GroundEnemyAi>().GetIsFacingLeft())
        {
            Debug.Log("3b.2: Right");
            enemyAi.MoveInDirection(Vector2.right);
        }
        yield return new WaitForSeconds(duration);
        Debug.Log("4: Stopping");
        
        StartCoroutine(WaitThenTurn(waitDuration));
        StopCoroutine("MoveForwardForTime");
    }

    //Animates enemy idle state. At end turn around.
    IEnumerator WaitThenTurn(float duration)
    {
        StopCoroutine("MoveForwardForTime");
        enemyAnimator.SetBool("IsAttacking", false);
        enemyAnimator.SetBool("IsHit", false);
        enemyAnimator.SetBool("IsDead", false);
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.Play("Idle");
        enemyAi.isWaiting = true;
        Debug.Log("5: Waiting");
        isMoving = false;
        isHit = false;
        isAttacking = false;
        if (enemyAi.GetIsPlayerSighted())
        {
            Debug.Log("6a: PlayerFound");
            StopCoroutine("WaitThenTurn");
        }
        yield return new WaitForSeconds(duration);
        enemyAnimator.SetBool("IsAttacking", false);
        enemyAnimator.SetBool("IsHit", false);
        enemyAnimator.SetBool("IsDead", false);
        enemyAnimator.SetBool("IsWalking", false);
        enemyAnimator.Play("Idle");
        Debug.Log("6b: Start Turning");
        enemyAi.TurnAround();
        enemyAi.isWaiting = false;
        StopCoroutine("WaitThenTurn");
        StopAllCoroutines();
    }

    //Checks if enemy is alive. if dead, start death animation
    void CheckIsEnemyAlive()
    {
        if (health <= 0)
        {
            StartCoroutine(EnemyDeath());
        }
    }

    //Updates moving animation state
    void CheckIsEnemyMoving()
    {
        //If waiting, return
        if (enemyAi.GetIsEnemyWaiting())
        {
            return;
        }

        //if attacking, return
        if (isAttacking)
        {
            return;
        }

        //if can be hit, animate movement
        if (canBeHit)
        {
            isMoving = enemyAi.GetIsEnemyMoving();
            if (isMoving)
            {
                enemyAnimator.SetBool("IsAttacking", false);
                enemyAnimator.SetBool("IsHit", false);
                enemyAnimator.SetBool("IsDead", false);
                enemyAnimator.SetBool("IsWalking", true);
                enemyAnimator.Play("Walk");
            }
            else if (!isMoving)
            {
                enemyAnimator.SetBool("IsAttacking", false);
                enemyAnimator.SetBool("IsHit", false);
                enemyAnimator.SetBool("IsDead", false);
                enemyAnimator.SetBool("IsWalking", false);
                enemyAnimator.Play("Idle");
            }
        }
    }
    
    //Returns whether the enemy is hit
    public bool GetIsEnemyHit()
    {
        return isHit;
    }

    //Draws overlap colliders for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
    
}
