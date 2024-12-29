using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoiningManager : MonoBehaviour
{
    public GameObject referenceToPlayer;
    public GameObject referenceToPlayer2Keyboard;
    public static PlayerJoiningManager PlayerJoiningManagerInstance;
    
    private bool Player2KeyboardIsLoaded = false;
    private int noOfPlayersLoaded = 0;

    private void Awake()//awake runs before Start()
    {
        if(PlayerJoiningManagerInstance == null)
        {
            PlayerJoiningManagerInstance = this;            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void createNewPlayers()
    //creates a new player object which will be hidden until the character select screen. number of players loaded is incremented so that the game knows when to load the character select.
    {
        if(!Player2KeyboardIsLoaded && noOfPlayersLoaded==1)//the second player to load will be keyboardplayer2
        {
            Instantiate(referenceToPlayer2Keyboard,transform.position, transform.rotation);
            Player2KeyboardIsLoaded=true;
            noOfPlayersLoaded++;
        }
        else
        {
            Instantiate(referenceToPlayer,transform.position, transform.rotation);
            noOfPlayersLoaded++;
        }
    }
    
}


