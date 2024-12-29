using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    private float cobwebTimer;
    public bool speedsSet = false;
    public bool timerSet = false;
    public List<PlayerController> PCs = new List<PlayerController>();
    
    void Update()
    /*every frame counts down the timer until it reaches 0, 
    where the timer is reset, and the speeds of any players that come into contact with the cobweb will be set*/
    {
        if(cobwebTimer >0)
        {
            cobwebTimer-=Time.deltaTime;
            if(cobwebTimer<=0)
            {
                cobwebTimer=0;
                speedsSet=false;
                timerSet=false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    //when a player touches the cobweb, add that player's playercontroller to the list and set the timer, and play the cobweb SFX
    {
        if(other.tag == "Player")
        {
            if(cobwebTimer==0 && !timerSet)
            {
                PCs.Add(other.GetComponent<PlayerController>());
                cobwebTimer=1f;
                timerSet = true;
                AudioManager.instance.PlaySFX(9);
            }
            if(cobwebTimer>0)
            {
                if(!speedsSet)
                {
                    //updates speed of most recent player to touch the cobweb
                    PCs[PCs.Count -1].xSpeed = PCs[PCs.Count -1].xSpeed * 0.1f;
                    PCs[PCs.Count -1].ySpeed = PCs[PCs.Count -1].ySpeed * 0.1f;
                    speedsSet= true;
                }
            }
        }
    }    
    private void OnTriggerExit2D(Collider2D other)//when the player is no longer touching/gets out of the cobweb, reset their speed
    {
        if(other.tag == "Player")
        {
            PCs[0].xSpeed = 4;
            PCs[0].ySpeed = 21;
            PCs.RemoveAt(0);
        }
    }
}
