using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using OVR;

public class MainController : MonoBehaviour
{
    // Object for saving data
    private SavedData savedData = new SavedData();

    private List<int> visualizationList = new List<int>(); /* There are three allowed values on the list: 0 for equation visualization, 1 for graph, 2 for interactive VR, used values are deleted */
    private int visualizationListCounter = 0;
    private int calculationCounter = 0; /* Multiple calculations are needed for each visualization, so here we keep track of them */
    private int investmentCounter = 0; /* Same as for the calculations, multiple for each visualization, this keeps track of where we are at the moment */

    private bool visualizationUsedBefore = false;

    [SerializeField]
    private GameObject buttonSoundParent;
    private ButtonSound buttonSound;

    // GameObjects and their text components for displaying instructions
    [SerializeField]
    private GameObject instructionsParent;
    [SerializeField]
    private GameObject instructionsTextParent;
    private TextMeshProUGUI instructionsText;
    [SerializeField]
    private GameObject instructionsContinueParent;

    // The objects for the different visualizations classes are components of the corresponding GameObject
    [SerializeField]
    private GameObject visualizationEquationParent;
    [SerializeField]
    private GameObject visualizationGraphParent;
    [SerializeField]
    private GameObject visualizationInteractiveParent;
    // The classes for the different visualizations of the exponential growth
    private VisualizationEquation visualizationEquation;
    private VisualizationGraph visualizationGraph;
    private VisualizationInteractive visualizationInteractive;

    // The parent GameObjects for the various buttons the user has to press
    [SerializeField]
    private GameObject buttonsContinueReplayParent;
    [SerializeField]
    private GameObject numPadParent;
    [SerializeField]
    private GameObject sliderParent;
    [SerializeField]
    private GameObject sliderMainParent;
    private InputSlider inputSlider;

    // Parent GameObjects and text objects for the calculation display/prompts and input
    [SerializeField]
    private GameObject calculationParent;

    // Text objects and GameObjects they are placed in for calculation questions and answers
    [SerializeField]
    private GameObject textCalculationObject;
    private TextMeshProUGUI textCalculation;
    [SerializeField]
    private GameObject textCalculationAnswerParent;
    private TextMeshProUGUI textCalculationAnswer;

    [SerializeField]
    private GameObject numPadConfirmParent;

    // GameObjects for the investment picking
    [SerializeField]
    private GameObject investmentTwoImagesParent;
    [SerializeField]
    private GameObject investmentTwoImagesButtonPanelParent;
    [SerializeField]
    private GameObject investmentPickButtonLeft;
    [SerializeField]
    private GameObject investmentPickButtonRight;

    [SerializeField]
    private GameObject investmentPromptTextParent;
    private TextMeshProUGUI investmentPromptText;

    // The current method of visualization. 0 = equation, 1 = graph, 2 = interactive.
    private int currentVisualization;
    private GameObject currentVisualizationGameObject;

    // Current investment GameObject
    private GameObject currentInvestmentObject;

    // The values for the current exponential growth function
    public float initialValue;
    public float growthFactor;
    public float speed;
    public float frequency;
    public float maxX;
    public string functionType;
    [SerializeField]
    public float noiseLevel; /* As percentage */

    private float correctResult; /* Variable for the 'correct' result of the calculation prompt */

    [SerializeField]
    private GameObject centerEyeObject; /* The center eye object in the VR rig structure */

    // Variables for setting the height of the user after a short delay
    private bool firstUpdate = true;
    private float setHeightTimer = 0;

    private float timeForTask = 0;

    private bool firstVisualization = true;
    private bool firstCalculation = true;

    private string currentState; /* The state the experiment is currently in, for debugging in PC */
    [SerializeField]
    private GameObject investmentPickedDefaultButton; /* For debugging on PC */

    private bool finished = false;
    private float endCountdown = 0; /* Countdown at the end of the experiment, restarts after */

    [SerializeField]
    private int runthroughAmount; /* The number of complete run throughs */

    [SerializeField]
    public int goldBarScaling; /* The monetary value of one gold bar */

    private string stockIdent; /* 'StockA' or 'StockB' */

    // Slider values
    private float minSliderValue;
    private float maxSliderValue;

    private Settings settings;

    bool loadedSettingsSuccessfull;
    [SerializeField]
    private GameObject debugParent;

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);

        // Load the settings from the JSON settings file */
        settings = Util.LoadSettingsJSON();
        if(settings == null) { loadedSettingsSuccessfull = false; } else { loadedSettingsSuccessfull = true; }

        if (loadedSettingsSuccessfull)
        {
            /*speed = PlayerPrefs.GetFloat("speed");
            frequency = PlayerPrefs.GetFloat("frequency");
            goldBarScaling = PlayerPrefs.GetInt("goldBarScaling");
            runthroughAmount = PlayerPrefs.GetInt("amountOfRuns");
            noiseLevel = PlayerPrefs.GetFloat("noise");*/
            speed = settings.speed;
            frequency = settings.frequency;
            goldBarScaling = settings.goldBarScaling;
            runthroughAmount = settings.amountOfRuns;
            noiseLevel = settings.noise;
        } else
        {
            speed = 1;
            frequency = 2;
            goldBarScaling = 100;
            runthroughAmount = 2;
            noiseLevel = 0.05f;
            debugParent.SetActive(true);
        }

        // Get the script for the input slider
        inputSlider = sliderParent.GetComponent<InputSlider>();

        // Get the data from the intro and save it to the object for data saving
        savedData.Age = PlayerPrefs.GetInt("age");
        savedData.Gender = PlayerPrefs.GetString("gender");

        // Get the current time as start time
        savedData.Starttime = Util.getCurrentDateAndTime();

        // Make the initial save in the file
        Util.WriteToOutputFile(savedData.SaveProgress("initial"));

        List<int> partialList = new List<int>();

        // When first starting this (and the experiment), fill the list for the visualization options, then shuffle it. This way, the order of the visualizations is always randomized.
        for (int j = 0; j < runthroughAmount; j++)
        {
            partialList.Add(0); /* Add the equation visualization */
            partialList.Add(1); /* Add the graph visualization */
            partialList.Add(2); /* Add the interactive visualization */
            partialList = Util.shuffleList(partialList);

            // To avoid having situations where the same visualization is played back-to-back, add partial lists to the main list, instead of simply shuffling the main list
            for(int k = 0; k < 3; k++)
            {
                visualizationList.Add(partialList[k]);
            }

            partialList = new List<int>();
        }

        // Because of later changes, every visualization is used twice. To comply with existing code, we simply expand the list by adding the same value after each value once, for example: {1,3,2} -> {1,1,3,3,2,2}
        List<int> tempList = new List<int>();
        for(int i = 0; i < visualizationList.Count; i++)
        {
            tempList.Add(visualizationList[i]);
            tempList.Add(visualizationList[i]);
        }
        visualizationList = tempList;

        // TODO: REMOVE!
        //visualizationList = new List<int> { 1,1,0,0,2,2, 1, 1, 0, 0, 2, 2 };

        // Get the different classes for visualization from the components of the GameObjects
        visualizationEquation = visualizationEquationParent.GetComponent<VisualizationEquation>();
        visualizationGraph = visualizationGraphParent.GetComponent<VisualizationGraph>();
        visualizationInteractive = visualizationInteractiveParent.GetComponent<VisualizationInteractive>();

        // Get the text element of the calculation step
        textCalculation = textCalculationObject.GetComponent<TextMeshProUGUI>();
        textCalculationAnswer = textCalculationAnswerParent.GetComponent<TextMeshProUGUI>();

        buttonSound = buttonSoundParent.GetComponent<ButtonSound>();

        // Get the instruction text component
        instructionsText = instructionsTextParent.GetComponent<TextMeshProUGUI>();

        // Set the first textual instruction
        //instructionsText.text = Util.GetInstructionalText("previsualization");
        instructionsText.text = settings.instructionsPrevisualization;
        instructionsText.fontSize = settings.instructionsPrevisualizationTextSize;
        currentState = "instructions";

        investmentPromptText = investmentPromptTextParent.GetComponent<TextMeshProUGUI>();
        investmentPromptText.text = settings.instructionsInvestmentPrompt;
        investmentPromptText.fontSize = settings.instructionsInvestmentPromptTextSize;

        //startVisualization(); /* Start the first visualization with the first integer value on the now shuffled list */
    }

    private void Update()
    {
        // The height of the HMD is not always correctly detected on the first update call, sometimes it is far too low. To let user start from a comfortable height, without need for interaction,
        // wait 2 seconds before checking the height and setting the button heights accordingly.
        if(firstUpdate)
        {
            setHeightTimer += Time.deltaTime;
            if(setHeightTimer > 2)
            {
                setButtonHeights();
                firstUpdate = false;
            }
        }

        // When the user presses one of the buttons on the controller, the buttons etc. are being moved in height, in case they are still in a position uncomfortable for the user
        if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three)) /* Sometimes error messages regarding OVR? Program still compiling anyways */
        {
            Debug.Log("Center Eye Position: " + centerEyeObject.transform.position.ToString());
            setButtonHeights(); /* Sets the height of all control elements and buttons according to the current height of the HMD */
        }

        // When the calculation is active, react to slider input
        if(textCalculationAnswerParent.activeSelf)
        {
            if (inputSlider.touched)
            {
                float temp = inputSlider.currentValue + minSliderValue;
                textCalculationAnswer.text = temp.ToString("F0"); /* Set the text in the answer field to the value of the slider */
            } else
            {
                textCalculationAnswer.text = "?"; /* If the slider has not been touched yet, display a '?' instead of a value */
            }
        }

        timeForTask += Time.deltaTime; /* Saves the time passed since starting a calculation/investment. This is reset upon starting a calculation/investment, so no further if-block is needed */

        // When the experiment is finished, enable the countdown until restarting it and display the according text
        if(finished)
        {
            endCountdown += Time.deltaTime;
            int temp = 30 - (int)endCountdown;
            instructionsText.text = settings.instructionsEnding + "\n\n Application will restart in " + temp + " seconds.";
            if (endCountdown > 30)
            {
                SceneManager.LoadScene("Tutorial");
            }
        }

        // For debugging on PC, enables pressing keyboard buttons to continue
        if (Input.GetKeyDown(KeyCode.C))
        {
            switch (currentState)
            {
                case "instructions":
                    continueFromInstructions();
                    break;
                case "visualization":
                    continueFromVisualization();
                    break;
                case "calculation":
                    calculationConfirmInput(true);
                    break;
                case "investment":
                    investmentPicked(investmentPickedDefaultButton, true);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (currentState == "visualization")
            {
                replayVisualization();
            }
        }
    }

    /// <summary>
    /// Activate a GameObject in according to the value of the parameter. The GameObject/number represent a type of visualization.
    /// </summary>
    /// <param name="option"></param>
    private void startVisualization()
    {
        currentState = "visualization"; /* For debugging */
        if (visualizationListCounter < 6 * runthroughAmount) /* Per run, six visualizations are shown. This is multiplied with the number of runs. */
        {
            currentVisualization = visualizationList[visualizationListCounter]; /* Get the value for the type of visualization to use */

            // For a new visualization type, randomize the values. Since exponential and logarithmic function use the same values, don't randomize in a visualization type.
            if (!visualizationUsedBefore) 
            {
                setVisualizationData();
            }

            // Every visualization is used twice with an exponential and a logarithmic function. The following block decides what to use by randomization.
            if (!visualizationUsedBefore)
            {
                // The first time a visualization is started, randomize if it should show exponential or logarithmic growth first.
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    functionType = "exp";
                }
                else
                {
                    functionType = "log";
                }
                stockIdent = "stockA"; /* The first displayed visualization is always 'StockA', regardless of type (exp/log) */
                visualizationUsedBefore = true;
            }
            else
            {
                if (functionType == "exp")
                {
                    functionType = "log";
                }
                else
                {
                    functionType = "exp";
                }
                stockIdent = "stockB"; /* The second displayed visualization is always 'StockB', regardless of type (exp/log) */
                visualizationUsedBefore = false;
            }

            switch (currentVisualization) /* Depending on what visualization should be used, enable the according parent GameObject */
            {
                case 0: /* equation */
                    visualizationEquationParent.SetActive(true);
                    if(!visualizationUsedBefore) { visualizationEquation.reset(); } /* If this is the second time the visualization is used, reset it instead of calculating new values */
                    currentVisualizationGameObject = visualizationEquationParent;

                    // Save the visualization in the object for saving
                    savedData.addVisualization("equation");
                    savedData.addVisualizationStockIdentifier(stockIdent);
                    if(stockIdent == "stockA") { visualizationEquation.enableStockIdentA(); } else { visualizationEquation.enableStockIdentB(); }
                    Debug.Log("Starting equation visualization");
                    break;
                case 1: /* graph */
                    visualizationGraphParent.SetActive(true);
                    if (!visualizationUsedBefore) { visualizationGraph.reset(); } /* If this is the second time the visualization is used, reset it instead of calculating new values */
                    currentVisualizationGameObject = visualizationGraphParent;

                    // Save the visualization in the object for saving
                    savedData.addVisualization("graph");
                    savedData.addVisualizationStockIdentifier(stockIdent);
                    if (stockIdent == "stockA") { visualizationGraph.enableStockIdentA(); } else { visualizationGraph.enableStockIdentB(); }

                    Debug.Log("Starting graph visualization");
                    break;
                case 2: /* interative */
                    visualizationInteractiveParent.SetActive(true);
                    if (!visualizationUsedBefore) { visualizationInteractive.reset(); } /* If this is the second time the visualization is used, reset it instead of calculating new values */
                    currentVisualizationGameObject = visualizationInteractiveParent;

                    // Save the visualization in the object for saving
                    savedData.addVisualization("interactive");
                    savedData.addVisualizationStockIdentifier(stockIdent);
                    if (stockIdent == "stockA") { visualizationInteractive.enableStockIdentA(); } else { visualizationInteractive.enableStockIdentB(); }

                    Debug.Log("Starting interactive visualization");
                    break;
                case 3: /* This should never be reached */
                    Debug.LogError("Case 3 reached in visualization selection!");
                    break;
                default:
                    Debug.LogError("Value " + currentVisualization + " in switch case in startVisualization(), something went wrong.");
                    break;
            }

            // These three save values can be added here, since it does not matter what visualization they are used for
            // Saves type and values of the current visualization
            savedData.addVisualizationType(functionType);
            Util.WriteToOutputFile(savedData.SaveProgress("visualizationType"));
            savedData.addVisualizationInitial(initialValue.ToString().Replace(",", "."));
            Util.WriteToOutputFile(savedData.SaveProgress("visualizationInitial"));
            savedData.addVisualizationGrowth(growthFactor.ToString().Replace(",", "."));
            Util.WriteToOutputFile(savedData.SaveProgress("visualizationGrowth"));

            // Make a partial save
            Util.WriteToOutputFile(savedData.SaveProgress("visualization"));

            visualizationListCounter++;
        } else
        {
            saveAndExit();
        }
    }

    /// <summary>
    /// Method to start the calculation prompt.
    /// </summary>
    /// <param name="counter"></param>
    private void startCalculation()
    {
        currentState = "calculation"; /* For debugging */

        // Active the calculation and the slider
        calculationParent.SetActive(true);
        sliderMainParent.SetActive(true);

        int afterYears;
        // Randomize the year value for the prompt
        if (loadedSettingsSuccessfull)
        {
            //afterYears = Random.Range(PlayerPrefs.GetInt("afterYearsMin"), PlayerPrefs.GetInt("afterYearsMax"));
            afterYears = Random.Range(settings.afterYearsMin, settings.afterYearsMax);
        } else
        {
            afterYears = Random.Range(10, 50);
        }

        afterYears += (int)maxX; /* Add the maximum year that was used in the visualization */
        while (afterYears % 5 != 0) /* Only use multiple of 5, for easier reading and understanding */
        {
            afterYears++;
        }

        // Calculate 'correct' value for exponential function of the pair of visualizations
        correctResult = calculateCalculationResult(afterYears);
        

        float tempMaxY = PlayerPrefs.GetFloat("maxY"); /* Get the saved maximum value reached by the visualization (script) */
        minSliderValue = tempMaxY; /* Set the slider minimum to the maximum value reached by the visualization, as anything below makes no sense */

        // Set the maximum value of the slider to a randomized value multiplied with the maximum value of the expontial function
        maxSliderValue = correctResult * Random.Range(settings.sliderMultiplierMin, settings.sliderMultiplierMax); 

        inputSlider.setSliderValues(tempMaxY, maxSliderValue); /* Set the values for the slider */

        // Display the reached year/value and the prompt for the calculation
        textCalculationObject.GetComponent<TextMeshProUGUI>().text = "Year: " + maxX.ToString("F0") + "\n" + 
                                                                     "Value: " + tempMaxY + " $" + "\n\n" +
                                                                     "After " + afterYears.ToString("F0") + " years: ";

        // Save data, make partial save
        savedData.addCalculation(afterYears.ToString());
        Util.WriteToOutputFile(savedData.SaveProgress("calculation"));

        // Measure the time needed
        timeForTask = 0;

        // Reset the input field
        textCalculationAnswer.text = "?";
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
    private void calculationConfirmInput(bool valid)
    {

        string time = timeForTask.ToString("F2").Replace(",", "."); /* Convert the float value for the time needed to a string with two decimals */

        // Add the answer and the time needed to the save data object and add it to the save file
        //savedData.addCalculationResult(textCalculationAnswer.text);
        savedData.addCalculationResult(inputSlider.currentValue.ToString("F0"));
        savedData.addCalculationTime(time);
        savedData.addCalculationCorrectResult(correctResult.ToString("F0"));
        Util.WriteToOutputFile(savedData.SaveProgress("calculationResult"));


        // Increase calculation counter, disable and reset calculation objects, go to calculation start
        calculationParent.SetActive(false);
        //numPadParent.SetActive(false);
        inputSlider.resetSlider();
        sliderMainParent.SetActive(false);
        Debug.Log("Input confirmed, calculationCounter is at " + calculationCounter);

        if (calculationCounter == 1)
        {
            // If the maximum amount has been reached, start the investment prompts, reset the counter
            calculationCounter = 0;
            startInvestment();
        }
        else /* If not, start second visualization */
        {
            calculationCounter++;
            startVisualization();
        }
    }

    /// <summary>
    /// Enables the GameObjects for the investment prompt.
    /// </summary>
    private void startInvestment()
    {
        Debug.Log("Start investment");
        currentState = "investment";

        investmentTwoImagesParent.SetActive(true);

        // Disable the buttons for a moment
        investmentPickButtonLeft.SetActive(false);
        investmentPickButtonRight.SetActive(false);

        StartCoroutine(waitSecondsBeforeEnable(investmentPickButtonLeft, investmentPickButtonRight, 3)); /* Wait 3 seconds before enabling the buttons to pick an investment */

        currentInvestmentObject = investmentTwoImagesParent;

        timeForTask = 0;
    }

    private void investmentPicked(GameObject button, bool valid)
    {

        string time = timeForTask.ToString("F2").Replace(",", "."); /* Convert the float value for the time needed to a string with two decimals */
        savedData.addInvestmentTime(time); /* Add the time to the object for saving data */

        // According to the button pressed, save the name of the stock picked */
        switch (button.name)
        {
            case "PickLeft":
                savedData.addInvestmentResult("pickedStockA");
                break;
            case "PickRight":
                savedData.addInvestmentResult("pickedStockB");
                break;
            default:
                Debug.LogError("InvestmentButtonPick with name '" + button.name + "' pressed. This name should not exist (Only left/right/middle).");
                break;
        }

        // Write to partial save file
        Util.WriteToOutputFile(savedData.SaveProgress("investmentResults"));

        currentInvestmentObject.SetActive(false); /* After picking, disable the GameObject */
        investmentCounter++; /* Increase the counter by one */

        // Start the next visualization
        startVisualization();
    }

    /// <summary>
    /// When the experiment is finished, make a final save and activate the final instructional text
    /// </summary>
    private void saveAndExit()
    {
        finished = true;
        savedData.Endtime = Util.getCurrentDateAndTime();

        Util.WriteToOutputFile(savedData.SaveProgress("finish"));

        instructionsParent.SetActive(true);
        //instructionsText.text = Util.GetInstructionalText("ending");
        instructionsText.text = settings.instructionsEnding;
        instructionsText.fontSize = settings.instructionsEndingTextSize;
        Debug.Log("Finished, saving and exiting");
    }

    /// <summary>
    /// Function that is called when any button in the scene is pressed. In it, it is decided which button, and what to do. The parameter 'name' is the name of the GameObject of which the collider was hit.
    /// </summary>
    public void buttonPressed(GameObject button)
    {
        buttonSound.playSound(); /* Simple audio feedback upon pressing a button */

        // If the 'Replay' button is pressed, replay the animation of the corresponding visualization by calling the replay function in the class.
        if(button.name == "Replay")
        {
            replayVisualization();
        }

        // If the 'Continue' button is pressed, deactivate the current visualization GameObject and start the calculation.
        if(button.name == "Continue")
        {
            continueFromVisualization();
        }

        if(button.name == "InstructionsContinue")
        {
            continueFromInstructions();
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
            calculationConfirmInput(true); /* valid input marked by 'true' */
        }

        if(button.tag == "ButtonPickInvestment") /* 'Pick' button for the two investments the user is presented */
        {
            Debug.Log("ButtonInvestmentPick pressed: " + button.name);
            investmentPicked(button, true); /* If a button was pressed in time, the result is valid */
        }
    }

    /// <summary>
    /// Function for replaying a visualization, calls the function in the appropriate visualization object
    /// </summary>
    private void replayVisualization()
    {
        switch (currentVisualization)
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

    /// <summary>
    /// When the visualization is finished, reset everything, then go to the calculation.
    /// </summary>
    private void continueFromVisualization()
    {
        if (visualizationInteractiveParent.activeSelf)
        {
            visualizationInteractive.reset();
        } else
        {
            if(visualizationGraphParent.activeSelf)
            {
                visualizationGraph.reset();
            } else
            {
                if(visualizationEquationParent.activeSelf)
                {
                    visualizationEquation.reset();
                }
            }
        }
        currentVisualizationGameObject.SetActive(false);
        buttonsContinueReplayParent.SetActive(false);

        if (!firstCalculation)
        {
            startCalculation();
        }
        else
        {
            instructionsParent.SetActive(true);
            instructionsContinueParent.SetActive(true);
            //instructionsText.text = Util.GetInstructionalText("calculations");
            instructionsText.text = settings.instructionsCalculations;
            instructionsText.fontSize = settings.instructionsCalculationsTextSize;
            currentState = "instructions";
        }
    }

    /// <summary>
    /// Depending on which instruction has been shown (for visualization or calculation), continue to the first visualization or calculation
    /// </summary>
    private void continueFromInstructions()
    {
        instructionsParent.SetActive(false);
        instructionsContinueParent.SetActive(false);

        if (firstVisualization)
        {
            firstVisualization = false;
            startVisualization();
        }
        else
        {
            if (firstCalculation)
            {
                firstCalculation = false;
                startCalculation();
            }
        }
    }

    /// <summary>
    /// Simple coroutine for waiting a number of seconds before enabling an object. Used to enable buttons later, to avoid accidentely clicking them too soon.
    /// </summary>
    /// <param name="objectToEnable">The GameObject that will be enabled.</param>
    /// <param name="seconds">The amount of seconds to wait.</param>
    /// <returns></returns>
    private IEnumerator waitSecondsBeforeEnable(GameObject objectToEnable, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        objectToEnable.SetActive(true);
    }

    /// <summary>
    /// Same coroutine with two objects that can be disabled.
    /// </summary>
    /// <param name="objectToEnable1"></param>
    /// <param name="objectToEnable2"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator waitSecondsBeforeEnable(GameObject objectToEnable1, GameObject objectToEnable2, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        objectToEnable1.SetActive(true);
        objectToEnable2.SetActive(true);
    }

    /// <summary>
    /// For users of different heights, the buttons should be comfortably reachable. This function sets the height of the buttons according to the HMD position.
    /// </summary>
    private void setButtonHeights()
    {
        float heightUser = centerEyeObject.transform.position.y;
        numPadParent.transform.position = new Vector3(numPadParent.transform.position.x, heightUser - 0.5f, numPadParent.transform.position.z);
        buttonsContinueReplayParent.transform.position = new Vector3(buttonsContinueReplayParent.transform.position.x, heightUser - 0.5f, buttonsContinueReplayParent.transform.position.z);
        investmentTwoImagesButtonPanelParent.transform.position = new Vector3(investmentTwoImagesButtonPanelParent.transform.position.x, heightUser - 0.5f, investmentTwoImagesButtonPanelParent.transform.position.z);
        sliderMainParent.transform.position = new Vector3(sliderMainParent.transform.position.x, heightUser - 0.5f, sliderMainParent.transform.position.z);
        instructionsContinueParent.transform.position = new Vector3(instructionsContinueParent.transform.position.x, heightUser - 0.5f, instructionsContinueParent.transform.position.z);
    }

    /// <summary>
    /// Set the global variables, which the three visualization classes use by assigning random values to them.
    /// </summary>
    /// <param name="counter"></param>
    private void setVisualizationData()
    {
        if (loadedSettingsSuccessfull)
        {
            /*initialValue = Random.Range(PlayerPrefs.GetFloat("initialMin"), PlayerPrefs.GetFloat("initialMax"));
            growthFactor = Random.Range(PlayerPrefs.GetFloat("growthMin"), PlayerPrefs.GetFloat("growthMax"));
            maxX = Random.Range(PlayerPrefs.GetFloat("maxXMin"), PlayerPrefs.GetFloat("maxXMax"));*/
            initialValue = Random.Range(settings.initialValueMin, settings.initialValueMax);
            growthFactor = Random.Range(settings.growthFactorMin, settings.growthFactorMax);
            maxX = Random.Range(settings.maxXValueMin, settings.maxXValueMax);
        } else
        {
            initialValue = Random.Range(1000, 10000);
            growthFactor = Random.Range(0.02f, 0.08f);
            maxX = Random.Range(50, 100);
        }

        Debug.Log("Set values to: initial: " + initialValue + ", growth: " + growthFactor + "maxX: " + maxX + ", frequency: " + frequency);
    }

    /// <summary>
    /// Public function to be called by the countdown class, to cancel the calculation and go to the next, if the user took to long to answer.
    /// </summary>
    public void invalidInputCalculation()
    {
        calculationConfirmInput(false);
    }

    /// <summary>
    /// Same function as for the calculation, but for the investment.
    /// </summary>
    public void invalidInputInvestment()
    {
        investmentPicked(null, false); 
    }

    /// <summary>
    /// Sets the button to continue or replay the visualization to active. Is called by the visualization classes when the visualization is finished.
    /// </summary>
    public void activatContinueButton()
    {
        buttonsContinueReplayParent.SetActive(true);
    }

    public void saveFunctionValues(Dictionary<float,float> dict, string type)
    {
        Debug.Log("Saving function data");

        string identifier = Util.SaveFunctionValues(dict, type);

        savedData.addVisualizationValues(identifier);
        Util.WriteToOutputFile(savedData.SaveProgress("visualizationValueIdentifier"));
    }

    /// <summary>
    /// Calculates the correct answer for the exponential function with the current values. 
    /// </summary>
    /// <param name="afterYears">The previously randomized value for the time to pass.</param>
    /// <returns>The calculated value for exponential the function after x years.</returns>
    public float calculateCalculationResult(int afterYears)
    {
        float x = (float)afterYears;
        MainCalculator calc = new MainCalculator(initialValue, growthFactor, x, functionType, 0); /* Create a new calculator object for calculating the value, without noise */
        return calc.getMaxY();
    }
}
