using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class displays the exponential function in the form of a graph on a 'whiteboard'. It also allows the user to continue drawing the graph.
/// </summary>
public class Whiteboard : MonoBehaviour
{
    private float expFunc_maxX; /* The highest value for x, used for scaling */
    private float expFunc_maxY; /* The highest value for y, for scaling as well */

    [SerializeField]
    private GameObject functionCalculator; /* The object with the script calculating the exponential function and its values */

    [SerializeField]
    private float expFunc_bufferX; /* The amount of 'buffer' applied to the exponential function, for example if the highest x value is 10, a buffer of 5 sets the highest value to 15, so the user has enough room to continue the graph */

    // Start is called before the first frame update
    void Start()
    {
        // Get the maximum x value from the calculator class
        expFunc_maxX = functionCalculator.GetComponent<FunctionCalculator>().expFunc_maxX;

        // The calculator class does not calculate the maximum y value, so we do this here with the helper class and the public variables for initial value and growth
        expFunc_maxY = Calculator.calculateExponentialFunctionValue(expFunc_maxX,
                                                                    functionCalculator.GetComponent<FunctionCalculator>().expFunc_initial,
                                                                    functionCalculator.GetComponent<FunctionCalculator>().expFunc_growth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
