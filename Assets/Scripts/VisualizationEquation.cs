using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The class for the simple visualization in form of a equation.
/// </summary>
public class VisualizationEquation : MonoBehaviour
{
    // Relevant objects and components on them
    [SerializeField]
    private GameObject mainControllerObject;
    private MainController mainController;

    [SerializeField]
    private GameObject textObject;
    private TextMeshProUGUI text;

    private MainCalculator calculator;

    // Values for the exponential function
    private float initialValue;
    private float growth;
    private float maxX;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float frequency;

    // Values for the calculation in the Update() calls
    private float frequencyThreshold = 0;
    private float x = 0;
    private float y;

    void Start()
    {
        // Get the scripts as components of the objects
        mainController = mainControllerObject.GetComponent<MainController>();
        text = textObject.GetComponent<TextMeshProUGUI>();

        // Get the values for the function as public variables from the MainController
        initialValue = mainController.initialValue;
        growth = mainController.growthFactor;
        maxX = mainController.maxX;

        // Create a new MainCalculator object with the values for the function
        calculator = new MainCalculator(initialValue, growth, speed, frequency, maxX);

        Debug.Log("Equation visualization, values used: Intial: " + mainController.initialValue + ", growth: " + mainController.growthFactor + ", speed: " + mainController.speed
            + ", frequency: " + mainController.frequency + ", maxX: " + mainController.maxX);

        // Set the initial text
        text.text = initialValue + " * ( 1 + " + growth + " )<sup>x</sup> = ";  
    }

    void Update()
    {
        x += Time.deltaTime * speed; /* This is where the speed value manipulates the function */

        if(x > frequencyThreshold && x <= maxX) /* To increase performance and make things easier to follow, only calculate the function value when a certain threshold is passed */
        {
            y = calculator.getY(x); /* Calculate the current y value */

            text.text = initialValue + " * ( 1 + " + growth + " )<sup>" + x.ToString("F1") + "</sup> = " + y.ToString("F1"); /* Set the text element accordingly */

            frequencyThreshold += frequency; /* Increase the threshold */
        }
    }

    /// <summary>
    /// Restart the equation by setting all relevant values to 0.
    /// </summary>
    public void replay()
    {
        x = 0;
        frequencyThreshold = 0;
        text.text = initialValue + " * ( 1 + " + growth + " )<sup>" + x.ToString("F1") + "</sup> = ";
    }
}
