using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Music : MonoBehaviour
{
    AudioSource musicAudio;
   

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //Fetch the AudioSource from the GameObject
        musicAudio = GetComponent<AudioSource>();
        musicAudio.Play();
    }

}
