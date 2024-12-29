using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb2d;//reference to the physics of the player
    public float xSpeed;
    public float ySpeed;
    private float velocity;//stores the input from the input device. this will be applied to the velocity of the rb
    private bool isGrounded;
    public Transform checkForGround;
    public LayerMask groundChecker;
    public Transform groundCheckerTransform;
    public bool isKeyboardPlayer2;
    public Animator playerAnimator;//reference to the animator of the player
    public string currentSprite;
    public RuntimeAnimatorController rtAnimator;
    public AnimatorOverrideController zombieORCompare;
    
    public int playerID;
    public string playerName;
    public bool isPlayer1Keyboard, isPlayer2Keyboard, isPlayer3Colour, isPlayer4Black;

    public int playerScore;
    public bool isWinner;
    
    public float powerUpCounter;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.gameManagerInstance.AddPlayers(this);
        //adds each new player to the list held by the gamemanager
        playerID = Login.LoginInstance.lastUserLoggedInID;//sets the playerID to the most recent ID used to log in
        transform.position = Vector3.zero;

    }
    // Update is called once per frame
    void Update()
    {
        if(isKeyboardPlayer2)
        {        
            velocity = 0f;
            if(Keyboard.current.lKey.isPressed)//walk right
            {
                velocity +=1f;
            }
            if(Keyboard.current.jKey.isPressed)//walk left
            {
                velocity -=1f;
            }
            if(SceneManager.GetActiveScene().name != "FallingFruit1")
            {
                if(isGrounded && Keyboard.current.iKey.wasPressedThisFrame/*Input.GetButtonDown("Jump")*/)//makes the person jump
                {
                    
                    rb2d.velocity = new Vector2(rb2d.velocity.x, ySpeed);
                }
                if(!isGrounded && Keyboard.current.iKey.wasReleasedThisFrame && rb2d.velocity.y > 0)//makes the person fall
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * .5f); 
                }
            }
            /*player 2 attack*/
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                playerAnimator.SetTrigger("attack");
                AudioManager.instance.PlaySFX(0);
            }
            rb2d.velocity = new Vector2(Input.GetAxis("Horizontal") * xSpeed,rb2d.velocity.y);     
        }
        
        rb2d.velocity = new Vector2(velocity * xSpeed * xSpeed,rb2d.velocity.y);//walking 
        /*the velocity of the rigidbody of the player will be updated to increase by the xSpeed, 
        while the velocity will unchanged at set at its current value*/
        isGrounded = Physics2D.OverlapCircle(checkForGround.position, .2f, groundChecker);


        //sets the animator parameters. left of the comma: parameter to set, right of the comma: value to give the parameter
        playerAnimator.SetFloat("Speed",Mathf.Abs(rb2d.velocity.x)); 
        playerAnimator.SetFloat("ySpeed", rb2d.velocity.y);
        playerAnimator.SetBool("isGrounded", isGrounded);
    
        if((SceneManager.GetActiveScene().name == "CharacterSelect") && rb2d.position.y < -9)
        {
            transform.position = Vector3.zero;
        }

        if(rb2d.velocity.x < 0){//flipping
            transform.localScale = new Vector3(-1f,1f,1f);//1f means unchanged
        }else if (rb2d.velocity.x >0)
        {
            transform.localScale = Vector3.one;
        }   
        if(powerUpCounter>0)//counts down the powerup counter every second
        {
            powerUpCounter -= Time.deltaTime;
            if(powerUpCounter <= 0)
            {
                xSpeed = 4;//4 is the perfect default speed
                rb2d.gravityScale = 5;
            }
        }
        if(playerAnimator.runtimeAnimatorController == zombieORCompare)//fixes an error which makes the zombie animator override float
        {
            gameObject.GetComponent<Collider2D>().offset = new Vector2(0.01923406f, 0f);
            groundCheckerTransform.localPosition = new Vector2(0f, -0.76f);
        }
        else //resets the player's positioning once they are reset to their original sprite after being a zombie
        {
            gameObject.GetComponent<Collider2D>().offset = new Vector2(0.01923406f, -0.3991069f);
            groundCheckerTransform.localPosition = new Vector2(0f, -1.011f);
        }

    }
    public void walk(InputAction.CallbackContext context)//function used to make the player move
    {
        velocity = context.ReadValue<Vector2>().x;//reads the x value from the velocity variable
    }
    
    void jump(InputAction.CallbackContext context)
    {
        if(SceneManager.GetActiveScene().name != "FallingFruit1")//jumping not allowed in falling fruit
        {
            if(isGrounded && context.started)//if jump button pressed and on the ground,
            rb2d.velocity = new Vector2(rb2d.velocity.x, ySpeed);//jump

            if(!isGrounded && context.canceled && rb2d.velocity.y > 0f)//when button is released
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y*.5f);//smaller jump
            }
        } else
        {
            Debug.Log("Jumping cancelled - current minigame is falling fruit");
        }
    }

    void attack(InputAction.CallbackContext context)
    {
        playerAnimator.SetTrigger("attack");/*changes the animation to attack. 
        don't pass any value because it will just revert back anyway*/
        AudioManager.instance.PlaySFX(0);
    }
}
