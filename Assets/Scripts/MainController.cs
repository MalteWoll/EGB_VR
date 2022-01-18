using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using OVR;

public class MainController : MonoBehaviour
{
    // Objects for data import
    private TextAsset jsonFile;
    // Values for the exponential functions for the visualization
    private ExponentialFunctions exponentialFunctionsInJson;
    private List<ExponentialFunction> exponentialFunctionsDataList = new List<ExponentialFunction>();
    // Values for the questions in the calculations
    private CalculationQuestions calculationQuestionsInJson;
    private List<CalculationQuestion> calculationQuestionsDataList = new List<CalculationQuestion>();
    private int calculationQuestionListCounter = 0;

    // Object for saving data
    private SavedData savedData = new SavedData();

    private List<int> visualizationList = new List<int>(); /* There are three allowed values on the list: 0 for equation visualization, 1 for graph, 2 for interactive VR, used values are deleted */
    private int visualizationListCounter = 0;
    private int calculationCounter = 0; /* Multiple calculations are needed for each visualization, so here we keep track of them */
    private int maxCalculations = 3; /* The maximum number of calculations. It should be hardcoded and always the same, just to be prepared this variable exists. */
    private int investmentCounter = 0; /* Same as for the calculations, multiple for each visualization, this keeps track of where we are at the moment */
    private int maxInvestments = 3; /* Maximum number of investments, same as for the calculations above */

    private bool visualizationUsedBefore = false;

    [SerializeField]
    private GameObject countdownSoundParent;
    private CountdownSound countdownSound;

    [SerializeField]
    private GameObject buttonSoundParent;
    private ButtonSound buttonSound;

    [SerializeField]
    private GameObject instructionsParent;
    [SerializeField]
    private GameObject instructionsTextParent;
    private TextMeshProUGUI instructionsText;
    [SerializeField]
    private GameObject instructionsContinueParent;

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
    [SerializeField]
    private GameObject sliderParent;
    [SerializeField]
    private GameObject sliderMainParent;
    private InputSlider inputSlider;

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
    private GameObject investmentTwoImagesButtonPanelParent;
    [SerializeField]
    private GameObject investmentPickButtonLeft;
    [SerializeField]
    private GameObject investmentPickButtonRight;

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
    public string functionType;
    [SerializeField]
    public float noiseLevel; /* As percentage */

    private float correctResult;

    [SerializeField]
    private GameObject centerEyeObject; /* The center eye object in the VR rig structure, TODO: Does not have to be serialized, remove after testing. */

    private bool firstUpdate = true;
    private float setHeightTimer = 0;

    private float timeForTask = 0;

    private bool firstVisualization = true;
    private bool firstCalculation = true;

    // TODO: DEBUG, delete after testing
    [SerializeField]
    private GameObject textDebugParent;
    private string currentState;
    [SerializeField]
    private GameObject investmentPickedDefaultButton;

    private bool finished = false;
    private float endCountdown = 0;

    [SerializeField]
    private int runthroughAmount;

    [SerializeField]
    public int goldBarScaling;

    private string stockIdent;
    private void Start()
    {
        Debug.Log(Application.persistentDataPath);

        Util.LoadSettingsJSON();

        speed = PlayerPrefs.GetFloat("speed");
        frequency = PlayerPrefs.GetFloat("frequency");
        goldBarScaling = PlayerPrefs.GetInt("goldBarScaling");

        /*
        // Load data, start with exponential function data
        exponentialFunctionsInJson = JsonUtility.FromJson<ExponentialFunctions>(Util.LoadResourceTextFile("exponentialFunctionsData.json"));
        // After parsing the json file, add each set of data to a list
        foreach(ExponentialFunction exponentialFunction in exponentialFunctionsInJson.exponentialFunctions)
        {
            //Debug.Log("Found exponential function " + exponentialFunction.identifier + " with initial value " + exponentialFunction.initialValue + ", growth factor "
                //+ exponentialFunction.growthFactor + ", maximum x " + exponentialFunction.maxX + ", speed " + exponentialFunction.speed + " and frequency " + exponentialFunction.frequency);
            exponentialFunctionsDataList.Add(exponentialFunction);
        }
        calculationQuestionsInJson = JsonUtility.FromJson<CalculationQuestions>(Util.LoadResourceTextFile("calculationsQuestionsData.json"));
        foreach(CalculationQuestion calculationQuestion in calculationQuestionsInJson.calculationQuestions)
        {
            //Debug.Log("Found question with identifier " + calculationQuestion.identifier + ". Question is: '" + calculationQuestion.question + "'");
            calculationQuestionsDataList.Add(calculationQuestion);
        }*/

        inputSlider = sliderParent.GetComponent<InputSlider>();

        // Get the data from the intro and save it to the object for data saving
        savedData.Age = PlayerPrefs.GetInt("age");
        savedData.Gender = PlayerPrefs.GetString("gender");

        // Get the current time as start time
        savedData.Starttime = Util.getCurrentDateAndTime();

        // Make the initial save in the file
        Util.WriteToOutputFile(savedData.SaveProgress("initial"));

        // Randomize the order of the items on the lists with the imported data
        exponentialFunctionsDataList = Util.shuffleList(exponentialFunctionsDataList);
        calculationQuestionsDataList = Util.shuffleList(calculationQuestionsDataList);

        List<int> partialList = new List<int>();

        // When first starting this (and the experiment), fill the list for the visualization options, then shuffle it. This way, the order of the visualizations is always randomized.
        for (int j = 0; j < runthroughAmount; j++)
        {
            partialList.Add(0);
            partialList.Add(1);
            partialList.Add(2);
            partialList = Util.shuffleList(partialList);

            for(int k = 0; k < 3; k++)
            {
                visualizationList.Add(partialList[k]);
            }

            partialList = new List<int>();

            //visualizationList.Add(0); /* 0 = equation */
            //visualizationList.Add(1); /* 1 = graph */
            //visualizationList.Add(2); /* 2 = interactive */
        }

        //visualizationList = Util.shuffleList(visualizationList); /* Randomize the order of the values on the list */

        // Because of later changes, every visualization is used twice. To comply with existing code, we simply expand the list by adding the same value after each value once, for example: {1,3,2} -> {1,1,3,3,2,2}
        List<int> tempList = new List<int>();
        for(int i = 0; i < visualizationList.Count; i++)
        {
            tempList.Add(visualizationList[i]);
            tempList.Add(visualizationList[i]);
        }
        visualizationList = tempList;

        // TODO: REMOVE!
        visualizationList = new List<int> { 1,1,0,0,2,2, 1, 1, 0, 0, 2, 2 };

        // Get the sound objects
        countdownSound = countdownSoundParent.GetComponent<CountdownSound>();
        buttonSound = buttonSoundParent.GetComponent<ButtonSound>();

        // Get the different classes for visualization from the components of the GameObjects
        visualizationEquation = visualizationEquationParent.GetComponent<VisualizationEquation>();
        visualizationGraph = visualizationGraphParent.GetComponent<VisualizationGraph>();
        visualizationInteractive = visualizationInteractiveParent.GetComponent<VisualizationInteractive>();

        // Get the text element of the calculation step
        textCalculation = textCalculationObject.GetComponent<TextMeshProUGUI>();
        textCalculationAnswer = textCalculationAnswerParent.GetComponent<TextMeshProUGUI>();

        // Get the instruction text component
        instructionsText = instructionsTextParent.GetComponent<TextMeshProUGUI>();

        // Set the first textual instruction
        instructionsText.text = Util.GetInstructionalText("previsualization");
        currentState = "instructions";

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

        textDebugParent.GetComponent<TextMeshProUGUI>().text = visualizationListCounter + "," + calculationCounter + "," + investmentCounter; /* TODO: Delete after testing */

        // When the user presses one of the buttons on the controller, the buttons etc. are being moved in height, in case they are still in a position uncomfortable for the user
        if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three)) /* Sometimes error messages regarding OVR? Program still compiling anyways */
        {
            Debug.Log("Center Eye Position: " + centerEyeObject.transform.position.ToString());
            setButtonHeights(); /* Sets the height of all control elements and buttons according to the current height of the HMD */
        }

        if(textCalculationAnswerParent.activeSelf)
        {
            textCalculationAnswer.text = inputSlider.currentValue.ToString("F2");
        }

        timeForTask += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.C))
        {
            switch(currentState)
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

        if(Input.GetKeyDown(KeyCode.B)) {
            if(currentState == "visualization")
            {
                replayVisualization();
            }
        }

        if(finished)
        {
            endCountdown += Time.deltaTime;
            int temp = 30 - (int)endCountdown;
            instructionsText.text = Util.GetInstructionalText("ending") + "\n\n Application will restart in " + temp + " seconds.";
            if (endCountdown > 30)
            {
                SceneManager.LoadScene("Tutorial");
            }
        }
    }

    /// <summary>
    /// Activate a GameObject in relation to the value of the argument. The GameObject/number represent a type of visualization.
    /// </summary>
    /// <param name="option"></param>
    private void startVisualization()
    {
        currentState = "visualization";
        if (visualizationListCounter < 6 * runthroughAmount)
        {
            currentVisualization = visualizationList[visualizationListCounter]; /* Get the value for the type of visualization to use */
            if (!visualizationUsedBefore)
            {
                setVisualizationData();
            }

            // Every visualization is used twice with a exponential and a logartithmic function. The following block decides what to use by randomization.
            if (!visualizationUsedBefore)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    functionType = "exp";
                }
                else
                {
                    functionType = "log";
                }

                if (Random.Range(0f, 1f) > 0.5f)
                {
                    stockIdent = "stockA";
                }
                else
                {
                    stockIdent = "stockB";
                }

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

                if(stockIdent == "stockA")
                {
                    stockIdent = "stockB";
                } else
                {
                    stockIdent = "stockA";
                }
                visualizationUsedBefore = false;
            }

            switch (currentVisualization)
            {
                case 0:
                    visualizationEquationParent.SetActive(true);
                    if(!visualizationUsedBefore) { visualizationEquation.reset(); }
                    //buttonsContinueReplayParent.SetActive(true);
                    currentVisualizationGameObject = visualizationEquationParent;
                    // Save the visualization in the object for saving
                    savedData.addVisualization("equation");
                    savedData.addVisualizationStockIdentifier(stockIdent);
                    if(stockIdent == "stockA") { visualizationEquation.enableStockIdentA(); } else { visualizationEquation.enableStockIdentB(); }

                    Debug.Log("Starting equation visualization");
                    break;
                case 1:
                    visualizationGraphParent.SetActive(true);
                    if (!visualizationUsedBefore) { visualizationGraph.reset(); }            
                    //buttonsContinueReplayParent.SetActive(true);
                    currentVisualizationGameObject = visualizationGraphParent;
                    // Save the visualization in the object for saving
                    savedData.addVisualization("graph");
                    savedData.addVisualizationStockIdentifier(stockIdent);
                    if (stockIdent == "stockA") { visualizationGraph.enableStockIdentA(); } else { visualizationGraph.enableStockIdentB(); }

                    Debug.Log("Starting graph visualization");
                    break;
                case 2:
                    visualizationInteractiveParent.SetActive(true);
                    if (!visualizationUsedBefore) { visualizationInteractive.reset(); }
                    //buttonsContinueReplayParent.SetActive(true);
                    currentVisualizationGameObject = visualizationInteractiveParent;
                    // Save the visualization in the object for saving
                    savedData.addVisualization("interactive");
                    savedData.addVisualizationStockIdentifier(stockIdent);
                    if (stockIdent == "stockA") { visualizationInteractive.enableStockIdentA(); } else { visualizationInteractive.enableStockIdentB(); }

                    Debug.Log("Starting interactive visualization");
                    break;
                case 3: /* This will never be reached, will it? */
                    // TODO: End, maximum number of visualizations reached.
                    Debug.Log("Finished all visualizations.");
                    break;
                default:
                    Debug.LogError("Value " + currentVisualization + " in switch case in startVisualization(), something went wrong.");
                    break;
            }

            // These three can be added here, since it does not matter what visualization they are used for
            savedData.addVisualizationType(functionType);
            Util.WriteToOutputFile(savedData.SaveProgress("visualizationType"));
            savedData.addVisualizationInitial(initialValue.ToString());
            Util.WriteToOutputFile(savedData.SaveProgress("visualizationInitial"));
            savedData.addVisualizationGrowth(growthFactor.ToString());
            Util.WriteToOutputFile(savedData.SaveProgress("visualizationGrowth"));

            // Make the partial save
            Util.WriteToOutputFile(savedData.SaveProgress("visualization"));

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
        currentState = "calculation";
        calculationParent.SetActive(true);
        sliderMainParent.SetActive(true);
        //numPadParent.SetActive(true);
        //numPadConfirmParent.SetActive(false); /* Hide the initial 'Confirm' button, so the user has to input something, and to prevent accidentally confirming multiple times */
        // TODO: Check if that is ok, or if user should have the option to skip. If so, build a sleeper function to prevent skipping by accident. */
        // This will be removed if we keep the slider

        int afterYears = Random.Range(20, 100); /* TODO: Should this be randomized? */

        // Calculate 'correct' value for the prompt
        correctResult = calculateCalculationResult(afterYears);

        string tempMaxY = PlayerPrefs.GetString("maxY"); /* Get the saved maximum value reached by the visualization (script) */

        inputSlider.setSliderValues(0, 20000);

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
        if (valid) /* If the user answered in the appropriate time */
        {
            string time = timeForTask.ToString("F2"); /* Convert the float value for the time needed to a string with two decimals */

            // Add the answer and the time needed to the save data object and add it to the save file
            //savedData.addCalculationResult(textCalculationAnswer.text);
            savedData.addCalculationResult(inputSlider.currentValue.ToString("F2"));
            savedData.addCalculationTime(time);
            savedData.addCalculationCorrectResult(correctResult.ToString("F2"));
            Util.WriteToOutputFile(savedData.SaveProgress("calculationResult"));
        } else /* If the user did not answer in time (which is not enabled at the moment, TODO: Delete if not used in the final version) */
        {
            // Save an appropriate remark as result for the calculation
            savedData.addCalculationResult("time expired");
            Util.WriteToOutputFile(savedData.SaveProgress("calculationResult"));
        }

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
        else
        {
            calculationCounter++;
            startVisualization();
        }
    }

    private void startInvestment()
    {
        Debug.Log("Start investment");
        currentState = "investment";

        investmentTwoImagesParent.SetActive(true);

        investmentPickButtonLeft.SetActive(false);
        investmentPickButtonRight.SetActive(false);

        StartCoroutine(waitSecondsBeforeEnable(investmentPickButtonLeft, investmentPickButtonRight, 3)); /* Wait 3 seconds before enabling the buttons to pick an investment */

        currentInvestmentObject = investmentTwoImagesParent;

        timeForTask = 0;
    }

    private void investmentPicked(GameObject button, bool valid)
    {
        if (valid) /* For now, all inputs are valid, as the countdown has been replaced with measuring the time. I'll keep this, in case we change it again */
        {
            string time = timeForTask.ToString("F2"); /* Convert the float value for the time needed to a string with two decimals */
            savedData.addInvestmentTime(time); /* Add the time to the object for saving data */

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
        } else
        {
            // If the user did not answer in time, add a remark as result
            // TODO: Delete if not used
            savedData.addInvestmentResult("time expired");
            Util.WriteToOutputFile(savedData.SaveProgress("investmentResults"));
        }

        currentInvestmentObject.SetActive(false); /* After picking, disable the GameObject */
        investmentCounter++; /* Increase the counter by one */

        // Start the next visualization
        startVisualization();
    }

    private void saveAndExit()
    {
        finished = true;
        savedData.Endtime = Util.getCurrentDateAndTime();

        Util.WriteToOutputFile(savedData.SaveProgress("finish"));

        instructionsParent.SetActive(true);
        instructionsText.text = Util.GetInstructionalText("ending");
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

    private void continueFromVisualization()
    {
        if (visualizationInteractiveParent.activeSelf) /* The interactive visualization is the only one that leaves active GameObjects behind after disabling, these need to be destroyed */
        {
            visualizationInteractive.destroyObjects();
        } else
        {
            if(visualizationGraphParent.activeSelf)
            {
                visualizationGraph.reset();
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
            instructionsText.text = Util.GetInstructionalText("calculations");
            currentState = "instructions";
        }
    }

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
        initialValue = Random.Range(PlayerPrefs.GetFloat("initialMin"), PlayerPrefs.GetFloat("initialMax"));
        growthFactor = Random.Range(PlayerPrefs.GetFloat("growthMin"), PlayerPrefs.GetFloat("growthMax"));
        maxX = Random.Range(PlayerPrefs.GetFloat("maxXMin"), PlayerPrefs.GetFloat("maxXMax"));

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
    /// Calculates the correct answer to the calculation prompt.
    /// </summary>
    /// <param name="afterYears">The previously randomized value for the time to pass.</param>
    /// <returns>The calculated value for the function after x years.</returns>
    public float calculateCalculationResult(int afterYears)
    {
        float x = (float)afterYears;
        MainCalculator calc = new MainCalculator(initialValue, growthFactor, x, functionType, 0); /* Create a new calculator object for calculating the value */
        return calc.getMaxY();
    }
}
