using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainController : MonoBehaviour
{
    private List<int> visualizationList = new List<int>(); /* There are three allowed values on the list: 0 for equation visualization, 1 for graph, 2 for interactive VR, used values are deleted */
    private int visualizationListCounter = 0;
    private int calculationCounter = 0; /* Multiple calculations are needed for each visualization, so here we keep track of them */
    private int maxCalculations = 3; /* The maximum number of calculations. It should be hardcoded and always the same, just to be prepared this variable exists. */
    private int investmentCounter = 0; /* Same as for the calculations, multiple for each visualization, this keeps track of where we are at the moment */
    private int maxInvestments = 3; /* Maximum number of investments, same as for the calculations above */

    // The classes for the different visualizations are components of the corresponding GameObject
    [SerializeField]
    private GameObject visualizationEquationParent;
    [SerializeField]
    private GameObject visualizationGraphParent;
    [SerializeField]
    private GameObject visualizationInteractiveParent;

    // The parent GameObjects for the various buttons the user has to press
    [SerializeField]
    private GameObject buttonsContinueReplayParent;
    [SerializeField]
    private GameObject numPadParent;

    // Parent GameObjects and text objects for the calculation display/prompts and input
    [SerializeField]
    private GameObject calculationParent;

    [SerializeField]
    private GameObject textCalculationObject;
    private TextMeshProUGUI textCalculation;

    [SerializeField]
    private GameObject textCalculationAnswerParent;
    private TextMeshProUGUI textCalculationAnswer;

    [SerializeField]
    private GameObject numPadConfirmParent;

    [SerializeField]
    private GameObject investmentTwoImagesParent;
    [SerializeField]
    private GameObject investmentThreeImagesParent;

    // The classes for the different visualizations of the exponential growth
    private VisualizationEquation visualizationEquation;
    private VisualizationGraph visualizationGraph;
    private VisualizationInteractive visualizationInteractive;

    // The current method for visualization. 0 = equation, 1 = graph, 2 = interactive.
    private int currentVisualization;
    private GameObject currentVisualizationGameObject;

    // Values for the investment part
    private int simultaneousInvestments;
    private GameObject currentInvestmentObject;

    // The values for the current exponential growth function
    public float initialValue;
    public float growthFactor;
    public float speed;
    public float frequency;
    public float maxX;

    // TODO: DEBUG, delete after testing
    [SerializeField]
    private GameObject textDebugParent;

    private void Start()
    {
        // When first starting this (and the experiment), fill the list for the visualization options, then shuffle it. This way, the order of the visualizations is always randomized.
        // Also, this makes adding more cycles of the process later on easier, just add more numbers to the list. Probably not needed though.
        visualizationList.Add(0); /* 0 = equation */
        visualizationList.Add(1); /* 1 = graph */
        visualizationList.Add(2); /* 2 = interactive */
        //visualizationList = Util.shuffleList(visualizationList); /* Randomize the order of the values on the list */

        // Get the different classes for visualization from the components of the GameObjects
        visualizationEquation = visualizationEquationParent.GetComponent<VisualizationEquation>();
        visualizationGraph = visualizationGraphParent.GetComponent<VisualizationGraph>();
        visualizationInteractive = visualizationInteractiveParent.GetComponent<VisualizationInteractive>();

        // Get the text element of the calculation step
        textCalculation = textCalculationObject.GetComponent<TextMeshProUGUI>();
        textCalculationAnswer = textCalculationAnswerParent.GetComponent<TextMeshProUGUI>();

        startVisualization(); /* Start the first visualization with the first integer value on the now shuffled list */
    }

    private void Update()
    {
        textDebugParent.GetComponent<TextMeshProUGUI>().text = visualizationListCounter + "," + calculationCounter + "," + investmentCounter;
    }

    /// <summary>
    /// Activate a GameObject in relation to the value of the argument. The GameObject/number represent a type of visualization.
    /// </summary>
    /// <param name="option"></param>
    private void startVisualization()
    {
        if (visualizationListCounter < 3) /* TODO: Replace hardcoded 3 */
        {
            currentVisualization = visualizationList[visualizationListCounter];

            switch (currentVisualization)
            {
                case 0:
                    visualizationEquationParent.SetActive(true);
                    buttonsContinueReplayParent.SetActive(true);
                    currentVisualizationGameObject = visualizationEquationParent;
                    break;
                case 1:
                    visualizationGraphParent.SetActive(true);
                    buttonsContinueReplayParent.SetActive(true);
                    currentVisualizationGameObject = visualizationGraphParent;
                    break;
                case 2:
                    visualizationInteractiveParent.SetActive(true);
                    buttonsContinueReplayParent.SetActive(true);
                    currentVisualizationGameObject = visualizationInteractiveParent;
                    break;
                case 3:
                    // TODO: End, maximum number of visualizations reached.
                    Debug.Log("Finished all visualizations.");
                    break;
                default:
                    Debug.LogError("Value " + currentVisualization + " in switch case in startVisualization(), something went wrong.");
                    break;
            }
            visualizationListCounter++;
        } else
        {
            saveAndExit();
        }
    }

    /// <summary>
    /// Method to start the calculation, depending on the amount of calculations already done.
    /// </summary>
    /// <param name="counter"></param>
    private void startCalculation()
    {
        // If the maximum amount of calculations is not reached, start a new calculation prompt by enabling the GameObjects
        if (calculationCounter < maxCalculations)
        {
            Debug.Log("calculationCounter is " + calculationCounter + ", < " + maxCalculations + ", start new calculation.");
            calculationParent.SetActive(true);
            numPadParent.SetActive(true);

            numPadConfirmParent.SetActive(false); /* Hide the initial 'Confirm' button, so the user has to input something, and to prevent accidentally confirming multiple times */
            // TODO: Check if that is ok, or if user should have the option to skip. If so, build a sleeper function to prevent skipping. */

            // TODO: Load and randomize the prompt/values and display it in the calculation text 

            // Reset the input field
            textCalculationAnswer.text = "";
        } else
        {
            Debug.Log("calculationCounter is " + calculationCounter + ", >= " + maxCalculations + ", start investments.");
            // If the maximum amount has been reached, start the investment prompts, reset the counter
            calculationCounter = 0;
            startInvestment();
        }
    }

    /// <summary>
    /// Method for adding the number the user pressed to the calculation answer text field.
    /// </summary>
    /// <param name="number"></param>
    private void calculationAddNumber(string number)
    {
        if(!numPadConfirmParent.activeSelf) { numPadConfirmParent.SetActive(true); } /* Activate the confirm button if it is not active */
        if(textCalculationAnswer.text.Length < 8)
        {
            textCalculationAnswer.text = textCalculationAnswer.text + number;
        }
    }

    /// <summary>
    /// Method for deleting the last number the user entered from the calculation answer text field.
    /// </summary>
    private void calculationDeleteNumber()
    {
        if(textCalculationAnswer.text.Length > 0)
        {
            textCalculationAnswer.text = textCalculationAnswer.text.Remove(textCalculationAnswer.text.Length - 1);
        }
    }

    /// <summary>
    /// Method for confirming the input of the calculation answer text field, save the data and continue.
    /// </summary>
    private void calculationConfirmInput()
    {
        // TODO: Save data

        // Increase calculation counter, disable calculation objects, go to calculation start
        calculationCounter++;
        calculationParent.SetActive(false);
        numPadParent.SetActive(false);
        Debug.Log("Input confirmed, calculationCounter is at " + calculationCounter);
        startCalculation();
    }

    private void startInvestment()
    {
        Debug.Log("Start investment");
        // TODO: Read out images for the investments
        // TODO: Set number of investments to be displayed next to each other

        simultaneousInvestments = 2;

        if (investmentCounter < maxInvestments)
        {
            if (simultaneousInvestments == 2)
            {
                investmentTwoImagesParent.SetActive(true);
                currentInvestmentObject = investmentTwoImagesParent;
                // TODO: Load the correct images
            }
            else
            {
                if (simultaneousInvestments == 3)
                {
                    investmentThreeImagesParent.SetActive(true);
                    currentInvestmentObject = investmentThreeImagesParent;
                    // TODO: Load the correct images
                }
                else
                {
                    Debug.LogError("simultaneousInvestments is != 2 or 3, is " + simultaneousInvestments + ". Something went wrong.");
                }
            }
        } else
        {
            // If maximum number of investments is reached, go to next visualization, reset the counter
            investmentCounter = 0;
            startVisualization();
        }
    }

    private void investmentPicked(GameObject button)
    {
        switch(button.name)
        {
            case "PickLeft":
                // TODO: Save data
                break;
            case "PickMiddle":
                // TODO: Save data
                break;
            case "PickRight":
                // TODO: Save data

                break;
            default:
                Debug.LogError("InvestmentButtonPick with name '" + button.name + "' pressed. This name should not exist (Only left/right/middle).");
                break;
        }

        currentInvestmentObject.SetActive(false); /* After picking, disable the GameObject */
        investmentCounter++; /* Increase the counter by one */
        startInvestment(); /* Go to the next investment choice */
    }

    private void saveAndExit()
    {
        // TODO: Implement
        Debug.Log("Finished, saving and exiting");
    }

    /// <summary>
    /// Function that is called when any button in the scene is pressed. In it, it is decided which button, and what to do. The parameter 'name' is the name of the GameObject of which the collider was hit.
    /// </summary>
    public void buttonPressed(GameObject button)
    {
        // If the 'Replay' button is pressed, replay the animation of the corresponding visualization by calling the function in the class.
        if(button.name == "Replay")
        {
            switch(currentVisualization)
            {
                case 0:
                    visualizationEquation.replay();
                    break;
                case 1:
                    visualizationGraph.replay();
                    break;
                case 2:
                    visualizationInteractive.replay();
                    break;
                default:
                    Debug.LogError("Value " + currentVisualization + " in switch case in buttonPressed(), something went wrong.");
                    break;
            }
        }

        // If the 'Continue' button is pressed, deactivate the current visualization GameObject and start the calculation.
        // TODO: Add some delay before disabling.
        if(button.name == "Continue")
        {
            currentVisualizationGameObject.SetActive(false);
            buttonsContinueReplayParent.SetActive(false);
            startCalculation();
        }

        if(button.tag == "ButtonNumber") /* Buttons with numbers on the numpad for the calculation */
        {
            calculationAddNumber(button.name);
        }

        if(button.tag == "ButtonNumberDelete") /* Deletes the last number entered with the numpad for the calculation */
        {
            calculationDeleteNumber();
        }

        if(button.tag == "NumPadConfirm") /* Confirm input made with the numpad for the calculation */
        {
            Debug.Log("NumPadConfirm pressed.");
            calculationConfirmInput();
        }

        if(button.tag == "ButtonPickInvestment")
        {
            Debug.Log("ButtonInvestmentPick pressed: " + button.name);
            investmentPicked(button);
        }
    }
}
