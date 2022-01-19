using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for keeping track of the time spent on the calculation/investment tasks. Cancels and continues if user takes to long, warns with sound before cancelling.
/// For now, will not be used, time will instead be tracked.
/// </summary>
public class CountdownSound : MonoBehaviour
{
    // MainController parent GameObject and its MainController component
    [SerializeField]
    private GameObject mainControllerObject;
    private MainController mainController;

    private AudioSource audioSource;

    private bool isRunning = false;
    private float timer; /* Main timer, tracking time the user has left for a task */
    private float lastTimer; /* Used to only play sound every full second */
    private float maxSeconds; /* The maximum amount of time the user has for completing a task */
    private float warningStart; /* The last x seconds of the task, when the alarm sound is played */
    private string typeOfTask; /* Differentiate between tasks for calling the correct function in the MainController object in case of timeouts */

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
                if(timer > lastTimer + 1) /* To not play the sound every update call, but rather every full second */
                {
                    audioSource.Play();
                    lastTimer = timer;
                }
            }
            if(timer > maxSeconds) /* Timeout */
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
