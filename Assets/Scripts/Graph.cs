using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for controlling the graph in the animation.
/// </summary>
public class Graph : MonoBehaviour
{
    public GameObject functionCalculator; /* The game object with the script calculating the current value of the exponential function */
    public GameObject prefab_plotPoint;

    private float expFunc_y; /* The current y value of the exponential function */
    private float expFunc_x; /* The current x value of the exponential function */

    private float expFunc_tempY; /* variable for storing the last y value in */

    // For scaling purposes, we require the maximum values of both x and y beforehand
    private float expFunc_maxX;
    private float expFunc_maxY;

    private Vector3 graph_zero = new Vector3(-20, 0, 50); /* Zero in the coordinate system, hardcoded for now */
    // Maximum x and y value of the graph in units, hardcoded for now
    private float graph_maxX = 40;
    private float graph_maxY = 30;

    // The unit length for x and y, calculated by scaling with the maximum x and y value of the function
    private float graph_scaleX;
    private float graph_scaleY;

    private float backgroundZValue;

    private List<Vector3> plotPointPositions = new List<Vector3>(); /* A list to store the plot point positions in */
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Get the maximum x value from the calculator class
        expFunc_maxX = functionCalculator.GetComponent<FunctionCalculator>().expFunc_maxX;

        // The calculator class does not calculate the maximum y value, so we do this here with the helper class and the public variables for initial value and growth
        expFunc_maxY = Calculator.calculateExponentialFunctionValue(expFunc_maxX,
                                                                    functionCalculator.GetComponent<FunctionCalculator>().expFunc_initial,
                                                                    functionCalculator.GetComponent<FunctionCalculator>().expFunc_growth);

        expFunc_tempY = functionCalculator.GetComponent<FunctionCalculator>().expFunc_initial;

        Debug.Log(this.transform.lossyScale.x + ", " + this.transform.lossyScale.y + ", " + this.transform.lossyScale.z);

        // Calculate the scaling values
        graph_scaleX = graph_maxX / expFunc_maxX;
        graph_scaleY = graph_maxY / expFunc_maxY;

        backgroundZValue = this.transform.position.z;

        // Set the plot point for the initial value
        initialPosition = new Vector3(graph_zero.x, /* The initial values x value is zero */
                                      graph_zero.y + functionCalculator.GetComponent<FunctionCalculator>().expFunc_initial * graph_scaleY, /* Scaling is required on the y axis */
                                      graph_zero.z);
        Instantiate(prefab_plotPoint, initialPosition, Quaternion.identity);
        plotPointPositions.Add(initialPosition);
        expFunc_tempY = functionCalculator.GetComponent<FunctionCalculator>().expFunc_initial; /* Set the temporary y value to the initial value of the function */
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the calculator returned a new y value, to make sure a pairing of a x and a y value exists
        if (functionCalculator.GetComponent<FunctionCalculator>().expFunc_y > (expFunc_tempY))
        {
            expFunc_x = functionCalculator.GetComponent<FunctionCalculator>().expFunc_x;
            expFunc_y = functionCalculator.GetComponent<FunctionCalculator>().expFunc_y;

            // Calculate the plot point position by scaling with the previously calculated scaling factors and adding the value to the starting point of the graph
            Vector3 currentPlotPosition = new Vector3(graph_zero.x + expFunc_x * graph_scaleX,
                                                        graph_zero.y + expFunc_y * graph_scaleY,
                                                        graph_zero.z);
            // Instantiate a prefab for the plot point
            Instantiate(prefab_plotPoint, currentPlotPosition, Quaternion.identity);
            // Add the position to the list of points
            plotPointPositions.Add(currentPlotPosition);

            expFunc_tempY = functionCalculator.GetComponent<FunctionCalculator>().expFunc_y;
        }
    }
}
