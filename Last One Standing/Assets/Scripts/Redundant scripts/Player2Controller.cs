using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player2Controller : MonoBehaviour
{
    public Rigidbody2D rb2d;//reference to the physics of the player
    public float xSpeed;
    public float ySpeed;
    private float velocity;//stores the input from the input device. this will be applied to the velocity of the rb
    private bool isGrounded;
    public Transform checkForGround;
    public LayerMask groundChecker;
    private bool isKeyboardPlayer2;
    public Animator playerAnimator;//reference to the animator of the player
    public string currentSprite;
    
    void Start(){}
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(checkForGround.position, .2f, groundChecker);
        playerAnimator.SetFloat("Speed",Mathf.Abs(rb2d.velocity.x)); 
        playerAnimator.SetFloat("ySpeed", rb2d.velocity.y);
        playerAnimator.SetBool("isGrounded", isGrounded);
        
        velocity = 0f;
        if(Keyboard.current.lKey.isPressed)//walk right
        {
            velocity +=1f;
        }
        if(Keyboard.current.jKey.isPressed)//walk left
        {
            velocity -=1f;
        }
        if(isGrounded && Keyboard.current.iKey.wasPressedThisFrame/*Input.GetButtonDown("Jump")*/)//makes the person jump
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, ySpeed);
        }
        if(!isGrounded && Keyboard.current.iKey.wasReleasedThisFrame && rb2d.velocity.y > 0)//makes the person fall
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * .5f); 
        }
        /*player 2 attack*/
        if(Keyboard.current.enterKey.wasPressedThisFrame)
        {
            playerAnimator.SetTrigger("attack");
        }
        rb2d.velocity = new Vector2(Input.GetAxis("Horizontal") * xSpeed,rb2d.velocity.y);

        if(rb2d.velocity.x <0){
            transform.localScale = new Vector3(-1f,1f,1f);//1f means unchanged
        }
        else if (rb2d.velocity.x >0)
        {
            transform.localScale = Vector3.one;
        }        
    }
 }

