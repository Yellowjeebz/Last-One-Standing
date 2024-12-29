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

public class saveToDB : MonoBehaviour
{
    //SQL
    string constr = "Server=localhost;Database=lastonestandingdb;User ID=root;Password=root;";
    MySqlConnection con = null;
    MySqlCommand cmd = null;
    MySqlDataReader rdr = null;

    //Other Variables
    private PlayerController[] playersOrderdb = new PlayerController[4];
    public static saveToDB sTDbInstance;
    private int newGameNoToUse;

    //parameters into updateWinsdb
    private int dbWin, dbFfScore, dbTrophies, dbgameNo, dbplayerID;

    private void Awake()//awake runs before Start()
    {
        if(sTDbInstance == null)
        {sTDbInstance = this;}
        else{Destroy(gameObject);}
    }

    public void buttonMethod()//the method that is called when the quit button is pressed on the leaderboard
    {
        AudioManager.instance.PlaySFX(2);
        Debug.Log("saveToDB script has started");
        setGameID();
        for(int i =0;i<4; i++)//takes the winning order of players, determined in the leaderboard, and copies this into an array 
        {
            playersOrderdb[i] = Leaderboard.leaderboardInstance.playersOrder[i];
        }
        for(int i =0;i<4; i++)//for each player, in order, adds to the database by calling the updateWinsdb method with arguments from their player object
        {
            if(playersOrderdb[i].isWinner || playersOrderdb[i].playerScore == 3) dbWin = 1;
            dbFfScore = playersOrderdb[i].GetComponent<PlayerMinigameController>().maxFfScore;
            dbTrophies = playersOrderdb[i].GetComponent<PlayerMinigameController>().runningTrophyTotal;
            dbplayerID = playersOrderdb[i].playerID;
            dbgameNo = newGameNoToUse;

            updateWinsdb(dbWin, dbFfScore, dbTrophies, dbgameNo, dbplayerID);

            //resets scores to 0 to prevent player scores overflowing into another player's records.
            dbWin = 0;
            dbFfScore=0;
            dbTrophies =0;
            dbplayerID = 0;
        }
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    void setGameID()
    //GameID is not auto-incremented, so the last gameNumber must be fetched, incremented by 1 and then set as the new value
    {
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            Debug.Log("Connection State: "+ con.State);
            string query = string.Empty;
            query = "SELECT * FROM gamerecords ORDER BY GameNumber DESC LIMIT 1;";
            cmd = new MySqlCommand(query, con);
            var sqlreturn = cmd.ExecuteScalar(); 
            if(sqlreturn != null) 
            {
                int gameNoFetched = Convert.ToInt32(sqlreturn);
                Debug.Log("gameIDFetched: " + gameNoFetched);
                newGameNoToUse = gameNoFetched+1;             
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    void updateWinsdb(int dbWin, int dbFfScore, int dbTrophies, int dbgameNo, int dbplayerID)
    {//this will be called once per player. Per game, there is 1 gameID which is used for all players. 
    //Information is added to the Gamerecords table based on the UserID of each player
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            Debug.Log("Connection State: " + con.State);
            string query = string.Empty;
            query = "INSERT INTO gamerecords (GameTrophies, GameWin, GameFFScore, GameNumber, FKUserID) VALUES (@GameTrophies, @GameWin, @GameFFScore, @GameNumber, @UserID)";
            using (cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@GameTrophies", dbTrophies);
                    cmd.Parameters.AddWithValue("@GameWin", dbWin);
                    cmd.Parameters.AddWithValue("@GameFFScore", dbFfScore);
                    cmd.Parameters.AddWithValue("@GameNumber", dbgameNo);
                    cmd.Parameters.AddWithValue("@UserID", dbplayerID);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    Debug.Log("User details updated in DB");            
                }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}
