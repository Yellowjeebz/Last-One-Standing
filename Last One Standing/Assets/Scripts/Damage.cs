using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Damage : MonoBehaviour
{
    public int dmgToDeal;
    public bool isDeathPit;
    public PlayerController playerTheSwipeBelongsTo;
    //public PlayerController 

    private void OnTriggerEnter2D(Collider2D other)
        {
            if(SceneManager.GetActiveScene().name == "EnduranceBattle1")
            {
                if(other.tag =="Player" && GameManager.gameManagerInstance.fightingEnabled)
                {
                    AudioManager.instance.PlaySFX(1);
                    Debug.Log("Damage called!!! playerTheSwipeBelongsTo: " + other.GetComponent<PlayerController>().playerID);
                    other.GetComponent<PlayerMinigameController>().DamagePlayer(dmgToDeal, isDeathPit);
                    //damages the player that enters the collider of the swipe/trap

                }  
            }
            else if(SceneManager.GetActiveScene().name == "ZombiePocalypse1")
            {
                if(other.tag =="Player" && playerTheSwipeBelongsTo.GetComponent<PlayerMinigameController>().isZombie && !other.GetComponent<PlayerMinigameController>().isZombie)
                //zombie swipe - if the player attacking is a zombie, then they can infect human players into zombies
                {
                    AudioManager.instance.PlaySFX(1);
                    Debug.Log("Zombie Damage called!!!");
                    ZombiePocalypseManager.ZPInstance.infectedPlayers.Add(other.GetComponent<PlayerController>());
                    //saves information about the player prior to them becoming a zombie
                    other.GetComponent<PlayerMinigameController>().infect();
                    //converts the player that enters the collider of the swipe into a zombie
                }
                if(other.tag =="Player" && !playerTheSwipeBelongsTo.GetComponent<PlayerMinigameController>().isZombie && other.GetComponent<PlayerMinigameController>().isInitialZombie)
                //human swipe - if the player attacking is a human, they can damage the initial zombie
                {
                    AudioManager.instance.PlaySFX(1);
                    Debug.Log("human swipe");
                    other.GetComponent<PlayerMinigameController>().DamageInitialZombie(playerTheSwipeBelongsTo);
                }
            }
            
            this.enabled = false;
        }
}
