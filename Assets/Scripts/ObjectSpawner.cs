using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Do we want other ways to import functions, or even let the user define data points and interpolate between them?

/// <summary>
/// This class controls the amount of objects appearing in the simulation, according to a exponential function. The function is (for now) defined by a starting value and a growth rate,
/// f(x) = a * ( 1 + r ) ^ x, or f(x) = a * b ^ x where b = 1 + r
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private float expFunc_initial; /* The initial, or starting value for the exponential function */
    [SerializeField]
    private float expFunc_growth; /* The growth rate of the exponential function */
    [SerializeField]
    private float expFunc_speed; /* A variable to multiply with the time value, to slow down or speed up the simulation */
    [SerializeField]
    private float thresholdCounter; /* To not calculate in every single update step, use this variable */
    private float threshold;

    public GameObject prefab_object; /* The gameobject that holds the prefab for the objects appearing in the simulation */

    private float expFunc_x = 0; /* The x, or time, value for the exponential function */
    private float expFunc_y; /* The y, or f(x), value for the exponential function */

    private bool start; /* Set this true when the start button is pressed */


    void Start()
    {
        threshold = thresholdCounter;
    }


    void Update()
    {
        if(start)
        {
            // Add the time between update calls to the time variable and multiply it with the speed factor
            expFunc_x += Time.deltaTime * expFunc_speed;

            // To avoid calculating the value for y in every single update call, only do so when x exceeds a fixed value
            if(expFunc_x > threshold)
            {
                threshold += thresholdCounter;

                // Calculate the f(x) for x
                expFunc_y = calculateExpFuncY(expFunc_x);
                Debug.Log("y value: " + expFunc_y);
            }
        }
    }

    /// <summary>
    /// Method for starting the simulation, does not work as a listener in the code for some reason, so the onClick() is set in the inspector of the button.
    /// </summary>
    public void startButtonPressed()
    {
        start = true;
    }

    private float calculateExpFuncY(float x)
    {
        return (expFunc_initial * Mathf.Pow((1 + expFunc_growth), x));
    }
}
