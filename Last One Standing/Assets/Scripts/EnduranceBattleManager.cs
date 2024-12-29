using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnduranceBattleManager : MonoBehaviour
{
    private bool roundOver = false;
    private int noOfTimesTheElsePartOfUpdateCalled = 0;
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> powerupSpawnPoints = new List<Transform>();

    public float timeBetweenPowerups = 5f;
    private float powerupCounter;
    public GameObject[] powerUps;
    public GameObject trophy;

    public GameObject Canvas;
    private bool postLoadingScreenStartMethodRun = false;

    void Start()
    {
        WinnerUICanvas.WUCInstance.callLoadingScreenCo(3f);//loads the loading screen with argument of 3 seconds
    }
    void postLoadingScreenStart()
    {
        //after the loading screen has disappeared, powerupCounter is initialised, fighting is enabled, players are spawned in random locations and activated and their healths are initialised
        Debug.Log("postLoadingScreenStart method run");
        if(WinnerUICanvas.WUCInstance.loadingScreenCoroutineHasCompleted)
        {
            powerupCounter=timeBetweenPowerups;
            if(SceneManager.GetActiveScene().name == "EnduranceBattle1")//enables fighting if it is EB
            {
                GameManager.gameManagerInstance.fightingEnabled=true;
            }
            foreach(PlayerController player in GameManager.gameManagerInstance.activePlayers)
            //sets the spawn point for each player, when EB is first loaded.
            {
                int randomSpawnPoint = Random.Range(0,spawnPoints.Count);
                player.transform.position = spawnPoints[randomSpawnPoint].position;
                spawnPoints.RemoveAt(randomSpawnPoint);
            }
            for(int i =0;i<4;i++)
            {
                GameManager.gameManagerInstance.activePlayers[i].gameObject.SetActive(true);
                GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().currentHealth = 3;
            }
            postLoadingScreenStartMethodRun = true;
        }
    }
    void Update()
    {
        if(!postLoadingScreenStartMethodRun) postLoadingScreenStart();
        if(WinnerUICanvas.WUCInstance.loadingScreenCoroutineHasCompleted)//if the loading screen has disappeared
        {
            //spawn powerups every timeBetweenPowerups seconds
            if(powerupCounter > 0)//counts down the counter until it reaches 0
            {
                powerupCounter -= Time.deltaTime;
                if(powerupCounter <= 0)//when the counter reaches 0, spawn
                {
                    powerupCounter = timeBetweenPowerups * Random.Range(.75f,1.25f);
                    int randomPoint = Random.Range(0, powerupSpawnPoints.Count);
                    Instantiate(powerUps[Random.Range(0, powerUps.Length)], powerupSpawnPoints[randomPoint].position, powerupSpawnPoints[randomPoint].rotation);  

                    int chanceOfTrophy = Random.Range(0,20);
                        if(chanceOfTrophy==10)//if chanceOfTrophy value is 10, a trophy is spawned
                        {
                            randomPoint = Random.Range(0, powerupSpawnPoints.Count);
                            Instantiate(trophy, powerupSpawnPoints[randomPoint].position, powerupSpawnPoints[randomPoint].rotation);  
                        }
                }
            }

            if(GameManager.gameManagerInstance.CheckActivePlayers() == 1)
            //if one player is left, the round ends.
            {
                roundOver = true;
            }
            if(roundOver)
            {
                //increment score of remaining player, then load the next level.
                if(noOfTimesTheElsePartOfUpdateCalled == 0)
                {
                    noOfTimesTheElsePartOfUpdateCalled++;
                    GameManager.gameManagerInstance.fightingEnabled=false;
                    //WinnerUICanvas obj = WinnerUICanvasObj.WinnerUICanvas.GetComponent
                    //StartCoroutine(WinnerUICanvas.WUCInstance.WinningInfoBoxCo(GameManager.gameManagerInstance.lastPlayerRemainingName));;
                    //WinnerUICanvas.WUCInstance.callWinningInfoBoxCo(GameManager.gameManagerInstance.lastPlayerRemainingName);
                    string name = GameManager.gameManagerInstance.lastPlayerRemainingName;
                    Canvas.GetComponent<WinnerUICanvas>().callWinningInfoBoxCo(name, GameManager.gameManagerInstance.lastPlayerRemainingNumber);
                    //Debug.Log("this callWInning");
                    //GameManager.gameManagerInstance.updateWins(GameManager.gameManagerInstance.activePlayers[GameManager.gameManagerInstance.lastPlayerRemainingNumber]);
                    //GameManager.gameManagerInstance.GoToNextLevel();
                    Debug.Log("end of update reached");
                }
            }
        }
    }
    public void LateUpdate()
    {
        if(roundOver && WinnerUICanvas.WUCInstance.coroutineHasCompleted)
        {
            Debug.Log("LateUpdate is called in Endurance Battle Manager");
            for(int i =0;i<4;i++)
            {
                if(GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore > GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().maxFfScore) GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().maxFfScore = GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore; //sets maxFfScore if new high ffScore
                GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore=0;//at the end of the minigame, prep for FF, reset the player's ffScore to 0
                GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().currentHealth = 3;//at the end of the minigame, prep for EB, set the player's health to full.
                GameManager.gameManagerInstance.activePlayers[i].gameObject.SetActive(true);
                GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().UpdateHearts();
                for(int x=0;x<3;x++) 
                {
                    GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().greenHeartMasks[x].SetActive(false);
                    //resets invincibility
                }
            }
            roundOver = false;
        }
    }

}
