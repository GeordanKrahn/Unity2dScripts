/*
 * This script is responsible for updating the player
 * animation state.
 * Please give credit if you use or modify this code
 * 
 * Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject groundHitBox;
    [SerializeField] GameObject dashHitBox;
    [SerializeField] GameObject airHitBox;
    LayerMask enemyLayerMask;
    PlayerControl playerStatus;
    PlayerAttack playerAttackStatus;
    Animator playerAnimator;
    public Transform groundAttackPosition;
    public float groundAttackRange;
    public float groundAttackTime;
    [RangeAttribute(0.01f, 1.0f)] public float groundAttackDelayFactor;
    public Transform dashAttackPosition;
    public float dashAttackRange;
    public float dashAttackTime;
    [RangeAttribute(0.01f, 1.0f)] public float dashAttackDelayFactor;
    public Transform airAttackPosition;
    public float airAttackRange;
    public float airAttackTime;
    [RangeAttribute(0.01f, 1.0f)] public float airAttackDelayFactor;
    public float reduceSpeedFactor;
    public float reduceRunSpeedFactor;
    public int playerHealth;
    public int damagePerHit;
    public float hitTime;
    public float invincibilityTime;
    public Vector2 knockbackForce;
    bool isAttacking = false;
    bool isDashAttacking = false;
    bool isAirAttacking = false;
    bool isAnimated = false;
    bool isWalking = false;
    bool isRunning = false;
    bool isHit = false;
    bool isJumping = false;
    bool isOnGround = false;
    bool isInvincible = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
        playerStatus = GetComponent<PlayerControl>();
        playerAttackStatus = GetComponent<PlayerAttack>();
        playerAnimator = GetComponent<Animator>();
        enemyLayerMask = LayerMask.GetMask("Enemy");
    }

    //checks the player control states
    void CheckStates()
    {
        isWalking = playerStatus.IsPlayerWalking();
        isRunning = playerStatus.IsPlayerRunning();
        isJumping = playerStatus.IsPlayerJumping();
        isOnGround = playerStatus.IsPlayerGrounded();
    }

    //returns the player hit state
    public bool GetIsPlayerHit()
    {
        return isHit;
    }

    //Handles the player taking damage
    //called when enemy successfully attacks player hitbox by enemy
    public void TakeDamage(int damageToTake, bool knockBackDirection)
    {
        if (isInvincible)
        {
            return;
        }
        StopAllCoroutines();
        if (knockBackDirection)
        {
            if (playerStatus.GetIsPlayerFacingLeft() == knockBackDirection)
            {
                playerStatus.ChangeDirection();
            }
            GetComponent<Rigidbody2D>().AddRelativeForce(-knockbackForce, ForceMode2D.Impulse);
        }
        if (!knockBackDirection)
        {
            if (playerStatus.GetIsPlayerFacingLeft() == knockBackDirection)
            {
                playerStatus.ChangeDirection();
            }
            GetComponent<Rigidbody2D>().AddRelativeForce(knockbackForce, ForceMode2D.Impulse);
        }

        //apply the damage and animate
        playerHealth -= damageToTake;
        StartCoroutine(TakeHit());
    }

    //Animates player taking hit
    IEnumerator TakeHit()
    {
        isInvincible = true;
        isAttacking = false;
        isDashAttacking = false;
        isAirAttacking = false;
        isAnimated = false;
        isWalking = false;
        isRunning = false;
        isHit = true;
        playerAnimator.SetBool("IsWalking", false);
        playerAnimator.SetBool("IsRunning", false);
        playerAnimator.SetBool("IsJumping", false);
        playerAnimator.SetBool("IsAttacking", false);
        playerAnimator.SetBool("IsDashAttacking", false);
        playerAnimator.SetBool("IsAirAttacking", false);
        playerAnimator.SetBool("IsHit", true);
        playerAnimator.Play("TakeHit");
        yield return new WaitForSeconds(hitTime);
        isHit = false;
        GetComponent<PlayerControl>().ResetSpeed();
        playerAnimator.SetBool("IsWalking", false);
        playerAnimator.SetBool("IsRunning", false);
        playerAnimator.SetBool("IsJumping", false);
        playerAnimator.SetBool("IsAttacking", false);
        playerAnimator.SetBool("IsDashAttacking", false);
        playerAnimator.SetBool("IsAirAttacking", false);
        playerAnimator.SetBool("IsHit", false);
        playerAnimator.Play("Idle");
        StartCoroutine(HitInvincibility());
    }

    //Disables invincibility after a certain time
    IEnumerator HitInvincibility()
    {
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    //cehcks if player is attacking
    void CheckAttackStates()
    {
        isAttacking = playerAttackStatus.IsPlayerAttacking();
        isDashAttacking = playerAttackStatus.IsPlayerDashAttacking();
        isAirAttacking = playerAttackStatus.IsPlayerAirAttacking();
    }

    //Animates player by state
    void SetAnimationState()
    {
        //if already doing something, return
        if (isAnimated)
        {
            return;
        }

        //if hit, return
        if (isHit)
        {
            return;
        }

        //updates the animator based on control states
        if (isOnGround)
        {
            //if walking
            if (isWalking)
            {
                //if attacking
                if (isAttacking)
                {
                    StartCoroutine(GroundAttack());
                }
                else if (!isAttacking)
                {
                    playerAnimator.SetBool("IsWalking", true);
                    playerAnimator.SetBool("IsRunning", false);
                    playerAnimator.SetBool("IsJumping", false);
                    playerAnimator.SetBool("IsAttacking", false);
                    playerAnimator.SetBool("IsDashAttacking", false);
                    playerAnimator.SetBool("IsAirAttacking", false);
                    playerAnimator.SetBool("IsHit", false);
                    playerAnimator.Play("Walk");
                }
            }
            else if (isRunning) //if running
            {
                //if attacking
                if (isDashAttacking)
                {
                    StartCoroutine(DashAttack());
                }
                else if (!isDashAttacking) // just running
                {
                    playerAnimator.SetBool("IsWalking", false);
                    playerAnimator.SetBool("IsRunning", true);
                    playerAnimator.SetBool("IsJumping", false);
                    playerAnimator.SetBool("IsAttacking", false);
                    playerAnimator.SetBool("IsDashAttacking", false);
                    playerAnimator.SetBool("IsAirAttacking", false);
                    playerAnimator.SetBool("IsHit", false);
                    playerAnimator.Play("Run");
                }
            }
            else
            {
                //if attacking
                if (isAttacking)
                {
                    StartCoroutine(GroundAttack());
                }
                else if (!isAttacking) // not doing anything
                {
                    playerAnimator.SetBool("IsWalking", false);
                    playerAnimator.SetBool("IsRunning", false);
                    playerAnimator.SetBool("IsJumping", false);
                    playerAnimator.SetBool("IsAttacking", false);
                    playerAnimator.SetBool("IsDashAttacking", false);
                    playerAnimator.SetBool("IsAirAttacking", false);
                    playerAnimator.SetBool("IsHit", false);
                    playerAnimator.Play("Idle");
                }
            }
        }
        else if (!isOnGround) // if in the air
        {
            //if attacking
            if (isAirAttacking)
            {
                StartCoroutine(AirAttack());
            }
            else if (!isAirAttacking) // just jumping
            {
                playerAnimator.SetBool("IsWalking", false);
                playerAnimator.SetBool("IsRunning", false);
                playerAnimator.SetBool("IsJumping", true);
                playerAnimator.SetBool("IsAttacking", false);
                playerAnimator.SetBool("IsDashAttacking", false);
                playerAnimator.SetBool("IsAirAttacking", false);
                playerAnimator.SetBool("IsHit", false);
                playerAnimator.Play("Jump");
            }
        }
    }

    //enable the players attack box
    IEnumerator Attack(float attackTime, GameObject hitBox)
    {
        hitBox.GetComponent<PlayerHitBox>().EnableHitBox();
        yield return new WaitForSeconds(attackTime);
        hitBox.GetComponent<PlayerHitBox>().DisableHitBox();
        
        /* Obsolete since i came up with better hit detection
         * 
        if(Physics2D.OverlapCircleAll(attackPos.position, range, enemyLayerMask).Length > 0)
        {
            Collider2D[] enemiesToDamage =
                Physics2D.OverlapCircleAll(attackPos.position,
                range, enemyLayerMask);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                enemiesToDamage[i].GetComponent<GroundEnemy>().TakeDamage(damagePerHit);
            }
        }
        */

        playerStatus.ResetSpeed();
    }

    //Delay before ground attack
    IEnumerator GroundAttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Attack(groundAttackTime - delay, groundHitBox));
    }

    //Delay before dash attack
    IEnumerator DashAttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Attack(dashAttackTime - delay, dashHitBox));
    }

    //Delay before air attack
    IEnumerator AirAttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Attack(airAttackTime - delay, airHitBox));
    }

    //Animate Ground Attack
    IEnumerator GroundAttack()
    {
        isAnimated = true;
        playerStatus.ReduceSpeed(reduceSpeedFactor);
        playerAnimator.SetBool("IsWalking", false);
        playerAnimator.SetBool("IsRunning", false);
        playerAnimator.SetBool("IsJumping", false);
        playerAnimator.SetBool("IsAttacking", true);
        playerAnimator.SetBool("IsDashAttacking", false);
        playerAnimator.SetBool("IsAirAttacking", false);
        playerAnimator.SetBool("IsHit", false);
        playerAnimator.Play("GroundAttack");
        StartCoroutine(GroundAttackDelay(groundAttackTime * groundAttackDelayFactor));
        yield return new WaitForSeconds(groundAttackTime);
        playerStatus.ResetSpeed();
        isAnimated = false;
        StopCoroutine("Attack");
    }

    //Animate Dash Attack
    IEnumerator DashAttack()
    {
        isAnimated = true;
        playerStatus.ReduceSpeed(reduceRunSpeedFactor);
        playerAnimator.SetBool("IsWalking", false);
        playerAnimator.SetBool("IsRunning", true);
        playerAnimator.SetBool("IsJumping", false);
        playerAnimator.SetBool("IsAttacking", false);
        playerAnimator.SetBool("IsDashAttacking", true);
        playerAnimator.SetBool("IsAirAttacking", false);
        playerAnimator.SetBool("IsHit", false);
        playerAnimator.Play("DashAttack");
        StartCoroutine(DashAttackDelay(dashAttackTime * dashAttackDelayFactor));
        yield return new WaitForSeconds(dashAttackTime);
        playerStatus.ResetSpeed();
        isAnimated = false;
    }

    //Animate Air Attack
    IEnumerator AirAttack()
    {
        isAnimated = true;
        playerStatus.ReduceSpeed(1);
        playerAnimator.SetBool("IsWalking", false);
        playerAnimator.SetBool("IsRunning", false);
        playerAnimator.SetBool("IsJumping", true);
        playerAnimator.SetBool("IsAttacking", true);
        playerAnimator.SetBool("IsDashAttacking", false);
        playerAnimator.SetBool("IsAirAttacking", true);
        playerAnimator.SetBool("IsHit", false);
        playerAnimator.Play("AirAttack");
        StartCoroutine(AirAttackDelay(airAttackTime * airAttackDelayFactor));
        yield return new WaitForSeconds(airAttackTime);
        playerStatus.ResetSpeed();
        isAnimated = false;
    }

    //Draws the colliders in engine for debugging
    void OnDrawGizmosSelected()
    {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundAttackPosition.position, groundAttackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(dashAttackPosition.position, dashAttackRange);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(airAttackPosition.position, airAttackRange);
    }
    private void Update()
    {
        //current animation
        CheckStates();
        CheckAttackStates();
        SetAnimationState();
    }
}
