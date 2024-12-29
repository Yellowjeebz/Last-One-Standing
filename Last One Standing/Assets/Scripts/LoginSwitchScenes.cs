using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginSwitchScenes : MonoBehaviour
{
    public string registerScene;
    public string loginScene;
    public string startScene = "userDetailsScreen";
    public TMP_Text noOfPlayersText;

    public static LoginSwitchScenes instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;    //so that an instance of this script can be referenced by other scripts        
        }
    }
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "PreLogin")//updates the text in the PreLogin screen to current number of players logged in
        {
            noOfPlayersText.text = ("Number of Users logged in: "+GameManager.gameManagerInstance.activePlayers.Count);
        }
        
    }

    public void goRegister()//loads the register screen
    {
        SceneManager.LoadScene(registerScene);
    }    
    public void goLogin()//loads the login screen
    {
        SceneManager.LoadScene(loginScene);
    }
    public void proceedFromDetailScreen()//loads the next screen after the user details screen, the next scene depends on if all players (4) have logged in or not
    {
        Debug.Log("no logged in is: "+Login.noOfLoggedIn);
        if(Login.noOfLoggedIn >=4)
        {
            SceneManager.LoadScene("CharacterSelect");
        }
        else SceneManager.LoadScene(loginScene);
    }
    public void goBackToStartMenu()//goes back to the start screen
    {
        SceneManager.LoadScene("StartGameMenu");
    }
}
