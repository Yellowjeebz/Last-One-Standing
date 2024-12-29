using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform exitLocation;
    public GameObject playerSpawnEffect;
    public float individualPortalSpawnCountdown;
    private float individualPortalDestroyCountdown = 4f;
    public Transform portalSpawnLocation;
    private bool canBeDeleted;

    void Update()
    {
        if(individualPortalSpawnCountdown > 0)//before the portal has appeared, the countdown will count down until 0
        {
            individualPortalSpawnCountdown -= Time.deltaTime;
            if(individualPortalSpawnCountdown <=0) //once the countdown reaches 0, the portal moves on screen
            {
                this.transform.position = portalSpawnLocation.position;
                Instantiate(playerSpawnEffect, portalSpawnLocation.position, portalSpawnLocation.rotation);
            }
        }
        if(individualPortalDestroyCountdown > 0 && canBeDeleted)//if the portal has been touched, a countdown starts which will make the portal disappear after four seconds
        {
            individualPortalDestroyCountdown -= Time.deltaTime;
            if(individualPortalDestroyCountdown<=0)
            {
                Instantiate(playerSpawnEffect, transform.position, transform.rotation);//spawns a particle effect where the portal disappears
                Destroy(gameObject);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)//when a player enters the portal, it teleports the player to a designated destination, and triggers a condition which allows the portal to disappear
    {
        if(other.tag == "Player")
        {
            other.transform.position = exitLocation.position;//moves the player to exitLocation

            Instantiate(playerSpawnEffect, transform.position, transform.rotation);
            Instantiate(playerSpawnEffect,exitLocation.position,exitLocation.rotation);
            //effect to happen where the player starts and finishes

            AudioManager.instance.PlaySFX(7);
            canBeDeleted = true;//once the portal has been used for the first time, it can now be deleted
        }
    }
}
