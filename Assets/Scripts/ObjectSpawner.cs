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
    private float frequency; /* The frequency in which the value from the exponential function is calculated */
    private float threshold; /* Helper variable for deciding whether to calculate and create objects */

    public GameObject prefab_object; /* The gameobject that holds the prefab for the objects appearing in the simulation */

    [SerializeField]
    private List<SimulationObject> simulationObjectList; /* The list of all objects that are created depending on the values of the exponential function */

    private float expFunc_x = 0; /* The x, or time, value for the exponential function */
    private float expFunc_y; /* The y, or f(x), value for the exponential function */
    private int roundedY; /* Rounded valur for y, so the number of objects can be deployed */
    private int highestY = 0; /* To hold the highest rounded value the function reached so far */

    [SerializeField]
    private bool start; /* Set this true when the start button is pressed */

    void Start()
    {
        threshold = frequency;
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
                threshold += frequency;

                // Calculate the f(x) for x
                expFunc_y = calculateExpFuncY(expFunc_x);
                Debug.Log("y value: " + expFunc_y);

                roundedY = Mathf.RoundToInt(expFunc_y);

                // If the rounded y value is higher than the highest reached so far, calculate the difference between highest and second highest and deploy that amount of objects
                if(roundedY > highestY)
                {
                    for(int i = 0; i < (roundedY - highestY); i++)
                    {
                        // To not instantiate all objects in each other, every loop the object is instantiated one length higher
                        // TODO: replace the hardcoded 'i*0.3f' with the size of the object
                        //Instantiate(prefab_object, new Vector3(this.transform.position.x, this.transform.position.y + i*0.3f, this.transform.position.z), Quaternion.identity);
                        SimulationObject simulationObject = Instantiate(prefab_object, 
                                                                        new Vector3(this.transform.position.x, this.transform.position.y + i * 0.3f, this.transform.position.z), 
                                                                        Quaternion.identity).GetComponent<SimulationObject>();
                        simulationObjectList.Add(simulationObject);
                    }

                    highestY = roundedY;
                }
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
