using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void playSound()
    {
        audioSource.Play();
    }
}
