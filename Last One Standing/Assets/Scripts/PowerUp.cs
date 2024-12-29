using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool isTrophy, isHealthPu, isSpeedPu, isInvincPu, isJumpPu;//determines which powerup the object is, and therefore its functionality
    public GameObject pickupEffect;//used by all powerups, not just the trophy.
    public Transform thisPowerUpPos;
    private float time = 5f;

    void Start()
    {
        Debug.Log("Powerup spawned");
    }

    void Update()
    {
        if(thisPowerUpPos.position.y <= 7)//once the powerup spawns
        {
            
            if (time > 0) //count down from 2 seconds
            {
            time -= Time.deltaTime;
            } 
            else //once 2 seconds are up, delete the powerup
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerController playerThatSelectedThisPowerup = other.GetComponent<PlayerController>();
            if(isHealthPu)
            {
                //fill health
                other.GetComponent<PlayerMinigameController>().fillHealthPowerup();
            }
            if(isInvincPu)
            {
                //make invincible for a few seconds
                other.GetComponent<PlayerMinigameController>().invincibilePowerup();
            }
            if(isSpeedPu)
            {
                //increase the x speed velocity for a few seconds
                playerThatSelectedThisPowerup.xSpeed = 6;
                playerThatSelectedThisPowerup.powerUpCounter = 3f;
            }
            if(isJumpPu)
            {
                //decrease the gravity for a few seconds
                playerThatSelectedThisPowerup.rb2d.gravityScale = 1f;
                playerThatSelectedThisPowerup.powerUpCounter = 3f;   
            }
            if(isTrophy)
            {
                //increase number of trophies
                PlayerMinigameController playerThatSelectedThisTrophy = other.GetComponent<PlayerMinigameController>();
                playerThatSelectedThisTrophy.runningTrophyTotal++;

            }
            Instantiate(pickupEffect, transform.position, transform.rotation);
            AudioManager.instance.PlaySFX(9);
            Destroy(gameObject);
        }

    }
}