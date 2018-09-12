using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour {

    public AudioSource play;
    //点击按钮音效
    public void Chick ()
    {
        play.Play();
    }
}
