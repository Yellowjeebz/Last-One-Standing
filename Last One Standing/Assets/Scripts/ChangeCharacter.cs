using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    public bool isPressed;
    private bool isActive;
    public float upWaitTime;
    public SpriteRenderer buttonSR;
    public Sprite buttonUp, buttonDown;
    private float countdownCounter;
    public AnimatorOverrideController newSpriteOverride; 
    public bool spriteInUse;
    public GameObject silhouette;
    public string spriteName; //the name of the sprite which the player is changing into

    public PlayerController playerToChange;
    
    void Update()
    //this is where the button will be reset to the "up" position, once the counter has reached 0
    {
        checkIfSpriteInUse(playerToChange);
        if(isPressed)
        //if the button is pressed, it starts the countdown from upWaitTime, minusing the equivalent of 1 second in game time each second
        {
            countdownCounter -= Time.deltaTime;
            if(countdownCounter<=0)
            {
                isPressed=false;
                buttonSR.sprite = buttonUp;
            }
        }
        if(spriteInUse)//if the sprite is in use, the silhouette object shows to signify it is in use
        {
            silhouette.SetActive(true);
        }
        else if(!spriteInUse)//when the sprite is no longer in use after being in use, the silhouette object disappears
        {
            silhouette.SetActive(false);
        }
    }

    private PlayerController OnTriggerEnter2D(Collider2D other)
    //this is used to make the button "Pressed"
    {    
        if(other.tag == "Player" && !isPressed)
        //if a player presses the button, their animator is changed to the animator of the button they press (unless it is in use).
        {
        playerToChange = other.GetComponent<PlayerController>();
        
        
            if(playerToChange.rb2d.velocity.y < -.1f)
            {
                if(spriteInUse == false)
                {
                    spriteInUse = true;
                    silhouette.SetActive(true);
                    playerToChange.playerAnimator.runtimeAnimatorController = newSpriteOverride;
                    playerToChange.rtAnimator = newSpriteOverride;//this is used to determine set a button as Active again, and resetting sprites after the ZP minigame
                    playerToChange.currentSprite = spriteName;
                }                
                isPressed = true;
                buttonSR.sprite = buttonDown;
                countdownCounter = upWaitTime;
                //Setting counter to the variable upWaitTime rather than a fixed number in order to adjust in the editor.
                AudioManager.instance.PlaySFX(2);
            }
        }
        
        return playerToChange;
    }

    private void OnTriggerExit2D(Collider2D other)
    //if the player steps off the button, reset the button to the "up" position regardless of the countdownCounter
    {
        if(other.tag == "Player" && isPressed)
        {
            isPressed = false;
            buttonSR.sprite = buttonUp;
        }

    } 

    private void checkIfSpriteInUse(PlayerController playerToCheck)
    //iterates through the current players when a button is pressed to check if the sprite is in use, if it isn't in use, then it is set to in use
    {
        for(int i=0;i<GameManager.gameManagerInstance.activePlayers.Count;i++)
        {
            if((GameManager.gameManagerInstance.activePlayers[i].currentSprite)==spriteName)
            {
                spriteInUse = true;
                break;
            }
            else spriteInUse = false;
            }
    }
}


