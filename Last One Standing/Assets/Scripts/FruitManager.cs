using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FruitManager : MonoBehaviour
{
    public GameObject[] fruits;
    private List<GameObject> fruitOrder = new List<GameObject>();
    private List<GameObject> tempFruit = new List<GameObject>();//creates a temporary list of all the fruit in fruits array
    public GameObject goldenApple;
        
    private bool roundOver = false;
    public float timeBetweenFruit;
    private float fruitBreakCounter;
    private int noOfTimesTheElsePartOfUpdateCalled = 0;
    private bool goldenAppleSpawned = false;//used to prevent more than 1 golden apple spawning in the minigame.

    public TMP_Text player1NameText;
    public TMP_Text player1ScoreText;
    public TMP_Text player2NameText;
    public TMP_Text player2ScoreText;
    public TMP_Text player3NameText;
    public TMP_Text player3ScoreText;
    public TMP_Text player4NameText;
    public TMP_Text player4ScoreText;
    public TMP_Text countdownTimer;

    private float ffUnconditCountdown;
    public GameObject Canvas;//to reference the win UI screen


    public List<Transform> playerSpawnPoints = new List<Transform>();

    void Start()
    {
        WinnerUICanvas.WUCInstance.callLoadingScreenCo(2f);
        
        fruitBreakCounter = timeBetweenFruit * Random.Range(.75f,1.25f);
        setPlayerNames();

        if(SceneManager.GetActiveScene().name == "FallingFruit1")
        {
            ffUnconditCountdown = 20f;
            GameManager.gameManagerInstance.fightingEnabled=false;
        }
        foreach(PlayerController player in GameManager.gameManagerInstance.activePlayers)
        //sets the spawn point for each player, when FF is first loaded.
        {
            int randomSpawnPoint = Random.Range(0,playerSpawnPoints.Count);
            player.transform.position = playerSpawnPoints[randomSpawnPoint].position;
            playerSpawnPoints.RemoveAt(randomSpawnPoint);
        }
    }

    void Update()
    {
        if(ffUnconditCountdown<=0f)
        {
            roundOver=true;
        }
        
        if(WinnerUICanvas.WUCInstance.loadingScreenCoroutineHasCompleted)//if the loading screen has disappeared
            {
                if(!roundOver)
                /*if the round hasn't ended, continue generating the order of random fruit, 
                updating player scores at the top of the screen, and countdown the minigame timer 
                (and update the text displaying the rounded value of the countdown)*/
                {
                    chooseRandomFruit();
                    setPlayerScores();    
                    ffUnconditCountdown -= Time.deltaTime;
                    countdownTimer.text = Mathf.CeilToInt(ffUnconditCountdown).ToString();
                }
                else
                //if the countdown has reached 0, iterate through all of the players' ffScores to find the player that has won the minigame and display their user
                {
                    int tempPlayerNo=0;
                    int tempHighestScore = 0;
                    for(int i=0; i<4; i++)
                    {
                        if(GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore > tempHighestScore)
                        {
                            tempHighestScore=GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore;
                            tempPlayerNo = i;
                        }
                    }
                    if(noOfTimesTheElsePartOfUpdateCalled == 0)
                    {
                        noOfTimesTheElsePartOfUpdateCalled++;
                        string name = GameManager.gameManagerInstance.playerNames[tempPlayerNo];
                        Canvas.GetComponent<WinnerUICanvas>().callWinningInfoBoxCo(name, tempPlayerNo);
                    }
                }
            }
    }
    public void LateUpdate()
    {
        if(roundOver)
        //if the countdown has reached 0, reset health, ffScores and active status of each player. Update the max ff score of players if appropriate.
        {
            for(int i =0;i<4;i++)
            {
                if(GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore > GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().maxFfScore) GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().maxFfScore = GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore; //sets maxFfScore if new high ffScore
                GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().ffScore=0;
                //at the end of the minigame, prep for FF, reset the player's ffScore to 0
                GameManager.gameManagerInstance.activePlayers[i].GetComponent<PlayerMinigameController>().currentHealth = 3;
                //at the end of the minigame, prep for EB, set the player's health to full.
                GameManager.gameManagerInstance.activePlayers[i].gameObject.SetActive(true);
            }
            roundOver = false;
        }

    }

    public void chooseRandomFruit()//randomly adds fruit to fruitOrder list and then loads the fruit at the start of the list (and removes it from the list) 
    {  
        if(fruitOrder.Count <= 1)
        {
            tempFruit.AddRange(fruits);
            for(int i=0; i < fruits.Length; i++)//randomly selects a fruit in the tempFruit array, and sets a random value. 
                {
                    int fruitIndex = Random.Range(0,tempFruit.Count);
                    int chanceOfGoldenApple = Random.Range(0,6);

                    fruitOrder.Add(tempFruit[fruitIndex]);
                    if(chanceOfGoldenApple==5 && !goldenAppleSpawned)//if chanceOfGoldenApple value is 10, a goldenApple is added to the fruitOrder list
                    {
                        fruitOrder.Add(goldenApple);
                        goldenAppleSpawned = true;
                    }
                    tempFruit.RemoveAt(fruitIndex);
                }
        }
        
        if(fruitBreakCounter > 0)//if the counter is not 0, decrease it by (1ish seconds)
            {
                fruitBreakCounter -= Time.deltaTime;
                if(fruitBreakCounter <= 0)
                //if the counter reaches 0 (or below)
                //set it to random time based on timeBetweenFruit, and spawn the first fruit in the list then remove it from the list.
                {
                    fruitBreakCounter = timeBetweenFruit * Random.Range(.75f,1.25f);
                    float randomXCoord = Random.Range(-13.2f,13.5f);
                    float randomYCoord = Random.Range(0f,7.3f);
                    Instantiate(fruitOrder[0], new Vector3(randomXCoord,randomYCoord,0), transform.rotation);
                    fruitOrder.RemoveAt(0);   
                }
            }
    }
    public void setPlayerNames()//sets the names in the text boxes at the top of the screen to the appropriate usernames of each player
    {
        player1NameText.text = GameManager.gameManagerInstance.playerNames[0] + ":";
        player2NameText.text = GameManager.gameManagerInstance.playerNames[1] + ":";
        player3NameText.text = GameManager.gameManagerInstance.playerNames[2] + ":";
        player4NameText.text = GameManager.gameManagerInstance.playerNames[3] + ":";
    }
    public void setPlayerScores()
    /*sets the scores in the text boxes at the top of the screen to the appropriate scores of each player relating to their username, 
    this is called each frame to ensure the scores stay up to date*/
    {
        player1ScoreText.text = (GameManager.gameManagerInstance.activePlayers[0].GetComponent<PlayerMinigameController>().ffScore).ToString();
        player2ScoreText.text = (GameManager.gameManagerInstance.activePlayers[1].GetComponent<PlayerMinigameController>().ffScore).ToString();
        player3ScoreText.text = (GameManager.gameManagerInstance.activePlayers[2].GetComponent<PlayerMinigameController>().ffScore).ToString();
        player4ScoreText.text = (GameManager.gameManagerInstance.activePlayers[3].GetComponent<PlayerMinigameController>().ffScore).ToString();
    }
}
