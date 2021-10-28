using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for 'building' the objects as a structure on each other, making the kinematic so that there are no physics.
/// </summary>
public class ObjectSpawnerBuilder : MonoBehaviour
{
    public GameObject prefab_object; /* The gameobject that holds the prefab for the objects appearing in the simulation */
    private float prefab_height; /* The height of the prefab object */
    private float prefab_width; /* The length of the longest side of the prefab object */
    public GameObject calculator; /* The gameobject with the script calculating the values */

    [SerializeField]
    private List<SimulationObject> simulationObjectList; /* The list of all objects that are created depending on the values of the exponential function */

    private int highestY = 0; /* To hold the highest rounded value the function reached so far */

    private int roundedY;

    private List<Vector3> spawnerGrid = new List<Vector3>(); /* List for varying the spawn positions within a squared grid */
    [SerializeField]
    private int spawnerGridCounter = 0;
    
    private float layer = 1;

    [SerializeField]
    private int gridLength; /* Length of the sides of the squared grid, should be an odd number, so the parent transform is always in the middle */

    // Start is called before the first frame update
    void Start()
    {
        prefab_height = 0.3f; /* hardcoded for now, TODO: calculate the height */
        prefab_width = 0.3f; /* TODO: same for the width */

        // Make sure the grid length is an odd number
        if (gridLength % 2 == 0) { gridLength++; }

        Vector3 gridStartPosition = new Vector3(this.transform.position.x - (((float)gridLength / 2 - 0.5f) * prefab_width),
                                                this.transform.position.y + (0.3f / 2),
                                                this.transform.position.z - (((float)gridLength / 2 - 0.5f)) * prefab_width);

        // Filling the list for the spawner grid with entries around the position of the parent GameObject with a previously specified size (by squaring the length, so that it is always squared)
        for (int i = 0; i < gridLength; i++)
        {
            for (int j = 0; j < gridLength; j++)
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

        // If the rounded y value is higher than the highest reached so far, calculate the difference between highest and second highest and deploy that amount of objects
        if (roundedY > highestY)
        {
            for (int i = 0; i < (roundedY - highestY); i++)
            {
                // Objects are instantiated layer by layer in the specified grid size around the spawner
                // TODO: replace the hardcoded 'i*0.3f' with the size of the object
                SimulationObject simulationObject = Instantiate(prefab_object,
                                                                new Vector3(spawnerGrid[spawnerGridCounter].x, spawnerGrid[spawnerGridCounter].y * layer, spawnerGrid[spawnerGridCounter].z),
                                                                Quaternion.identity).GetComponent<SimulationObject>();
                simulationObject.setKinematic();
                simulationObjectList.Add(simulationObject);

                // If the end of the grid position list is not yet reached, increase the counter, else reset it to zero
                if(spawnerGridCounter < spawnerGrid.Count - 1)
                {
                    spawnerGridCounter++;
                } else
                {
                    spawnerGridCounter = 0;
                    layer++;
                }
            }

            highestY = roundedY;
        }
        
    }
}
