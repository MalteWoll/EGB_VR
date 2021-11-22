using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class displays the exponential function in the form of a graph on a 'whiteboard'.
/// </summary>
public class Whiteboard : MonoBehaviour
{
    private float expFunc_maxX; /* The highest value for x, used for scaling */
    private float expFunc_maxY; /* The highest value for y, for scaling as well */

    private float expFunc_x;
    private float expFunc_y;

    [SerializeField]
    private GameObject functionCalculatorObject; /* The object with the script calculating the exponential function and its values */
    [SerializeField]
    private GameObject whiteBoardPlane; /* The plane object of the whiteboard, containing a box collider */

    private Collider whiteBoardCollider; /* The collider of the whiteboard */
    private LineRenderer lineRenderer; /* The line renderer for drawing the graph */

    private Vector3 whiteBoardZero; /* The (0|0) value in the coordinate system on the whiteboard */

    // Random values for x and y to determine the position of the 'end' of the graph
    private float randomX;
    private float randomY;
    [SerializeField]
    private float randomMultiplier;
    [SerializeField]
    private float minEndPoint;

    private Vector3 maxValueWhiteboard; /* The position of the 'end' on the whiteboard */
    // Scaling values for x and z in relation to the whiteboard values
    // Possibly confusing: Because of the rotation, the y axis in the exponential function is the -x axis here, and the x axis in the function is the +z axis here
    private float scalingX;
    private float scalingZ;
    int i = 0; /* Counter for line renderer positions */

    private bool start; /* Grab the value from the calculator */
    private FunctionCalculator functionCalculator; /* The calculator class, for easier reading of the values */

    // Start is called before the first frame update
    void Start()
    {
        functionCalculator = functionCalculatorObject.GetComponent<FunctionCalculator>(); /* Grab the calculator class */

        // Get the maximum x value from the calculator class
        expFunc_maxX = functionCalculator.expFunc_maxX;

        // The calculator class does not calculate the maximum y value, so we do this here with the helper class and the public variables for initial value and growth
        expFunc_maxY = Calculator.calculateExponentialFunctionValue(expFunc_maxX,
                                                                    functionCalculator.expFunc_initial,
                                                                    functionCalculator.expFunc_growth);

        // Get the size of the whiteboard via the collider class
        whiteBoardCollider = whiteBoardPlane.GetComponent<Collider>();

        whiteBoardZero = new Vector3(5, 0, -5); /* The collider always has the dimensions x = 10, z = 10, and 0|0 is the center, so the bottom left is this value, calculated accordingly */

        // Random values for the 'end' of the graph on the whiteboard
        randomX = Random.value * randomMultiplier; 
        randomY = Random.value * randomMultiplier;
        maxValueWhiteboard = whiteBoardZero + new Vector3(-minEndPoint - randomX, 0, minEndPoint + randomY);
        Debug.Log("Random max value whiteboard: " + maxValueWhiteboard);

        // Scaling of both axis, determined by position of the 'end' point of the graph
        scalingX = (whiteBoardZero.x - maxValueWhiteboard.x) / expFunc_maxY;
        scalingZ = (maxValueWhiteboard.z - whiteBoardZero.z) / expFunc_maxX;

        // Get the line renderer on the whiteboard
        lineRenderer = whiteBoardPlane.GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false; /* Line renderer should use object space, to make drawing on the board easier */

        // Some line renderer presets
        lineRenderer.widthMultiplier = 0.02f;
        lineRenderer.positionCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        // Grab the start boolean value from the calculator
        start = functionCalculator.start;

        if(start)
        {
            if(functionCalculator.expFunc_y > expFunc_y) /* Only do something if something changed, otherwise endless horizontal line when finished */
            {
                lineRenderer.positionCount = i + 1; /* Add a position to the line renderer, must be filled immediately after, otherwise line to center of board */

                // Grab the current values of the exponential function
                expFunc_y = functionCalculator.expFunc_y;
                expFunc_x = functionCalculator.expFunc_x;

                Vector3 newPosition = whiteBoardZero + new Vector3(-scalingX * expFunc_y, 0, scalingZ * expFunc_x); /* Calculate position according to scaling of x and z */
                lineRenderer.SetPosition(i, newPosition); /* Set position */
                i++;
            }
        }
    }
}
