using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpAttack : MonoBehaviour
{
    public float headBounceForce = 4f;
    public PlayerController attackingPlayer;
    public Rigidbody2D playerRb2d;
    
    private void OnTriggerEnter2D(Collider2D other)//other is the conventional variable here
    {
        if(other.tag == "Player" && playerRb2d.velocity.y <=0 )
        //if touching another player, and the player (that this collider belongs to) is falling, damage & bounce
        {
            //player bounces from head of other player
            attackingPlayer.rb2d.velocity = new Vector2(attackingPlayer.rb2d.velocity.x, headBounceForce);
            if(GameManager.gameManagerInstance.fightingEnabled) 
            {
                other.GetComponent<PlayerMinigameController>().DamagePlayer(1, false);
                AudioManager.instance.PlaySFX(2);
            }
        }
        
    }
}
