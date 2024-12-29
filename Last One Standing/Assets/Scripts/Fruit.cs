using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject goldenEffect;
    public bool isGoldenApple;
    public int value;//the value that catching a golden apple increments score by, rather than normal fruit
    public float speed;
    public Rigidbody2D fruitRb2d;
    private float time = 2f;

    void Update()
    {
        if(fruitRb2d.position.y <= -6.5f)//once the fruit reaches the ground
        {
            if (time > 0) //count down from 2 seconds
            {
            time -= Time.deltaTime;
            } 
            else //once 2 seconds are up, delete the fruit
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "Player")
        {
            if(isGoldenApple)
            /*if this object is a golden apple, increment the score & number of trophies
            of the player that touches it, and spawn an effect*/
            {
                other.GetComponent<PlayerMinigameController>().IncrementFruitScore(value);
                other.GetComponent<PlayerMinigameController>().IncrementTrophies();
                Instantiate(goldenEffect, transform.position, transform.rotation);
            } 
            else
            {//if not a golden apple, increment the score of the player that catches it by 1
                other.GetComponent<PlayerMinigameController>().IncrementFruitScore(1);
            }
            AudioManager.instance.PlaySFX(3);
            Destroy(gameObject);
        }
    }

    
}

