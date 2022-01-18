using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class for visualizing the (exponential) growth via graph, as a LineRenderer.
/// </summary>
public class VisualizationGraph : MonoBehaviour
{
    // Relevant objects and components on them
    [SerializeField]
    private GameObject mainControllerObject;
    private MainController mainController;

    [SerializeField]
    private GameObject plotPoint;
    private List<GameObject> plotPointList = new List<GameObject>();

    [SerializeField]
    private GameObject graphBackground;

    [SerializeField]
    private GameObject counterObject;
    private TextMeshProUGUI text;

    [SerializeField]
    private GameObject backgroundGraphParent; /* For easier coordinate transformation */

    private float maxX; /* The highest value for x, used for scaling */
    private float maxY; /* The highest value for y, for scaling as well */

    private float x;
    private float y;

    //private LineRenderer lineRenderer; /* The line renderer for drawing the graph */

    private Vector3 graphZero; /* The (0|0) value in the coordinate system on the graph */

    private Vector3 maxValueGraph; /* The position of the 'end' on the graph */

    private float scalingX;
    private float scalingZ;
    int i = 0; /* Counter for line renderer positions */

    [SerializeField]
    private float frequency;
    private float frequencyThreshold = 0;
    [SerializeField]
    private float speed;

    private float noiseLevel;

    private MainCalculator calculator;

    private bool finished = false;
    private bool saved = false;

    [SerializeField]
    private GameObject textYearValueParent;
    private TextMeshProUGUI textYearValue;

    [SerializeField]
    private GameObject stockIdentA;
    [SerializeField]
    private GameObject stockIdentB;

    void Start()
    {
        // Get the scripts as components of the objects
        mainController = mainControllerObject.GetComponent<MainController>();

        text = counterObject.GetComponent<TextMeshProUGUI>();

        textYearValue = textYearValueParent.GetComponent<TextMeshProUGUI>();
        textYearValue.text = "0";

        noiseLevel = mainController.noiseLevel;

        speed = mainController.speed;
        frequency = mainController.frequency;

        // Create a new calculator object and fill the constructor with the values from the main controller
        calculator = new MainCalculator(mainController.initialValue, mainController.growthFactor, mainController.maxX, mainController.functionType, noiseLevel);
        Debug.Log("Graph visualization, values used: Intial: " + mainController.initialValue + ", growth: " + mainController.growthFactor + ", speed: " + mainController.speed 
            + ", frequency: " + mainController.frequency + ", maxX: " + mainController.maxX + ", type: " + mainController.functionType);

        // Get the maximum values for x and y
        maxX = mainController.maxX;
        maxY = calculator.getMaxY();

        scalingX = -10 / maxX; /* The size of the graph is always 10 wide and 10 high, bottom left is (-5,0,-5), top right is (5,0,5) */
        scalingZ = 10 / maxY; /* Therefore, -10 and 10 can be divided by the maximum value of the axis for scaling */

        // Since 0|0 is the center of the object and the length and width is always 10, the following vector is '0' in the coordinate system, i.e. the bottom left corner of the graph
        graphZero = new Vector3(-5, 0.02f, -5);

        /*LineRenderer lineRenderer = backgroundGraphParent.GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, graphZero + new Vector3(-scalingX * 0, 0, scalingZ * mainController.initialValue));
        lineRenderer.SetPosition(1, graphZero + new Vector3(-scalingX * mainController.maxX, 0, scalingZ * calculator.getMaxY()));
        lineRenderer.startWidth = 0.02f;*/

    }

    void Update()
    {
        x += Time.deltaTime * speed; /* This is where the speed value manipulates the function */

        if (x > frequencyThreshold && x <= maxX) /* To increase performance and make things easier to follow, only calculate the function value when a certain threshold is passed */
        {
            frequencyThreshold += frequency;

            if (!finished)
            {
                y = calculator.getY(x); /* Calculate the current y value */
            } else
            {
                y = calculator.getYAgain();
            }

            Vector3 newPosition = graphZero + new Vector3(-scalingX * x, 0, scalingZ * y); /* Calculate the position on the graph in relation to the value for zero with the previously calculated scaling values */

            GameObject tempPoint = Instantiate(plotPoint, newPosition, Quaternion.identity);
            tempPoint.transform.SetParent(backgroundGraphParent.transform, false);

            plotPointList.Add(tempPoint);

            i++; /* Increase position count of the line renderer */

            text.text = y.ToString("F0") + "$";
            textYearValue.text = x.ToString("F0");
        } else
        {
            if (!finished && x >= maxX) /* To only call the activation of the continue button once, use a boolean that is set to true after activation */
            {
                Debug.Log("FINISHED, SAVING");

                PlayerPrefs.SetString("maxY", y.ToString("F0"));
                PlayerPrefs.Save();

                if (!saved)
                {
                    // Save the values of the equation by calling the function in the main controller
                    mainController.saveFunctionValues(calculator.getValueDict(),"graph");
                    saved = true;
                }

                //calculator.showValues();

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
    /// Resets the lineRenderer and all values related to it.
    /// </summary>
    public void replay()
    {
        x = 0;
        frequencyThreshold = 0;
        i = 0;
        calculator.resetDictCounter();
        
        foreach(GameObject point in plotPointList)
        {
            Destroy(point.gameObject);
        }
    }

    public void reset()
    {
        x = 0;
        frequencyThreshold = 0;
        i = 0;

        calculator = new MainCalculator(mainController.initialValue, mainController.growthFactor, mainController.maxX, mainController.functionType, noiseLevel);
        Debug.Log("Graph visualization, values used: Initial: " + mainController.initialValue + ", growth: " + mainController.growthFactor + ", speed: " + mainController.speed
            + ", frequency: " + mainController.frequency + ", maxX: " + mainController.maxX + ", type: " + mainController.functionType);

        maxX = mainController.maxX;
        maxY = calculator.getMaxY();

        scalingX = -10 / maxX;
        scalingZ = 10 / maxY;

        foreach (GameObject point in plotPointList)
        {
            Destroy(point.gameObject);
        }

        finished = false;
        saved = false;
    }

    public void saveFile()
    {
        if(!saved)
        {

        }
    }

    public void enableStockIdentA()
    {
        stockIdentB.SetActive(false);
        stockIdentA.SetActive(true);
    }

    public void enableStockIdentB()
    {
        stockIdentA.SetActive(false);
        stockIdentB.SetActive(true);
    }
}
