using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //reference to all of the SFX
    public AudioSource[] SFX;

    //static - let all other scripts in the game have the ability to access the audio manager
    public static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void PlaySFX(int sfxToPlay)
    //takes the input of element of sfx in array, then plays the SFX at that position
    {
        SFX[sfxToPlay].Stop();//to prevent overlapping of soudns
        SFX[sfxToPlay].Play();
    }
}
