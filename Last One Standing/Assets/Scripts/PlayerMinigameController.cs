using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMinigameController : MonoBehaviour
{

    public int runningTrophyTotal;
    public PlayerMinigameController pMinigameInstance;

    //FF variables:
    public int ffScore;
    public int maxFfScore = 0;

    //endurance battle variables:
    public int maxHealth =3;//each player has 3 hearts
    public int currentHealth;
    public SpriteRenderer[] playerHearts;
    public Sprite fullHeart, emptyHeart;
    public Transform heartContainer;
    public GameObject hearts;
    private float damageCooldown = 0;
    public float invincCounter = 0;
    public GameObject[] greenHeartMasks;

    //ZP variables:
    public bool isInitialZombie = false;
    public bool isZombie = false;
    public int playerIDofFinalKill;
    

    private void Awake()
    {
        if(pMinigameInstance == null)//instance of the controller to be referenced by the minigame manager scripts
        {
            pMinigameInstance = this;            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        currentHealth= maxHealth;
    }
    void Update()
    {
        if(damageCooldown>=0)
        {
            damageCooldown -= Time.deltaTime;
        }
        if(SceneManager.GetActiveScene().name == "EnduranceBattle1" || isInitialZombie)
        //if it is the endurance battle scene or the player has been selected as the initial zombie, their hearts will be visible
        {
            hearts.SetActive(true);
        }
        else hearts.SetActive(false);

        if(invincCounter>0)
        {
            invincCounter -= Time.deltaTime;
            if(invincCounter <= 0)
            {
                hearts.gameObject.SetActive(true);
                //reset heart colour (by hiding the colours)
                for(int i=0;i<3;i++) greenHeartMasks[i].SetActive(false);
            }
        }
    }
    void LateUpdate()//stops the hearts flipping 
    {
        heartContainer.localScale = transform.localScale;
    }

    public void DamagePlayer(int dmgToTake, bool isDeathPit)
    //EB: this will be called when players attack each other, or players interact with damaging objects
    {
        if(damageCooldown <=0 && invincCounter <=0 || isDeathPit)
        {
            damageCooldown=2f;
            currentHealth -= dmgToTake;
            if(currentHealth<0)
            {
                currentHealth=0;//ensures the player isn't accidentally damaged too much
            }
            UpdateHearts();
            if(currentHealth==0)
            {
                gameObject.SetActive(false);//hide, but don't delete, the character from the scene
                AudioManager.instance.PlaySFX(4);
                
            }
        }
    }
    
    public void UpdateHearts()//EB:
    {/*switch based on current health*/
        switch(currentHealth) 
        {
            case 3:
                playerHearts[0].sprite = fullHeart;//0 because arrays start at 0 so 0 is the first element in the array.
                playerHearts[1].sprite = fullHeart;
                playerHearts[2].sprite = fullHeart;
                break;//break means "that's the end of dealing with this case"
            case 2:
                playerHearts[0].sprite = fullHeart;
                playerHearts[1].sprite = fullHeart;
                playerHearts[2].sprite = emptyHeart;
                break;
            case 1:
                playerHearts[0].sprite = fullHeart;
                playerHearts[1].sprite = emptyHeart;
                playerHearts[2].sprite = emptyHeart;
                break;

            case 0://the player shouldn't see this (health=0), implemented incase the player doesn't get disabled straight away.
                playerHearts[0].sprite = emptyHeart;
                playerHearts[1].sprite = emptyHeart;
                playerHearts[2].sprite = emptyHeart;
                break;
        }
    }

    public void IncrementFruitScore(int valueToIncrementBy)//FF: touching fruit triggers this function. 
    {
        if(SceneManager.GetActiveScene().name == "FallingFruit1")
        {//check the current minigame is falling fruit, then increment falling fruit score by the right amount
            ffScore+=valueToIncrementBy;
        }
    }
    public void IncrementTrophies()//called by other scripts (trophies) if the player touches them 
    {
        runningTrophyTotal++;
    }
    public void fillHealthPowerup()//called by the health powerup to restore health to its maximum
    {
        currentHealth= maxHealth;
        UpdateHearts();
        hearts.gameObject.SetActive(true);      
    }
    public void invincibilePowerup()//called by the invincibility powerup to temporarily stop the player getting damaged.
    {
        invincCounter = 3f;
        hearts.gameObject.SetActive(true);
        for(int i=0;i<3;i++) greenHeartMasks[i].SetActive(true);      
    }


    public void DamageInitialZombie(PlayerController other)
    //if the player is the initial zombie, then when they are damaged, this is called and is used to damage/kill the player
    {
        if(damageCooldown <=0)
        {
            playerIDofFinalKill = other.playerID;
            damageCooldown=2f;//prevents other players spamming the damage player and "instantly" killing the zombie
            currentHealth -= 1;
            if(currentHealth<0)
            {
                currentHealth=0;//ensures the zombie player isn't accidentally damaged too much
            }
            UpdateHearts();
            if(currentHealth==0)
            {
                gameObject.SetActive(false);//hide, but don't delete, the character from the scene
                ZombiePocalypseManager.ZPInstance.initialZombieLoses = true;
            }
        }
    }
    public void infect()//when this is called, the player becomes a zombie
    {
        isZombie=true;
        ZombiePocalypseManager.ZPInstance.numberOfZombies++;
        this.GetComponent<PlayerController>().playerAnimator.runtimeAnimatorController = ZombiePocalypseManager.ZPInstance.zombieOverride;
        this.GetComponent<PlayerController>().xSpeed = 3;//set the speed of any infected players to be slower.
    }
}
