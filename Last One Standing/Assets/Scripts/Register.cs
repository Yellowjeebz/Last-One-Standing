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

public class Register : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField passwordField;
    public TMP_InputField pConfirmationField;
    public Button registerButton;
    public TMP_Text passwordsDontMatchText;
    public TMP_Text usernameInUseText;
    public string userDetailsScreen;
    public bool pwMatch;
    public bool userAdded;

    private int sqlreturn;
    private bool letUserBeAdded = true;

    //Initialising variables for the SQL connection
    string constr = "Server=localhost;Database=lastonestandingdb;User ID=root;Password=root;";
    MySqlConnection con = null;// connection object
    MySqlCommand cmd = null;// command object
    MySqlDataReader rdr = null;// reader object

    void Start()
    {
        userAdded = false;
        passwordsDontMatchText.gameObject.SetActive(false);
    }
    void Update()
    {
        if(passwordField.text.Length > 0 && pConfirmationField.text.Length > 0)//validates password inputs are filled in, and displays and error if they are filled in but do not match 
        {
            if(passwordField.text == pConfirmationField.text)
            {
                pwMatch = true;
            }
        else pwMatch = false;
            VerifyInputs();           
            if(!pwMatch)
            {
                passwordsDontMatchText.gameObject.SetActive(true);
            } else
            {
                passwordsDontMatchText.gameObject.SetActive(false);
            }   
        }
        else if(passwordField.text.Length == 0 || pConfirmationField.text.Length == 0)
        {
            passwordsDontMatchText.gameObject.SetActive(false);
        }

        if(letUserBeAdded)
        {
            usernameInUseText.gameObject.SetActive(false);
        }
        else
        {
            usernameInUseText.gameObject.SetActive(true);
        }
        if(userAdded)//if a player has pressed the register button and successfully signed up with a new username and valid password, then the login scene will load
        {
            SceneManager.LoadScene("Login");
        }
    }
    public void VerifyInputs()
    {
        //tells the forms to only accept if the conditions are met (validation)
        registerButton.interactable = (nameField.text.Length >= 3 && passwordField.text.Length >=8 && pwMatch);
    }
    public void CallRegisterMethod()//a method is needed to call a coroutine from other scripts/by a button
    {
        StartCoroutine(checkIfUsernameTaken());  
    }

    IEnumerator checkIfUsernameTaken()
    {//select the id of the username already in use, otherwise return username not in use
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            Debug.Log("Connection State: " + con.State);

            string query = string.Empty;
            query = "SELECT UserID from users WHERE Username = @name;";
            cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", nameField.text);

            var sqlreturn = cmd.ExecuteScalar(); 
            if(sqlreturn != null) 
            {
                Debug.Log("username taken");   
                letUserBeAdded = false;              
            }
            else //if the username is not in use, the new username and password will be added and a new record will be created to the Users table
            { 
                Debug.Log("username not taken");
                letUserBeAdded = true;  
                string query2 = string.Empty;
                query2 = "INSERT INTO users (Username, Password) VALUES (@name, @password)";
                using (cmd = new MySqlCommand(query2, con))
                {
                    cmd.Parameters.AddWithValue("@name",nameField.text);
                    cmd.Parameters.AddWithValue("@password", passwordField.text);
                    cmd.CommandText = query2;
                    cmd.ExecuteNonQuery();
                    Debug.Log("user added");            
                }
            userAdded = true;

            }          
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        yield break; 
    }

}
