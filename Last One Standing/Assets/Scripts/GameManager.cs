using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public List<PlayerController> activePlayers = new List<PlayerController>();
    public static GameManager gameManagerInstance;
    private bool gameWon;
    public string[] allLevels;
    private List<string> levelOrder = new List<string>();
    public GameObject playerSpawnEffect;

    public List<string> playerNames = new List<string>();

    public bool fightingEnabled=false;
    public int lastPlayerRemainingNumber;
    public string lastPlayerRemainingName;

    void Start()
    {
        DontDestroyOnLoad(gameObject);//after the gamemanager is first made, it will not be deleted even when loading a new scene
    }
    private void Awake()//awake runs before Start(), and at the start of a new scene, if no gamemanager object exists a new one is created, otherwise duplicate gamemanagers are deleted
    {
        if(gameManagerInstance == null)//if no instance has been set as the game manager
        {gameManagerInstance = this;}
        else{Destroy(gameObject);}
    }
    public void AddPlayers(PlayerController newPlayer)//each time a player is created, it is spawned and also added to the activePlayers list
    {
        Instantiate(playerSpawnEffect,newPlayer.transform.position,newPlayer.transform.rotation);
        activePlayers.Add(newPlayer);
        
    }
    
    public void StartFirstRound()//loads the first minigame and ensures that the game is not won
    {
        gameWon = false;
        GoToNextLevel();
    }
    public void GoToNextLevel()//this is called at the end of each minigame when the minigame has been won. This method generates a random order of minigames which is continually refilled when it is empty.
    {       
        if(!gameWon){ 
            if(levelOrder.Count == 0)//if there are no more levels then refill the list
            {
                List<string> tempLevelList = new List<string>();
                tempLevelList.AddRange(allLevels);

                for(int i=0; i < allLevels.Length; i++)//repeat until all minigames are added at random to the list
                {
                    int selectedMinigame = Random.Range(0,tempLevelList.Count);//selects a random minigame

                    levelOrder.Add(tempLevelList[selectedMinigame]);
                    tempLevelList.RemoveAt(selectedMinigame);
                }
            }
            string levelToLoad = levelOrder[0];//first level to load is at the position 0 
            levelOrder.RemoveAt(0);//removes the randomly selected minigame at position 0
            SceneManager.LoadScene(levelToLoad);
        } 
        else
        {
            SceneManager.LoadScene("Leaderboard");
        }     
    }
    public bool checkIfIDInUse(int attemptedID)//this is used for login validation, to prevent multiple users logging in with the same details, and uses the ID of players logged in to compare.
    {
        bool IDinUse = false;
        if(activePlayers.Count == 0)
        {
            IDinUse = false;
        }
        else
        {
            for(int i=0; i < activePlayers.Count;i++)
            {
                Debug.Log("activePlayers[i].playerID " + activePlayers[i].playerID + "attemptedID " + attemptedID);
                if(activePlayers[i].playerID == attemptedID)
                {
                    Debug.Log("IDinUse: " + IDinUse);
                    IDinUse = true;
                    break;
                }
                else
                {
                    IDinUse = false;
                }
            }
        }
        return IDinUse;
    }
    public void updateWins(PlayerController winner)//called at the end of a minigame. increments the score of the player (which is an argument to the method)
    {
        Debug.Log("UpdateWins called");
        winner.playerScore++;
        if(winner.playerScore>=3)//if the game has been won (the winning player of the minigame gets their score incremented to 3), they are set as the winner and the game is won
            {
                gameWon=true;
                winner.isWinner = true;
            }
    }
    public int CheckActivePlayers()//this is int not void
    //returns the number of active players. if there is 1 player active remaining, this is set as lastRemaining
    {
        int playerAliveCount = 0;
        for(int i =0; i < activePlayers.Count; i++)
        {
            if(activePlayers[i].gameObject.activeInHierarchy)
            {
                playerAliveCount++;
                lastPlayerRemainingNumber = i;
                lastPlayerRemainingName = playerNames[i];
            }
        }
        return playerAliveCount;
    }
}



