using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerDoubling : MonoBehaviour
{
    public GameObject prefab_object; /* The gameobject that holds the prefab for the objects appearing in the simulation */
    private float prefab_height; /* The height of the prefab object */
    private float prefab_width; /* The length of the longest side of the prefab object */

    private List<SimulationObject> simulationObjectList = new List<SimulationObject>(); /* The list of all objects that are created depending on the values of the exponential function */

    [SerializeField]
    private float x_time = 0; /* Counter for the time */
    private float timeCounter = 1;

    [SerializeField]
    private int objectAmount = 1;

    [SerializeField]
    private float speed;

    [SerializeField]
    private int gridLength; /* Length of the sides of the squared grid, should be an odd number, so the parent transform is always in the middle */
    [SerializeField]
    private int gridHeight;

    private List<Vector3> spawnGrid = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        prefab_height = 0.3f; /* hardcoded for now, TODO: calculate the height */
        prefab_width = 0.3f; /* TODO: same for the width */

        // Make sure the grid length is an odd number
        if (gridLength % 2 == 0) { gridLength++; }

        Vector3 gridStartPosition = new Vector3(this.transform.position.x - (((float)gridLength / 2 - 0.5f) * prefab_width),
                                                this.transform.position.y - prefab_height,
                                                this.transform.position.z - (((float)gridLength / 2 - 0.5f)) * prefab_width);

        // Filling the list for the spawner grid with entries around the position of the parent GameObject with a previously specified size (by squaring the length, so that it is always squared)
        for (int k = 1; k < gridHeight; k++)
        {
            for (int i = 0; i < gridLength; i++)
            {
                for (int j = 0; j < gridLength; j++)
                {
                    Vector3 currentPosition = gridStartPosition + new Vector3(prefab_width * j, this.transform.position.y*k*prefab_height, prefab_width * i);
                    spawnGrid.Add(currentPosition);
                }
            }
        }

        spawnObjects(objectAmount);
    }

    // Update is called once per frame
    void Update()
    {
        x_time += Time.deltaTime * speed;

        if(x_time > timeCounter)
        {
            objectAmount = objectAmount * 2;

            spawnObjects(objectAmount);

            timeCounter++;
        }
    }

    void spawnObjects(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            SimulationObject simulationObject = Instantiate(prefab_object,
                                                            spawnGrid[i],
                                                            Quaternion.identity).GetComponent<SimulationObject>();
            simulationObjectList.Add(simulationObject);
        }
    }
}
