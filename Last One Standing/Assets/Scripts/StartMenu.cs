using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//needs importing to change scenes
using TMPro;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    //References
    public string startingScene;//references the next Scene to load (login)
    public Canvas howToPlayCanvas;
    [HideInInspector]
    public bool howToPlayCanvasActive;

    void Start()//used to make sure that the How To Play Screen is hidden when the game first laods
    {
        howToPlayCanvasActive = false;
    }
    public void StartGame()//the function that loads the next scene when "Play Game" button is clicked
   {
    SceneManager.LoadScene(startingScene);
    AudioManager.instance.PlaySFX(2);
   }

    public void HowToPlay()//function for clicking how to play. This will toggle the How To Play Canvas screen to appear
    {
        howToPlayCanvasActive = true;
        AudioManager.instance.PlaySFX(2);
    } 

    public void QuitGame()//quits the game and plays a SFX. for testing purposes, there is code to pause the execution in the unity editor
    {
        AudioManager.instance.PlaySFX(2);
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void BackToMainMenu()
    {
        howToPlayCanvasActive = false;
    }

    void Update()//This is used to toggle the How To Play canvas.
    {
        if(howToPlayCanvasActive == true){
            howToPlayCanvas.GetComponent<Canvas>().enabled = true;
            howToPlayCanvas.gameObject.SetActive(true);
        }
        else
        {
            howToPlayCanvas.gameObject.SetActive(false);
        }
    }
}
