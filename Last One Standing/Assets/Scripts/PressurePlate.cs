using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PressurePlate : MonoBehaviour
{
    public string firstScene;
    private int playersOnPressurePlate;
    public TMP_Text counterText;
    public TMP_Text gameStartingInText;
    public SpriteRenderer pPlateSR;
    public Sprite pPlateUp, pPlateDown;
    public float timeToStart = 3f;
    private float startCounter;

    void Update()
    {
        if(playersOnPressurePlate > 1)
        {
            //when 2+ players are on the pressure plate, start countdown and display text and toggle sprite of pressure plate
            counterText.gameObject.SetActive(true);
            gameStartingInText.gameObject.SetActive(true);
            pPlateSR.sprite = pPlateDown;
            startCounter -= Time.deltaTime;
            counterText.text = Mathf.CeilToInt(startCounter).ToString();//make sure to convert to string since the textbox only accepts strings, not numbers
                                        //ceiltoint will round the number up
            if(counterText.text=="0")
            {
               GameManager.gameManagerInstance.StartFirstRound();
            }
        }
        else if(playersOnPressurePlate < 2) 
        {
            //else reset 
            startCounter = timeToStart;
            //if our players are NOT in the zone, we'll always be making sure that the start counter is reset to timeToStart
            pPlateSR.sprite = pPlateUp; 
            counterText.gameObject.SetActive(false);
        }

    }
    private void OnTriggerEnter2D(Collider2D other)//each time a player steps on the pressure plate, add 1
    {
        if(other.tag == "Player")
        {
            playersOnPressurePlate++;
        }
    }
    private void OnTriggerExit2D(Collider2D other)//each time a player steps off the pressure plate, minus 1
    {
        if(other.tag == "Player")
        {
            playersOnPressurePlate--;
        }
    }
    private void playPPNoise()//plays a noise signifying the countdown to the start of the game
    {
        if(startCounter < 3f) AudioManager.instance.PlaySFX(10);
    }
}
