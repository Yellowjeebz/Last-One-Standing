using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayer2Keyboard : MonoBehaviour
{
    public GameObject referenceToPlayer2Keyboard;
    private bool Player2KeyboardIsLoaded = false;
    void Update()//check in every frame whether the k key has been pressed
    {
        if(!Player2KeyboardIsLoaded)
        {
            if(Keyboard.current.kKey.wasPressedThisFrame)
            {
                Instantiate(referenceToPlayer2Keyboard,transform.position, transform.rotation);
                Player2KeyboardIsLoaded=true;
            }
        }
    }
    
}
