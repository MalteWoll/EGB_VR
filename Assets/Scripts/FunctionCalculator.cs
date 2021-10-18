using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionCalculator : MonoBehaviour
{
    [SerializeField]
    private float expFunc_initial; /* The initial, or starting value for the exponential function */
    [SerializeField]
    private float expFunc_growth; /* The growth rate of the exponential function */
    [SerializeField]
    private float expFunc_speed; /* A variable to multiply with the time value, to slow down or speed up the simulation */
    [SerializeField]
    private float frequency; /* The frequency in which the value from the exponential function is calculated */
    private float threshold; /* Helper variable for deciding whether to calculate and create objects */

    private float expFunc_x = 0; /* The x, or time, value for the exponential function */

    // These are public, as other classes need them
    public float expFunc_y; /* The y, or f(x), value for the exponential function */
    public int roundedY; /* Rounded valur for y, so the number of objects can be deployed */

    [SerializeField]
    public bool start; /* Set this true when the start button is pressed */

    // Start is called before the first frame update
    void Start()
    {
        threshold = frequency;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            // Add the time between update calls to the time variable and multiply it with the speed factor
            expFunc_x += Time.deltaTime * expFunc_speed;

            // To avoid calculating the value for y in every single update call, only do so when x exceeds a fixed value
            if (expFunc_x > threshold)
            {
                threshold += frequency;

                // Calculate the f(x) for x
                expFunc_y = calculateExpFuncY(expFunc_x);
                Debug.Log("y value: " + expFunc_y);

                roundedY = Mathf.RoundToInt(expFunc_y);
            }
        }
    }

    private float calculateExpFuncY(float x)
    {
        return (expFunc_initial * Mathf.Pow((1 + expFunc_growth), x));
    }
}
