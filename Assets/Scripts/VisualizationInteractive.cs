using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisualizationInteractive : MonoBehaviour
{
    [SerializeField]
    private GameObject mainControllerObject;
    private MainController mainController;

    [SerializeField]
    private GameObject prefab_object; /* The gameobject that holds the prefab for the objects appearing in the simulation */
    private float prefab_height; /* The height of the prefab object */
    private float prefab_width; /* The length of the side of the prefab object */
    private float prefab_length; /* The length of the other side of the prefab object */

    private MainCalculator calculator; /* The calculator class, calculating the y values for an (exponential) function */

    [SerializeField]
    private List<SimulationObject> simulationObjectList; /* The list of all objects that are created depending on the values of the function. Does not have to be serialized, TODO: remove after testing */

    [SerializeField]
    private GameObject counterObject;
    private TextMeshProUGUI text;

    private int highestY = 0; /* To hold the highest rounded value the function reached so far */
    private int roundedY; /* rounded up value for y, since we can't create fractions of objects */

    private int spawnerGridCounter = 0;
    private List<Vector3> spawnerGrid = new List<Vector3>(); /* List for varying the spawn positions within a squared grid */

    private float layer = 1;

    [SerializeField]
    private int gridLength; /* Length of the sides of the squared grid, should be an odd number, so the parent transform is always in the middle */
    [SerializeField]
    private int gridWidth;

    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private float speed; /* The factor the value for time passed in multiplied with, to speed up or slow down the visualization */
    [SerializeField]
    private float frequency;
    private float frequencyThreshold = 0;

    [SerializeField]
    private float x;
    [SerializeField]
    private float maxX;

    private float noiseLevel;

    private Renderer renderer;
    private Vector3 rendererSize;

    private GameObject simulationObjectParent;

    bool finished = false;
    private bool saved = false;
    private bool kinematicDisabled = false;
    private int goldBarScaling;

    [SerializeField]
    private GameObject textYearValueParent;
    private TextMeshProUGUI textYearValue;

    void Start()
    {
        // Get the script as components of the objects
        mainController = mainControllerObject.GetComponent<MainController>();

        noiseLevel = mainController.noiseLevel;

        textYearValue = textYearValueParent.GetComponent<TextMeshProUGUI>();
        textYearValue.text = "0";

        text = counterObject.GetComponent<TextMeshProUGUI>();

        // Get the renderer and calculate the dimensions of the prefab object with it
        renderer = prefab_object.GetComponent<Renderer>();
        rendererSize = renderer.bounds.size;
        Debug.Log("Renderer size: " + rendererSize.ToString());

        prefab_height = rendererSize.y;
        prefab_width = rendererSize.x;
        prefab_length = rendererSize.z;

        prefab_height = 0.352f; /* Height is not detected correctly with the gold bar prefab. TODO: maybe find a better way than hardcoding here? */

        // Make sure the grid length is an odd number
        if (gridLength % 2 == 0) { gridLength++; }

        Vector3 gridStartPosition = new Vector3(startPosition.x - (((float)gridLength / 2 - 0.5f) * prefab_width),
                                        startPosition.y + (prefab_height / 2),
                                        startPosition.z - (((float)gridLength / 2 - 0.5f)) * prefab_length);

        // Filling the list for the spawner grid with entries around the position of the parent GameObject with a previously specified size (by squaring the length, so that it is always squared)
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                Vector3 currentPosition = gridStartPosition + new Vector3(prefab_width * j, 0, prefab_length * i);
                spawnerGrid.Add(currentPosition);
            }
        }

        maxX = mainController.maxX;
        calculator = new MainCalculator(mainController.initialValue, mainController.growthFactor, maxX, mainController.functionType, noiseLevel);
        Debug.Log("Interactive visualization, values used: Intial: " + mainController.initialValue + ", growth: " + mainController.growthFactor + ", speed: " + mainController.speed
            + ", frequency: " + mainController.frequency + ", maxX: " + mainController.maxX + ", type: " + mainController.functionType);

        speed = mainController.speed;
        frequency = mainController.frequency;
        goldBarScaling = mainController.goldBarScaling;

        simulationObjectParent = new GameObject();
    }

    void Update()
    {
        x += Time.deltaTime * speed;

        if (x > frequencyThreshold && x <= maxX)
        {
            if (!finished)
            {
                roundedY = calculator.getRoundedY(x);
            } else
            {
                roundedY = calculator.getRoundedYAgain();
            }

            if (roundedY > highestY)
            {
                float difference = ((float)roundedY - (float)highestY) / (float)goldBarScaling;
                for (int i = 0; i < (int)difference; i++)
                {
                    // Objects are instantiated layer by layer in the specified grid size around the spawner
                    SimulationObject simulationObject = Instantiate(prefab_object,
                                                                    //new Vector3(spawnerGrid[spawnerGridCounter].x, spawnerGrid[spawnerGridCounter].y * layer, spawnerGrid[spawnerGridCounter].z),
                                                                    new Vector3(spawnerGrid[spawnerGridCounter].x, startPosition.y + layer*prefab_height*0.5f, spawnerGrid[spawnerGridCounter].z),
                                                                    Quaternion.identity).GetComponent<SimulationObject>();

                    simulationObject.gameObject.transform.parent = simulationObjectParent.transform;

                    if (kinematicDisabled)
                    {
                        simulationObject.disableKinematic();
                    }
                    else
                    {
                        simulationObject.setKinematic();
                    }
                    simulationObjectList.Add(simulationObject);

                    // If the end of the grid position list is not yet reached, increase the counter, else reset it to zero and increase the one for layers
                    if (spawnerGridCounter < spawnerGrid.Count - 1)
                    {
                        spawnerGridCounter++;
                    }
                    else
                    {
                        spawnerGridCounter = 0;
                        layer++;
                    }
<<<<<<< Updated upstream
                    text.text = roundedY.ToString() + " $";
=======
                    text.text = roundedY.ToString() + "$";
                    textYearValue.text = x.ToString("F0");
>>>>>>> Stashed changes
                }
                highestY = roundedY;
            }
            frequencyThreshold += frequency;
        } else
        {
            if(!finished  && x >= maxX) /* To only call the activation of the continue button once, use a boolean that is set to true after activation */
            {
                PlayerPrefs.SetString("maxY", roundedY.ToString("F0"));
                PlayerPrefs.Save();

                if (!saved)
                {
                    // Save the values of the equation by calling the function in the main controller
                    mainController.saveFunctionValues(calculator.getValueDict(), "interactive");
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
    /// Resets the visualization, to start again.
    /// </summary>
    public void replay()
    {
        destroyObjects();

        highestY = 0;
        x = 0;
        frequencyThreshold = 0;

        spawnerGridCounter = 0;
        layer = 1;

        calculator.resetDictCounter();
    }

    /// <summary>
    /// Objects need to be deleted after the visualization is finished. 
    /// Seperate function as it is called from the MainController class as well.
    /// </summary>
    public void destroyObjects()
    {
        if (simulationObjectList.Count > 0)
        {
            simulationObjectList.Clear();

            foreach (Transform child in simulationObjectParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void reset()
    {
        destroyObjects();
        highestY = 0;
        x = 0;
        frequencyThreshold = 0;

        spawnerGridCounter = 0;
        layer = 1;

        maxX = mainController.maxX;
        calculator = new MainCalculator(mainController.initialValue, mainController.growthFactor, maxX, mainController.functionType, noiseLevel);
        Debug.Log("Interactive visualization, values used: Intial: " + mainController.initialValue + ", growth: " + mainController.growthFactor + ", speed: " + mainController.speed
            + ", frequency: " + mainController.frequency + ", maxX: " + mainController.maxX + ", type: " + mainController.functionType);

        finished = false;
        saved = false;
        enableKinematicForAll();
    }

    /// <summary>
    /// Call the function to deactivate kinematic and enable gravity for every gold bar (simulation object)
    /// </summary>
    public void disableKinematicForAll()
    {
        kinematicDisabled = true;
        foreach(SimulationObject simObj in simulationObjectList)
        {
            simObj.disableKinematic();
        }
    }

    public void enableKinematicForAll()
    {
        kinematicDisabled = false;
        foreach (SimulationObject simObj in simulationObjectList)
        {
            simObj.enableKinematic();
        }
    }
}
