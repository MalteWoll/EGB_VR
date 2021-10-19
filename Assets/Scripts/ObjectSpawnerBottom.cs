using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for creating and pushing the simulation objects through the floor, instead of having them fall down to create the impression of a growing pile, instead of the more chaotic dropping.
/// </summary>
public class ObjectSpawnerBottom : MonoBehaviour
{
    public GameObject prefab_object; /* The gameobject that holds the prefab for the objects appearing in the simulation */
    private float prefab_height; /* The height of the prefab object */
    private float prefab_width; /* The length of the longest side of the prefab object */
    public GameObject calculator; /* The gameobject with the script calculating the values */

    [SerializeField]
    private List<SimulationObject> simulationObjectList; /* The list of all objects that are created depending on the values of the exponential function */

    private int highestY = 0; /* To hold the highest rounded value the function reached so far */

    private bool start; /* Set this true when the start button is pressed */
    private int roundedY;

    private List<Vector3> spawnerGrid = new List<Vector3>(); /* List for varying the spawn positions within a squared grid */
    private int spawnerGridCounter = 0;

    [SerializeField]
    private int gridLength; /* Length of the sides of the squared grid, should be an odd number, so the parent transform is always in the middle */

    // Start is called before the first frame update
    void Start()
    {
        prefab_height = 0.3f; /* hardcoded for now, TODO: calculate the height */
        prefab_width = 0.3f; /* TODO: same for the width */

        // Make sure the grid length is an odd number
        if(gridLength % 2 == 0) { gridLength++; }

        Vector3 gridStartPosition = new Vector3(this.transform.position.x - (((float)gridLength / 2 - 0.5f) * prefab_width),
                                                this.transform.position.y - prefab_height,
                                                this.transform.position.z - (((float)gridLength / 2 - 0.5f)) * prefab_width);

        // Filling the list for the spawner grid with entries around the position of the parent GameObject with a previously specified size (by squaring the length, so that it is always squared)
        for (int i = 0; i < gridLength; i++)
        {
            for(int j = 0; j < gridLength; j++)
            {
                Vector3 currentPosition = gridStartPosition + new Vector3(prefab_width * j, 0, prefab_width * i);
                spawnerGrid.Add(currentPosition);

            }
        }

        Debug.Log("Grid List Count: " + spawnerGrid.Count);

    }

    // Update is called once per frame
    void Update()
    {
        // Get the values from the Calculator class
        roundedY = calculator.GetComponent<FunctionCalculator>().roundedY;
        start = calculator.GetComponent<FunctionCalculator>().start;

        if (start)
        {
            // If the rounded y value is higher than the highest reached so far, calculate the difference between highest and second highest and deploy that amount of objects
            if (roundedY > highestY)
            {
                for (int i = 0; i < (roundedY - highestY); i++)
                {
                    // To not instantiate all objects in each other, every loop the object is instantiated one step further on the grid, looping when the end of the grid position list is reached
                    // TODO: replace the hardcoded 'i*0.3f' with the size of the object
                    SimulationObject simulationObject = Instantiate(prefab_object,
                                                                    /*spawnerGrid[spawnerGridCounter],*/
                                                                    spawnerGrid[Random.Range(0,24)],
                                                                    Quaternion.identity).GetComponent<SimulationObject>();
                    simulationObject.moveObjectThroughFloor();
                    simulationObjectList.Add(simulationObject);

                    /*
                    // If the end of the grid position list is not yet reached, increase the counter, else reset it to zero
                    if(spawnerGridCounter < spawnerGrid.Count - 1)
                    {
                        spawnerGridCounter++;
                    } else
                    {
                        spawnerGridCounter = 0;
                    }*/
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
