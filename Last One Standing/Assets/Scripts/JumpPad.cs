using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private float springCounter;
    public SpriteRenderer jumpSR;
    public Sprite downSprite, upSprite;
    private bool isPressed;

    void Update()
    {
        //countdown until the jump pad should change back to the downSprite
        if(springCounter>0)
        {
            springCounter -= Time.deltaTime;
            
            if(springCounter<=0)
            {
                jumpSR.sprite=downSprite;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)//when a player jumps on the button, animate the button to the down sprite (and set the counter for it to reset to the upsprite), and make the player launch into the air with a SFX
    {
        
        jumpSR.sprite = downSprite;
        springCounter = 1f;
        isPressed = true;

        if(other.tag == "Player")
        {
            Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
            rb2d.velocity = new Vector2(rb2d.velocity.x,30);
            //bounce up in the air, leaving x unchanged

            AudioManager.instance.PlaySFX(6);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    //if the player steps off the button, reset the button to the "up" position regardless of the springCounter
    {
        if(other.tag == "Player" && isPressed)
        {
            isPressed = false;
            jumpSR.sprite = upSprite;
        }
    } 
}
