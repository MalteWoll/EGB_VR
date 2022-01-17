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

    private float noiseLevel;

    // Values for the calculation in the Update() calls
    private float frequencyThreshold = 0;
    private float x = 0;
    private float y;

    private bool finished = false;
    private bool saved = false;

    void Start()
    {
        // Get the scripts as components of the objects
        mainController = mainControllerObject.GetComponent<MainController>();
        text = textObject.GetComponent<TextMeshProUGUI>();

        // Get the values for the function as public variables from the MainController
        initialValue = mainController.initialValue;
        growth = mainController.growthFactor;
        maxX = mainController.maxX;

        speed = mainController.speed;
        frequency = mainController.frequency;

        noiseLevel = mainController.noiseLevel;

        // Create a new MainCalculator object with the values for the function
        calculator = new MainCalculator(initialValue, growth, maxX, mainController.functionType, noiseLevel);

        Debug.Log("Equation visualization, values used: Intial: " + mainController.initialValue + ", growth: " + mainController.growthFactor + ", speed: " + mainController.speed
            + ", frequency: " + mainController.frequency + ", maxX: " + mainController.maxX + ", type: " + mainController.functionType);

        // Set the initial text
        text.text = "";  
    }

    void Update()
    {
        x += Time.deltaTime * speed; /* This is where the speed value manipulates the function */

        if(x > frequencyThreshold && x <= maxX) /* To increase performance and make things easier to follow, only calculate the function value when a certain threshold is passed */
        {
            if (!finished)
            {
                y = calculator.getY(x); /* Calculate the current y value */
            } else
            {
                y = calculator.getYAgain();
            }

            //text.text = initialValue + " * ( 1 + " + growth + " )<sup>" + x.ToString("F1") + "</sup> = " + y.ToString("F1"); /* Set the text element accordingly */
            text.text = y.ToString("F1");

            frequencyThreshold += frequency; /* Increase the threshold */
        } else
        {
            if (!finished && x >= maxX) /* To only call the activation of the continue button once, use a boolean that is set to true after activation */
            {
                PlayerPrefs.SetString("maxY", y.ToString());
                PlayerPrefs.Save();

                if (!saved)
                {
                    // Save the values of the equation by calling the function in the main controller
                    mainController.saveFunctionValues(calculator.getValueDict(), "equation");
                    saved = true;
                }

                mainController.activatContinueButton();
                finished = true;
            }
        }

        // For debug purposes, TODO: remove
        if (Input.GetKey(KeyCode.K))
        {
            replay();
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
        calculator.resetDictCounter();
    }

    public void reset()
    {
        // Get the values for the function as public variables from the MainController
        initialValue = mainController.initialValue;
        growth = mainController.growthFactor;
        maxX = mainController.maxX;

        // Create a new MainCalculator object with the values for the function
        calculator = new MainCalculator(initialValue, growth, maxX, mainController.functionType, noiseLevel);

        Debug.Log("Equation visualization, values used: Intial: " + mainController.initialValue + ", growth: " + mainController.growthFactor + ", speed: " + mainController.speed
            + ", frequency: " + mainController.frequency + ", maxX: " + mainController.maxX + ", type: " + mainController.functionType);

        x = 0;
        frequencyThreshold = 0;
        text.text = "";

        finished = false;
        saved = false;
    }
}
