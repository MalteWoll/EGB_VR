using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for calculating the exponential function values depending on the input of initial value, growth and speed.
/// </summary>
public class FunctionCalculator : MonoBehaviour
{
    [SerializeField]
    public float expFunc_initial; /* The initial, or starting value for the exponential function */
    [SerializeField]
    public float expFunc_growth; /* The growth rate of the exponential function */
    [SerializeField]
    private float expFunc_speed; /* A variable to multiply with the time value, to slow down or speed up the simulation */
    [SerializeField]
    private float frequency; /* The frequency in which the value from the exponential function is calculated */
    private float threshold; /* Helper variable for deciding whether to calculate in the current update call */
    [SerializeField]
    public float expFunc_maxX; /* Maximum x value for the exponential function, set to public for the graph class */

    public float expFunc_x = 0; /* The x, or time, value for the exponential function. Set to public because the graph class needs the value. */

    // These variables are public, as other classes need them for their functionalities
    public float expFunc_y; /* The y, or f(x), value for the exponential function */
    public int roundedY; /* Rounded valur for y, so the number of objects can be deployed */

    [SerializeField]
    public bool start; /* Set this to true when the start button is pressed */

    // Start is called before the first frame update
    void Start()
    {
        threshold = frequency; /* First threshold is required, since threshold is increments of the frequency, we set it to the initial frequency */
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            // Add the time between update calls to the time variable and multiply it with the speed factor
            expFunc_x += Time.deltaTime * expFunc_speed;

            // To avoid calculating the value for y in every single update call, only do so when x exceeds a fixed value
            if (expFunc_x > threshold && expFunc_x < expFunc_maxX)
            {
                threshold += frequency; /* Increase the threshold value by the frequency value */

                // Calculate the f(x) for x
                expFunc_y = Calculator.calculateExponentialFunctionValue(expFunc_x, expFunc_initial, expFunc_growth);

                roundedY = Mathf.RoundToInt(expFunc_y);
            }
        }
    }
}
