using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for controlling and playing sound when a button is pressed.
/// </summary>
public class ButtonSound : MonoBehaviour
{
    private AudioSource audioSource; /* AudioSource */

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>(); /* Get the component of the AudioSource object on the GameObject this script is placed on */
    }

    public void playSound()
    {
        audioSource.Play(); /* Play the clip that was placed in the AudioSource */
    }
}
