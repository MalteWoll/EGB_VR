using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationGraph : MonoBehaviour
{
    // Relevant objects and components on them
    [SerializeField]
    private GameObject mainControllerObject;
    private MainController mainController;

    [SerializeField]
    private GameObject graphBackground;

    private float maxX; /* The highest value for x, used for scaling */
    private float maxY; /* The highest value for y, for scaling as well */

    private float x;
    private float y;

    private LineRenderer lineRenderer; /* The line renderer for drawing the graph */

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

    private MainCalculator calculator;

    // Start is called before the first frame update
    void Start()
    {
        // Get the scripts as components of the objects
        mainController = mainControllerObject.GetComponent<MainController>();

        // Create a new calculator object and fill the constructor with the values from the main controller
        calculator = new MainCalculator(mainController.initialValue, mainController.growthFactor, mainController.speed, mainController.frequency, mainController.maxX);

        maxX = mainController.maxX;
        maxY = calculator.getMaxY();

        scalingX = -10 / maxX;
        scalingZ = 10 / maxY;

        // Line renderer presets:
        lineRenderer = graphBackground.GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = 0.01f;

        // Since 0|0 is the center of the object and the length and width is always 10, the following vector is '0' in the coordinate system, i.e. the bottom left corner of the graph
        graphZero = new Vector3(-5, 0, -5);
        lineRenderer.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime * speed; /* This is where the speed value manipulates the function */

        if (x > frequencyThreshold && x <= maxX) /* To increase performance and make things easier to follow, only calculate the function value when a certain threshold is passed */
        {
            lineRenderer.positionCount = i + 1; /* Add a position to the line renderer, must be filled immediately after, otherwise line to center of board */
            y = calculator.getY(x); /* Calculate the current y value */

            Vector3 newPosition = graphZero + new Vector3(-scalingX * x, 0, scalingZ * y);
            lineRenderer.SetPosition(i, newPosition); /* Set position */
            i++; /* Increase position count of the line renderer */
        }
    }

    public void replay()
    {

    }
}
