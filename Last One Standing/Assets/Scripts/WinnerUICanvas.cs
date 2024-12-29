using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinnerUICanvas : MonoBehaviour
{
    public static WinnerUICanvas WUCInstance;
    public TMP_Text winnertext;
    public TMP_Text winnerNameText;
    public GameObject backgroundOverlay;
    public bool coroutineHasCompleted =false;
    public bool loadingScreenCoroutineHasCompleted =false;
    public GameObject loadingScreenCanvas;
    public GameObject loadingScreenBlackbg;

    public float timeToKeepLoadingScreenFor = 2f;

    private void Awake()//awake runs before Start()
    {
        if(WUCInstance == null)//if no instance has been set as the game manager
        {WUCInstance = this;}
        else{Destroy(gameObject);}
    }

    IEnumerator WinningInfoBoxCo(string winnerName, int winnerAPElement)//endround co-routine
    {
        Debug.Log("this is called");
        backgroundOverlay.SetActive(true);//make the winner info appear
        winnerNameText.gameObject.SetActive(true);//display which player won the round        
        winnertext.gameObject.SetActive(true);
        winnerNameText.text = winnerName;

        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;//prevents players moving
        
        //the co-routine waits for 2 seconds before proceeding
        //Increment score of the winner of EB
        GameManager.gameManagerInstance.updateWins(GameManager.gameManagerInstance.activePlayers[winnerAPElement]);
        GameManager.gameManagerInstance.GoToNextLevel();
        Time.timeScale = 1f;//allows the player(s) to move again
        coroutineHasCompleted = true;
    }
    IEnumerator LoadingScreenCo(float timeToKeepLoadingScreenFor)//endround co-routine
    {
        //show the loading screen - don't let the minigame start- , wait for x seconds, then start the minigame.
        loadingScreenCanvas.SetActive(true);
        loadingScreenBlackbg.SetActive(true);

        yield return new WaitForSeconds(timeToKeepLoadingScreenFor);
        Time.timeScale = 0f;
        loadingScreenCanvas.SetActive(false);
        loadingScreenBlackbg.SetActive(false);
        Time.timeScale = 1f;
        loadingScreenCoroutineHasCompleted = true;
           
    }
    public void callWinningInfoBoxCo(string winnerName,  int winnerAPElement)//calls the WinningInfoBox coroutine with the name of the winning player, and the position of the winning player in the list of active players
    {
        StartCoroutine(WinningInfoBoxCo(winnerName, winnerAPElement));
    }
    public void callLoadingScreenCo(float timeToKeepLoadingScreenFor)//calls the loadingScreen coroutine with a variable time to display the loading screen for
    {
        StartCoroutine(LoadingScreenCo(timeToKeepLoadingScreenFor));
    }
}
