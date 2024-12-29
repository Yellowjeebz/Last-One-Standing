using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using UnityEngine.SceneManagement;

public class UserDetails : MonoBehaviour
{/*allocate the controller to the player, as well as access the database to find the name of the player from the database, 
and load number of wins and trophies.*/
    string constr = "Server=localhost;Database=lastonestandingdb;User ID=root;Password=root;";
    MySqlConnection con = null;
    MySqlCommand cmd = null;
    MySqlDataReader rdr = null;

    public TMP_Text nameText;
    public TMP_Text controllerText;
    public string[] controllers = new string[] {"Keyboard1","Keyboard2","ColouredController","BlackController"};
    public string controllerTextToLoadValue;
    public string nameValue;
    //public int numberOfPlayers = GameManager.gameManagerInstance.activePlayers.Count;
    private int noLoaded = Login.noOfLoggedIn -1;

    void Start()
    {
        nameValue=null;
    }

    void Update()
    {
        if(nameValue==null)//sets the values of the text on the screen
        {
            getUsernameOfCurrentPlayer();
            controllerTextToLoadValue = controllers[noLoaded];
            controllerText.text = controllerTextToLoadValue;
            nameText.text = nameValue + "!";
        }
    }
    public void getUsernameOfCurrentPlayer()//queries the database to get the username corresponding to the ID of the player 
    {
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            Debug.Log("Connection State: " + con.State);
            string query = string.Empty;
            query = "SELECT Username from users WHERE UserID = @id;";
            cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", GameManager.gameManagerInstance.activePlayers[noLoaded].playerID);
            var sqlreturn = cmd.ExecuteScalar(); 
            nameValue = sqlreturn.ToString();
            Debug.Log("nameValue" + nameValue); 
            GameManager.gameManagerInstance.playerNames.Add(nameValue);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    public void setUserController()//method called when the continue button is pressed. sets the Player number for each player, depending on their controller
    {
        Debug.Log("setUserController called. controller: " + controllers[noLoaded] + "id of most recent player: " + GameManager.gameManagerInstance.activePlayers[noLoaded].playerID + "noLoaded" + noLoaded);
        if(controllers[noLoaded] == "Keyboard1"/*controller from userdetails script*/)
        {
            GameManager.gameManagerInstance.activePlayers[noLoaded].isPlayer1Keyboard = true;
        }
        else if(controllers[noLoaded]=="Keyboard2")
        {
            GameManager.gameManagerInstance.activePlayers[noLoaded].isPlayer2Keyboard = true;
        }
        else if(controllers[noLoaded]=="ColouredController")
        {
            GameManager.gameManagerInstance.activePlayers[noLoaded].isPlayer3Colour = true;
        }        
        else if(controllers[noLoaded]=="BlackController")
        {
            GameManager.gameManagerInstance.activePlayers[noLoaded].isPlayer4Black = true;
        }

        GameManager.gameManagerInstance.activePlayers[noLoaded].playerName = GameManager.gameManagerInstance.playerNames[noLoaded];
    }
}
