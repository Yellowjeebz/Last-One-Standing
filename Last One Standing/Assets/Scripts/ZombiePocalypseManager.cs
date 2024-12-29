using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombiePocalypseManager : MonoBehaviour
{
    public static ZombiePocalypseManager ZPInstance;
    private bool roundOver = false;
    public bool initialZombieLoses;
    public int playerSelectedAsInitial;
    public int winningPlayerPositionInActivePlayers;
    public int numberOfZombies =0;

    public RuntimeAnimatorController initialZombieOldOverride;
    public List<PlayerController> infectedPlayers = new List<PlayerController>();
    public RuntimeAnimatorController zombieOverride; 

    public List<Transform> spawnPoints = new List<Transform>();

    public GameObject Canvas;//to reference the win UI screen
    private bool postLoadingScreenStartMethodRun = false;

    public List<Transform> trophySpawnPoints = new List<Transform>();
    public GameObject trophy;
    private float trophyCounter = 3f;
    private int numberOfTrophies = 0;

    private void Awake()
    {
        if(ZPInstance == null)
        {ZPInstance = this;}
        else{Destroy(gameObject);}
    }
    void postLoadingScreenStart()//reset player positions if they move when the loading screen is active
    {
        foreach(PlayerController player in GameManager.gameManagerInstance.activePlayers)
        //sets the spawn point for each player, when ZP is first loaded.
        {
            int randomSpawnPoint = Random.Range(0,spawnPoints.Count);
            player.transform.position = spawnPoints[randomSpawnPoint].position;
            spawnPoints.RemoveAt(randomSpawnPoint);
        }
        postLoadingScreenStartMethodRun = true;
    }

    void Start()//when the minigame is first loaded...
    {
        WinnerUICanvas.WUCInstance.callLoadingScreenCo(3.5f);//loads the loading screen for 3.5 seconds
        selectInitialZombie();//chooses an initial zombie
    }
    void Update()
    {  
        if(WinnerUICanvas.WUCInstance.loadingScreenCoroutineHasCompleted)//if the loading screen has disappeared)
        {
            if(!postLoadingScreenStartMethodRun) postLoadingScreenStart();
            if(GameManager.gameManagerInstance.CheckActivePlayers() == 3)
            {
                //only the initialZombie can be set an inactive. 
                //therefore if there are not 4 active players then the initial zombie has been "killed"
                initialZombieLoses = true;
                for(int i =0;i<4;i++)
                {
                    if(GameManager.gameManagerInstance.activePlayers[i].playerID == GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].GetComponent<PlayerMinigameController>().playerIDofFinalKill)
                    {//finds the position of the killing player in the activePlayers list, comparing IDs
                        winningPlayerPositionInActivePlayers = i;
                    }
                }
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].gameObject.SetActive(true);
                roundOver = true;
            }
            if(numberOfZombies==4) 
            {
                initialZombieLoses = false;
                winningPlayerPositionInActivePlayers = playerSelectedAsInitial;
                roundOver = true;//the other winning condition: if there are 4 zombies, the round ends
            }
            if(roundOver)
            //when the minigame is over, player healths are reset, heart sprites are reset, the player speed is reset, and boolean values holding if the player is a zombie or not are set to false
            {
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].GetComponent<PlayerMinigameController>().isInitialZombie = false;
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].GetComponent<PlayerMinigameController>().currentHealth = 3;
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].GetComponent<PlayerMinigameController>().UpdateHearts();
                for(int i=0;i<4;i++)
                {
                    GameManager.gameManagerInstance.activePlayers[i].xSpeed = 4;
                    GameManager.gameManagerInstance.activePlayers[i].ySpeed = 21;
                }
                string name = GameManager.gameManagerInstance.playerNames[winningPlayerPositionInActivePlayers];//finds the name of the winner 
                Canvas.GetComponent<WinnerUICanvas>().callWinningInfoBoxCo(name, winningPlayerPositionInActivePlayers);//Display winner on screen
            } 
        }
        spawnZPTrophy();
    }
    public void LateUpdate()
    {
        if(roundOver && WinnerUICanvas.WUCInstance.coroutineHasCompleted)
        {
            //Reset animator controllers - get rid of the zombie override from all players
            GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].xSpeed = 4;//reset the speed of the initial zombie
            GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].playerAnimator.runtimeAnimatorController = GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].rtAnimator;
            for(int i=0;i<4;i++)
            {
                GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().isZombie = false;
                GameManager.gameManagerInstance.activePlayers[i].playerAnimator.runtimeAnimatorController=GameManager.gameManagerInstance.activePlayers[i].rtAnimator;
                GameManager.gameManagerInstance.activePlayers[i].xSpeed = 4;//reset the speed of any infected players.

            }
        }
    }

    void selectInitialZombie()
    {
        if(SceneManager.GetActiveScene().name == "ZombiePocalypse1")
            {
                playerSelectedAsInitial = Random.Range(0,3);
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].GetComponent<PlayerMinigameController>().isInitialZombie = true;
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].GetComponent<PlayerMinigameController>().isZombie = true;
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].xSpeed = 3;//slows the zombie player down slightly for the duration of ZP
                initialZombieOldOverride = GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].rtAnimator;//save the non-zombie animator
                GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].playerAnimator.runtimeAnimatorController = ZombiePocalypseManager.ZPInstance.zombieOverride;
                //GameManager.gameManagerInstance.activePlayers[playerSelectedAsInitial].rtAnimator = zombieOverride;
                numberOfZombies++;
            }
    }
    void spawnZPTrophy()
    {
        if(trophyCounter > 0)//counts down the counter until it reaches 0
            {
                trophyCounter -= Time.deltaTime;
                if(trophyCounter <= 0 && numberOfTrophies<=3)//when the counter reaches 0 but less than 3 trophies have been spawned already, spawn a trophy
                {
                    trophyCounter = 10 * Random.Range(.5f,1.5f);
                    int randomPoint = Random.Range(0, trophySpawnPoints.Count);
                    Instantiate(trophy, trophySpawnPoints[randomPoint].position, trophySpawnPoints[randomPoint].rotation);  

                }
            }
    }



}
