using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Do we want other ways to import functions, or even let the user define data points and interpolate between them?

/// <summary>
/// This class controls the amount of objects appearing in the simulation, according to a exponential function. The function is (for now) defined by a starting value and a growth rate,
/// f(x) = a * ( 1 + r ) ^ x, or f(x) = a * b ^ x where b = 1 + r
/// </summary>
public class ObjectSpawnerTop : MonoBehaviour
{
    public GameObject prefab_object; /* The gameobject that holds the prefab for the objects appearing in the simulation */
    public GameObject calculator; /* The gameobject with the script calculating the values */

    [SerializeField]
    private List<SimulationObject> simulationObjectList; /* The list of all objects that are created depending on the values of the exponential function */

    private int highestY = 0; /* To hold the highest rounded value the function reached so far */

    private bool start; /* Set this true when the start button is pressed */
    private int roundedY;

    void Start()
    {

    }


    void Update()
    {
        roundedY = calculator.GetComponent<FunctionCalculator>().roundedY;
        start = calculator.GetComponent<FunctionCalculator>().start;

        if(start)
        {  
            // If the rounded y value is higher than the highest reached so far, calculate the difference between highest and second highest and deploy that amount of objects
            if(roundedY > highestY)
            {
                for(int i = 0; i < (roundedY - highestY); i++)
                {
                    // To not instantiate all objects in each other, every loop the object is instantiated one length higher
                    // TODO: replace the hardcoded 'i*0.3f' with the size of the object
                    SimulationObject simulationObject = Instantiate(prefab_object, 
                                                                    new Vector3(this.transform.position.x, this.transform.position.y + i * 0.3f, this.transform.position.z), 
                                                                    Quaternion.identity).GetComponent<SimulationObject>();
                    simulationObjectList.Add(simulationObject);
                }

                highestY = roundedY;
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
}
