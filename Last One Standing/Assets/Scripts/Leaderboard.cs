using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard leaderboardInstance;
    //References
    public TMP_Text position1;
    public TMP_Text position2;
    public TMP_Text position3;
    public TMP_Text position4;

    public TMP_Text nameText1;
    public TMP_Text nameText2;
    public TMP_Text nameText3;
    public TMP_Text nameText4;

    public Image playerImage1;
    public Image playerImage2;
    public Image playerImage3;
    public Image playerImage4;

    public TMP_Text score1;
    public TMP_Text score2;
    public TMP_Text score3;
    public TMP_Text score4;

    public TMP_Text trophyText1;
    public TMP_Text trophyText2;
    public TMP_Text trophyText3;
    public TMP_Text trophyText4;
    public TMP_Text trophiesThisGameText;
    public Image trophy1, trophy2, trophy3, trophy4;


    //Logic variables
    public int[] runningTrophiesTotals;
    public bool tie;
    private List<int> playerScores = new List<int>();
    public PlayerController[] playersOrder = new PlayerController[4];
    private List<PlayerController> tempActivePlayers = new List<PlayerController>();
    private int[] finishingPositions = new int[4];
    
    public PlayerController defaultPC;

    private void Awake()//awake runs before Start()
    {
        if(leaderboardInstance == null)//if no instance has been set for leaderboard
        {leaderboardInstance = this;}
        else{Destroy(gameObject);}
    }

    void Start()
    {
        trophiesThisGameText.text = "Trophies awarded this game:";//this will be replaced if no trophies have been awarded
        calculateWinningPositions();
        setPositionsAndScores();
    }

    void calculateWinningPositions()//determines if there is a tie or not and adds the scores of all the players to a list to be sorted
    {
        for(int i=0;i<4;i++)
        {
            if(playerScores.Count > 0)
            {
                for(int x=0;x<playerScores.Count; x++)
                {
                    if(GameManager.gameManagerInstance.activePlayers[i].playerScore == playerScores[x])
                    {
                        tie =true;
                    }
                }
            }
            playerScores.Add(GameManager.gameManagerInstance.activePlayers[i].playerScore);
        }

        //sort the scores in descending order
        playerScores.Sort();
        playerScores.Reverse();

        for(int i=0;i<4;i++)//copies contents of activePlayers into tempActivePlayers
            {
                tempActivePlayers.Add(GameManager.gameManagerInstance.activePlayers[i]);
            }
        for(int x=0;x<tempActivePlayers.Count;x++)//determines winning order of players
            {
                bool writeMade = false;
                if(tempActivePlayers[x].playerScore == playerScores[0])
                {
                    if(playersOrder[0]==defaultPC) 
                    {
                        playersOrder[0] = GameManager.gameManagerInstance.activePlayers[x];//1st place
                        writeMade = true;
                    }
                }
                if(tempActivePlayers[x].playerScore == playerScores[1] && !writeMade)
                {
                    if(playersOrder[1]==defaultPC)
                    {
                        playersOrder[1] = (GameManager.gameManagerInstance.activePlayers[x]);//2nd place
                        writeMade = true;
                    } 
                }
                if(tempActivePlayers[x].playerScore == playerScores[2] && !writeMade)
                {
                    if(playersOrder[2]==defaultPC) 
                    {
                        playersOrder[2] = (GameManager.gameManagerInstance.activePlayers[x]);//3rd place
                        writeMade = true;
                    }
                }
                if(tempActivePlayers[x].playerScore == playerScores[3] && !writeMade)
                {
                    if(playersOrder[3]==defaultPC) 
                    {
                        playersOrder[3] = (GameManager.gameManagerInstance.activePlayers[x]);//4th place
                        writeMade = true;
                    }
                }
            }
    }
    
    void determineTiePositions()
    {
        Debug.Log("determineTiePositions is called");
        int maxScore=0;
        int numberOfOnes = 0;
        int numberOfTwos = 0;
        for(int i =1;i<3;i++)//highest score below 3
        {
            if(playerScores[i] > maxScore) maxScore = playerScores[i];
        }
        for(int i =0;i<3;i++)//determines if there are multiple scores of 2, and if so how many
        {
            if(playerScores[i] == 2) numberOfTwos++;
        }
        for(int i =0;i<3;i++)//determines if there are multiple scores of 1, and if so how many
        {
            if(playerScores[i] == 1) numberOfOnes++;
        }
        if(maxScore == 0) //scores - 3,0,0,0
        {
            finishingPositions[0]=1;
            finishingPositions[1]=2;
            finishingPositions[2]=2;
            finishingPositions[3]=2;
        }
        if(maxScore == 1)
        {
            if(numberOfOnes==3) //scores - 3,1,1,1
            {
                finishingPositions[0]=1;
                finishingPositions[1]=2;
                finishingPositions[2]=2;
                finishingPositions[3]=2;
            }
            else if(numberOfOnes==2) //scores - 3,1,1,0
            {                
                finishingPositions[0]=1;
                finishingPositions[1]=2;
                finishingPositions[2]=2;
                finishingPositions[3]=3;
            }
            else if (numberOfOnes==1) //scores - 3,1,0,0
            {
                finishingPositions[0]=1;
                finishingPositions[1]=2;
                finishingPositions[2]=2;
                finishingPositions[3]=3;
            }
        }
        if(maxScore == 2)
        {
            if(numberOfTwos==3) //scores - 3,2,2,2
            {
                finishingPositions[0]=1;
                finishingPositions[1]=2;
                finishingPositions[2]=2;
                finishingPositions[3]=2;
            }
            else if(numberOfTwos==2) //scores - 3,2,2,x
            {                
                finishingPositions[0]=1;
                finishingPositions[1]=2;
                finishingPositions[2]=2;
                finishingPositions[3]=3;
            }
            else if(numberOfOnes==2) //scores - 3,2,1,1
            {
                {                
                    finishingPositions[0]=1;
                    finishingPositions[1]=2;
                    finishingPositions[2]=3;
                    finishingPositions[3]=3;
                }
            }
            else if(numberOfOnes==0)
            {
                if(numberOfTwos==1) //scores - 3,2,0,0
                {
                    finishingPositions[0]=1;
                    finishingPositions[1]=2;
                    finishingPositions[2]=3;
                    finishingPositions[3]=3;
                }
            }
        }
        
    }

    void setPositionsAndScores()    
    {
        if(tie)//sets the text boxes showing the positions of the players to the correct positions, calculated in the determineTiePositions method

        {
            determineTiePositions();
            position1.text = finishingPositions[0].ToString();
            position2.text = finishingPositions[1].ToString();
            position3.text = finishingPositions[2].ToString();
            position4.text = finishingPositions[3].ToString();
        }
        else//culls unneccessary call of determineTiePositions() if no tie
        {
            position1.text = "1";
            position2.text = "2";
            position3.text = "3";
            position4.text = "4";
        }

        //sets the corresponding scores of each player depending on the order of winning positions
        score1.text = playersOrder[0].playerScore.ToString();
        score2.text = playersOrder[1].playerScore.ToString();
        score3.text = playersOrder[2].playerScore.ToString();
        score4.text = playersOrder[3].playerScore.ToString();

        //sets the corresponding sprites of each player, chosen in the character select screen, depending on the order of winning positions
        playerImage1.sprite = playersOrder[0].GetComponent<SpriteRenderer>().sprite;
        playerImage2.sprite = playersOrder[1].GetComponent<SpriteRenderer>().sprite;
        playerImage3.sprite = playersOrder[2].GetComponent<SpriteRenderer>().sprite;
        playerImage4.sprite = playersOrder[3].GetComponent<SpriteRenderer>().sprite;

        //sets the corresponding names of each player depending on the order of winning positions
        nameText1.text = playersOrder[0].playerName;
        nameText2.text = playersOrder[1].playerName;
        nameText3.text = playersOrder[2].playerName;
        nameText4.text = playersOrder[3].playerName;

        //set trophies
        int highestTrophyNo =0;
        for(int i = 0; i<4; i++)//determines whether to display "no trophies won this game" - if all players have won 0 trophies
        {
            if(playersOrder[i].GetComponent<PlayerMinigameController>().runningTrophyTotal > highestTrophyNo)
            {
                highestTrophyNo = playersOrder[i].GetComponent<PlayerMinigameController>().runningTrophyTotal;
            }
        }
        if(highestTrophyNo>0)//if at least one trophy has been won, trophy information will be displayed
        {
            trophyText1.text = playersOrder[0].playerName+" won "+playersOrder[0].GetComponent<PlayerMinigameController>().runningTrophyTotal+" trophies";
            if(playersOrder[0].GetComponent<PlayerMinigameController>().runningTrophyTotal > 0) trophy1.gameObject.SetActive(true);
            trophyText2.text = playersOrder[1].playerName+" won "+playersOrder[1].GetComponent<PlayerMinigameController>().runningTrophyTotal+" trophies";
            if(playersOrder[1].GetComponent<PlayerMinigameController>().runningTrophyTotal > 0) trophy2.gameObject.SetActive(true);
            trophyText3.text = playersOrder[2].playerName+" won "+playersOrder[2].GetComponent<PlayerMinigameController>().runningTrophyTotal+" trophies";
            if(playersOrder[2].GetComponent<PlayerMinigameController>().runningTrophyTotal > 0) trophy3.gameObject.SetActive(true);
            trophyText4.text = playersOrder[3].playerName+" won "+playersOrder[3].GetComponent<PlayerMinigameController>().runningTrophyTotal+" trophies";
            if(playersOrder[3].GetComponent<PlayerMinigameController>().runningTrophyTotal > 0) trophy4.gameObject.SetActive(true);
        }
        else
        {
            trophiesThisGameText.text = "No trophies awarded this game";
            trophyText1.gameObject.SetActive(false);
            trophyText2.gameObject.SetActive(false);
            trophyText3.gameObject.SetActive(false);  
            trophyText4.gameObject.SetActive(false);
        }
    }
    
}