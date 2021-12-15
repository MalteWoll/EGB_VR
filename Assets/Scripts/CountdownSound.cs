using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for keeping track of the time spent on the calculation/investment tasks. Cancels and continues if user takes to long, warns with sound before cancelling.
/// For now, will not be used, time will instead be tracked.
/// </summary>
public class CountdownSound : MonoBehaviour
{
    [SerializeField]
    private GameObject mainControllerObject;
    private MainController mainController;

    private AudioSource audioSource;

    private bool isRunning = false;
    private float timer;
    private float lastTimer;
    private float maxSeconds;
    private float warningStart;
    private string typeOfTask;

    void Start()
    {
        mainController = mainControllerObject.GetComponent<MainController>();
        audioSource = this.GetComponent<AudioSource>();
    }


    void Update()
    {
        if(isRunning)
        {
            timer += Time.deltaTime; /* Increase timer by time passed between update calls */
            if(timer > maxSeconds - warningStart)
            {
                // Warning sounds
                if(timer > lastTimer + 1)
                {
                    // SOUND
                    audioSource.Play();
                    lastTimer = timer;
                }
            }
            if(timer > maxSeconds)
            {
                // Call the appropriate cancel function for calculation or investment
                if(typeOfTask == "calculation")
                {
                    mainController.invalidInputCalculation();
                } else
                {
                    if(typeOfTask == "investment")
                    {
                        mainController.invalidInputInvestment();
                    }
                }
                resetTimer();
            }
        }
    }

    /// <summary>
    /// Function to set the values for the timer and start it.
    /// </summary>
    /// <param name="seconds">The maximum number of seconds until the task is cancelled and skipped.</param>
    /// <param name="warningStart">The last number of seconds where a warning sound is played.</param>
    /// <param name="type">The type of task, calculation or investment.</param> 
    public void startTimer(float seconds, float warningStart, string type)
    {
        maxSeconds = seconds;
        isRunning = true;
        this.warningStart = warningStart;
        typeOfTask = type;
    }

    /// <summary>
    /// Resets all variables and sets the timer to off.
    /// </summary>
    public void resetTimer()
    {
        maxSeconds = 0;
        isRunning = false;
        warningStart = 0;
        typeOfTask = "";
        timer = 0;
        lastTimer = 0;
    }
}
