using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBehaviour : MonoBehaviour
{
    public AudioSource yay;

    public void PlayYay()
    {
        yay.Play();
    }
}
