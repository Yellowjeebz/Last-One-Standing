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

public class LoadFromDB : MonoBehaviour
{
    //References to scene
    //public TMP_Text TotalWinsTxt;
    public TMP_Text NoWinsTxt;
    //public TMP_Text TrophiesTxt;
    public TMP_Text NoTrophiesTxt;
    //public TMP_Text FFTxt;
    public TMP_Text NoFruitTxt;

    //SQL
    string constr = "Server=localhost;Database=lastonestandingdb;User ID=root;Password=root;";
    MySqlConnection con = null;
    MySqlCommand cmd = null;
    MySqlDataReader rdr = null;

    //Other Variables
    public static LoadFromDB LFDBInstance;
    
    
    private void Awake()//awake runs before Start()
    {
        if(LFDBInstance == null)
        {LFDBInstance = this;}
        
        else{Destroy(gameObject);}
    }
    void Start()
    {
        setUserDbDetails();
    }

    public void setUserDbDetails()//for each player when they log in this script will be called. this method calls the methods which pull the maxFFScore, Number of wins and total number of trophies from the database
    {
        NoFruitTxt.text = LoadFromDB.LFDBInstance.findFFMaxScore(GameManager.gameManagerInstance.activePlayers[Login.noOfLoggedIn -1].playerID).ToString();
        NoWinsTxt.text = LoadFromDB.LFDBInstance.findTotalWins(GameManager.gameManagerInstance.activePlayers[Login.noOfLoggedIn -1].playerID).ToString();
        NoTrophiesTxt.text = LoadFromDB.LFDBInstance.findTrophies(GameManager.gameManagerInstance.activePlayers[Login.noOfLoggedIn -1].playerID).ToString();
    }
    public int findFFMaxScore(int UserIDinput)//queries the database to find the highest FFScore for a user stored in the database 
    {
        int maxFFScoreInDb = 0;
        try//try catch used since dealing with a database connection can be temperamental so if the connection is unsuccessful, this error is isolated and won't cause the game to crash
        {
            con = new MySqlConnection(constr);
            con.Open();
            Debug.Log("Connection State: "+ con.State);
            string query = string.Empty;
            query = "SELECT GameFFScore FROM gamerecords WHERE FKUserID = @ID ORDER BY GameFFScore DESC LIMIT 1;";
            cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ID", UserIDinput);
            var sqlreturn = cmd.ExecuteScalar(); 
            Debug.Log("sqlreturn" + sqlreturn);
            if(sqlreturn != null) 
            {
                maxFFScoreInDb = Convert.ToInt32(sqlreturn);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return maxFFScoreInDb;
    }
    

    public int findTotalWins(int UserIDinput)//queries the database to find the number of games that the player has won (where a record has a GameWin value of 1)
    {
        int noOfWinsInDb = 0;
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            string query = string.Empty;
            query = "SELECT COUNT(*) FROM gamerecords WHERE GameWin = 1 AND FKUserID = @ID";        
            cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ID", UserIDinput);  
            var sqlreturn = cmd.ExecuteScalar();
            if(sqlreturn != null) 
            {
                noOfWinsInDb = Convert.ToInt32(sqlreturn);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return noOfWinsInDb;
    }

    public int findTrophies(int UserIDinput)//queries the database to find the number of records holding a value of trophies greater than 0, and adding these together to get a running total.
    {
        int numberOfTrophies = 0;
        int totalNumberOfTrophies = 0;
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            string query = string.Empty;
            query = "SELECT COUNT(*) FROM gamerecords WHERE GameTrophies >= 1 AND FKUserID = @ID";        
            cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ID", UserIDinput);  
            var sqlreturn = cmd.ExecuteScalar();
            if(sqlreturn != null) 
            {
                numberOfTrophies = Convert.ToInt32(sqlreturn);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        if(numberOfTrophies>0)//if no records contain trophies, this part will not be run (since 0+0 =0!)
        {
            for(int i =0;i<numberOfTrophies;i++)
            try
            {
                con = new MySqlConnection(constr);
                con.Open();
                string query = string.Empty;
                query = "SELECT GameTrophies FROM gamerecords WHERE GameTrophies >= 1 AND FKUserID = @ID ORDER BY GameTrophies LIMIT @iteration,1;";        
                cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", UserIDinput);  
                cmd.Parameters.AddWithValue("@iteration", i);  
                var sqlreturn = cmd.ExecuteScalar();
                if(sqlreturn != null) 
                {
                    totalNumberOfTrophies += Convert.ToInt32(sqlreturn);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
        return totalNumberOfTrophies;
    }
}