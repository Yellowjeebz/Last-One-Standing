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

public class Login : MonoBehaviour
{
    //Initialising variables for the SQL connection
    string constr = "Server=localhost;Database=lastonestandingdb;User ID=root;Password=root;";
    MySqlConnection con = null;
    MySqlCommand cmd = null;
    MySqlDataReader rdr = null;

    //References to on-screen UI objects
    public TMP_InputField nameField;
    public TMP_InputField passwordField;
    public Button submitButton;
    public TMP_Text combinationErrorText;
    public TMP_Text combinationTakenErrorText;

    public int lastUserLoggedInID;
    public int previousID;

    public List<int> IDsInUse = new List<int>();

    public static int noOfLoggedIn;

    public bool match = false;
    public bool previousIsCurrentID;
    public static Login LoginInstance;
    public bool temp;

    private void Awake()//awake runs before Start()
    {
        if(LoginInstance == null)//if no instance has been set
        {
            LoginInstance = this;            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()//prevents players logging in when the scene is first loaded and no inputs are in the input fields
    {
        submitButton.interactable = false;
    }
    void Update()//constantly checking for if the input fields have been filled in
    {
        VerifyInputs(); 
    }

    public void VerifyInputs()//validates inputs are the correct length
    {
        submitButton.interactable = (nameField.text.Length >= 3 && passwordField.text.Length >=8);
    }

    public void CallLoginMethod()
    //queries the database, checking if there is a login with the username and password matching the input fields. 
    {
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            Debug.Log("Connection State: " + con.State);

            string query = string.Empty;
            query = "SELECT UserID from users WHERE Username = @name AND Password = @password;";
            cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", nameField.text);
            cmd.Parameters.AddWithValue("@password", passwordField.text);
            var sqlreturn = cmd.ExecuteScalar(); 
            lastUserLoggedInID = Convert.ToInt32(sqlreturn);
            Debug.Log("lastUserLoggedInID" + lastUserLoggedInID);
            temp = GameManager.gameManagerInstance.checkIfIDInUse(lastUserLoggedInID);
            combinationErrorText.gameObject.SetActive(false);  
            combinationTakenErrorText.gameObject.SetActive(false);
                
            if(sqlreturn != null)//if there is a record with matching password and username...
            {   
                if(!temp)//if ID not already in use by another player... add a new player with the corresponding database UserID and load the user detail screen
                {
                    Debug.Log("password and username match the db");
                    match = true;
                    combinationErrorText.gameObject.SetActive(false);  
                    combinationTakenErrorText.gameObject.SetActive(false);  
                    PlayerJoiningManager.PlayerJoiningManagerInstance.createNewPlayers();
                    SceneManager.LoadScene("UserDetailScreen");      
                    noOfLoggedIn++;  
                    Debug.Log("noOfLoggedIn" + noOfLoggedIn);
                }  
                else//if username and password already used this game to log someone else in, don't let the player proceed
                {
                    Debug.Log("login already in use, try another username");
                    combinationTakenErrorText.gameObject.SetActive(true);   
                }            
            }   
            else //if there is no record with the inputted username and password, the player won't log in and will be told that their details are incorrect.
            { 
                Debug.Log("Login unsuccessful - password and/or username do not match the db");
                combinationErrorText.gameObject.SetActive(true);     
                match = false;
            }        
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}
