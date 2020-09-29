/*
 * This script is responsible for updating the player
 * attack state.
 * Please give credit if you use or modify this code
 * 
 * Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerControl playerStatus;
    bool isAttacking = false;
    bool isDashAttacking = false;
    bool isAirAttacking = false;
    bool isWalking = false;
    bool isRunning = false;
    bool isJumping = false;
    bool isOnGround = false;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GetComponent<PlayerControl>();
    }

    //Check player states
    void CheckStates()
    {
        //checks the player control states
        isWalking = playerStatus.IsPlayerWalking();
        isRunning = playerStatus.IsPlayerRunning();
        isJumping = playerStatus.IsPlayerJumping();
        isOnGround = playerStatus.IsPlayerGrounded();
    }

    //Return whether player is already attacking
    public bool IsPlayerAttacking()
    {
        return isAttacking;
    }

    //return whether player is air attacking
    public bool IsPlayerAirAttacking()
    {
        return isAirAttacking;
    }

    //returns whether player is dash attacking
    public bool IsPlayerDashAttacking()
    {
        return isDashAttacking;
    }

    //Handles the attack input and updates the attack states
    void CheckPlayerAttackStatus()
    {
        //If the player is pressing the attack button
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            //if on ground
            if (isOnGround)
            {
                //if running
                if (isRunning && !isWalking)
                {
                    //if not already attacking, attack
                    if (!isDashAttacking)
                    {
                        isDashAttacking = true;
                        isAttacking = false;
                        isAirAttacking = false;
                    }
                }
                else if (!isRunning && isWalking) //regular attack
                {
                    //if not already attacking, attack
                    if (!isAttacking)
                    {
                        isDashAttacking = false;
                        isAttacking = true;
                        isAirAttacking = false;
                    }
                }
                else if(!isRunning && !isWalking) //idle attack
                {
                    //if not already attacking, attack
                    if (!isAttacking)
                    {
                        isDashAttacking = false;
                        isAttacking = true;
                        isAirAttacking = false;
                    }
                }
            }
            else if (!isOnGround) //if in the air
            {
                if (isJumping)
                {
                    //if not already attacking, attack
                    if (!isAirAttacking)
                    {
                        isDashAttacking = false;
                        isAttacking = false;
                        isAirAttacking = true;
                    }
                }
            }
        }
        else //Not attacking
        {
            isDashAttacking = false;
            isAttacking = false;
            isAirAttacking = false;
        }
    }

    //Update called once per frame
    private void Update()
    {
        CheckStates();
        CheckPlayerAttackStatus();
    }
}
