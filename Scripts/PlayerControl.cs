/* This script is responsible for player control
 * This is where the movement input is implemented
 * 
 * Please give credit if you use or modify this code
 * 
 * Created by Geordan Krahn
 */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float playerWalkSpeed;
    float initialPlayerWalkSpeed;
    public float playerRunSpeed;
    float initialPlayerRunSpeed;
    float airSpeed;
    public Vector2 jumpForce;
    bool isOnGround = true;
    bool isWalking = false;
    bool isRunning = false;
    bool isJumping = false;
    bool isFacingLeft = false;
    bool isTouchingWall = false;
    bool isTouchingEnemy = false;
    public float pushForce;
    Vector2 pushRight;
    Vector2 pushLeft;
    LayerMask groundLayer;
    LayerMask wallLayer;
    LayerMask enemyLayer;
    PlayerHitBox hitBox;

    // Start is called before the first frame update
    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        wallLayer = LayerMask.GetMask("Wall");
        enemyLayer = LayerMask.GetMask("Enemy");
        initialPlayerWalkSpeed = playerWalkSpeed;
        initialPlayerRunSpeed = playerRunSpeed;
        pushLeft = new Vector2(-pushForce, 0);
        pushRight = new Vector2(pushForce, 0);
        hitBox = GetComponentInChildren<PlayerHitBox>();
    }

    //Handles speed while attacking
    public void ReduceSpeed(float factor)
    {
        playerWalkSpeed = playerWalkSpeed / factor;
        playerRunSpeed = playerRunSpeed / factor;
    }

    //resets the speed to its intial value
    public void ResetSpeed()
    {
        playerWalkSpeed = initialPlayerWalkSpeed;
        playerRunSpeed = initialPlayerRunSpeed;
    }

    //Returns player facing direction
    public bool GetIsPlayerFacingLeft()
    {
        return isFacingLeft;
    }

    //This attempts to corrects potential wall clipping
    //not meant to be a permanent solution
    void HandlePlayerTouchingWall()
    {
        //check if player is HandlePlayerTouchingWall a wall
        isTouchingWall = GetComponent<Rigidbody2D>().IsTouchingLayers(wallLayer);
        if (isTouchingWall)
        {
            //push againsts the facing direction
            if (isFacingLeft)
            {
                GetComponent<Rigidbody2D>().AddForce(pushRight, ForceMode2D.Force);
            }
            else if (!isFacingLeft)
            {
                GetComponent<Rigidbody2D>().AddForce(pushLeft, ForceMode2D.Force);
            }
        }
    }

    /* Redundant
     *
     * 
    void HandlePlayerTouchingEnemy()
    {
        //check if player is HandlePlayerTouchingWall a wall
        isTouchingEnemy = GetComponent<Rigidbody2D>().IsTouchingLayers(enemyLayer);
        if (isTouchingEnemy)
        {
            //push againsts the facing direction
            if (isFacingLeft)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(KnockbackForce.x, KnockbackForce.y), ForceMode2D.Impulse);
            }
            else if (!isFacingLeft)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(-KnockbackForce.x, KnockbackForce.y), ForceMode2D.Impulse);
            }
        }
    }
    */

    //Handles behavior in air
    void HandlePlayerInAir()
    {
        if (!isOnGround)
        {
            //sets air speed. See PlayerJump()
            if(airSpeed == 0)
            {
                airSpeed = playerWalkSpeed / 2;
            }
            airSpeed = Mathf.Abs(airSpeed);
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                transform.position = new Vector2(transform.position.x - (airSpeed * Time.deltaTime),
                                                    transform.position.y);
            }
            else if(Input.GetAxisRaw("Horizontal") > 0)
            {
                transform.position = new Vector2(transform.position.x + (airSpeed * Time.deltaTime),
                                                    transform.position.y);
            }
        }
    }

    //Checks whether player is on ground and updates state
    void CheckIsPlayerGrounded()
    {
        //Is the player currently on the ground?
        isOnGround = GetComponent<Rigidbody2D>().IsTouchingLayers(groundLayer);
        if (isOnGround)
        {
            isJumping = false;
        }
        else if (!isOnGround)
        {
            isJumping = true;
        }
    }

    //returns whether player is walking
    public bool IsPlayerWalking()
    {
        return isWalking;
    }

    //returns whether player is jumping
    public bool IsPlayerJumping()
    {
        return isJumping;
    }

    //returns whether player is running
    public bool IsPlayerRunning()
    {
        return isRunning;
    }

    //returns whether player is on ground
    public bool IsPlayerGrounded()
    {
        return isOnGround;
    }

    //handles changing direction
    public void ChangeDirection()
    {
        //What direction we are currently facing
        if (isFacingLeft)
        {
            isFacingLeft = false;
        }
        else if (!isFacingLeft)
        {
            isFacingLeft = true;
        }
        //Turn the character around
        transform.localScale = new Vector3(-transform.localScale.x,
                                            transform.localScale.y,
                                            transform.localScale.z);

        hitBox.transform.localScale = new Vector3(-transform.localScale.x,
                                            transform.localScale.y,
                                            transform.localScale.z);
    }

    //Handles input on movement
    void ControlPlayer()
    {
        if (GetComponent<Player>().GetIsPlayerHit())
        {
            return;
        }
        //Moving left or right
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (Input.GetKey(KeyCode.RightShift))
            {
                //running
                isWalking = false;
                isRunning = true;

                if (isOnGround)
                {
                    transform.position = new Vector2(transform.position.x + (playerRunSpeed * Time.deltaTime),
                                                    transform.position.y);
                }
                //if facing left then change direction
                if (isFacingLeft)
                {
                    ChangeDirection();
                }
            }
            else
            {
                //walking
                isRunning = false;
                isWalking = true;
                if (isOnGround)
                {
                    transform.position = new Vector2(transform.position.x + (playerWalkSpeed * Time.deltaTime),
                                                    transform.position.y);
                }
                //if facing left then change direction
                if (isFacingLeft)
                {
                    ChangeDirection();
                }
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            //moving left
            if (Input.GetKey(KeyCode.RightShift))
            {
                //running
                isWalking = false;
                isRunning = true;

                if (isOnGround)
                {
                    transform.position = new Vector2(transform.position.x - (playerRunSpeed * Time.deltaTime),
                                                    transform.position.y);
                }
                //if facing right then change direction
                if (!isFacingLeft)
                {
                    ChangeDirection();
                }
            }
            else
            {
                //walking
                isRunning = false;
                isWalking = true;
                if (isOnGround)
                {
                    transform.position = new Vector2(transform.position.x - (playerWalkSpeed * Time.deltaTime),
                                                    transform.position.y);
                }
                //if facing right then change direction
                if (!isFacingLeft)
                {
                    ChangeDirection();
                }
            }
        }
        else
        {
            //Not Walking
            isWalking = false;
            isRunning = false;
        }


        //jumping
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            //Are we on the ground?
            if (isOnGround)
            {
                //Are we not already jumping?
                if (!isJumping)
                {
                    //We are now jumping
                    isJumping = true;
                    //which direction are we faceing?
                    if (isFacingLeft)
                    {
                        //facing left
                        //are we moving
                        if(isWalking && !isRunning)
                        {
                            //walking
                            PlayerJump(-playerWalkSpeed);
                        }
                        else if(!isWalking && isRunning)
                        {
                            //running
                            PlayerJump(-playerRunSpeed);
                        }
                        else if(!isWalking && !isRunning)
                        {
                            //idle
                            PlayerJump(0);
                        }
                    }
                    else if (!isFacingLeft)
                    {
                        //facing right
                        //are we moving
                        if (isWalking && !isRunning)
                        {
                            //walking
                            PlayerJump(playerWalkSpeed);
                        }
                        else if (!isWalking && isRunning)
                        {
                            //running
                            PlayerJump(playerRunSpeed);
                        }
                        else if (!isWalking && !isRunning)
                        {
                            //idle
                            PlayerJump(0);
                        }
                    }
                }
            }
        }
    }

    //Jump applies an upward force
    void PlayerJump(float horizontalSpeed)
    {
        //sets the air speed. See HandlePlayerInAir()
        airSpeed = horizontalSpeed;
        GetComponent<Rigidbody2D>().AddForce(jumpForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    private void Update()
    {
        //Determine if player is on ground before control
        CheckIsPlayerGrounded();

        //Allow control of player
        ControlPlayer();
    }

    //Called once per physics update
    private void FixedUpdate()
    {
        HandlePlayerInAir();
        HandlePlayerTouchingWall();
    }
}
