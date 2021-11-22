using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class for letting the user draw a line on the whiteboard. Seperate class because it makes collision detection easier.
/// </summary>
public class WhiteBoardUserDrawing : MonoBehaviour
{
    [SerializeField]
    private float sensibility;

    // Components on the GameObject
    private Collider collider_whiteBoardUser;
    private LineRenderer lineRenderer_whiteBoardUser;

    private Vector3 lastDrawnPosition; /* To check what the last position drawn on the LineRenderer is */
    private GameObject indexFinger;
    private int lineRendererPositions = 0;

    void Start()
    {
        // Getting the components from the GameObject and assigning the to the objects
        collider_whiteBoardUser = this.GetComponent<Collider>();
        lineRenderer_whiteBoardUser = this.GetComponent<LineRenderer>();

        // Set some parameters for the LineRenderer, like line width
        lineRenderer_whiteBoardUser.widthMultiplier = 0.01f;
        lineRenderer_whiteBoardUser.positionCount = lineRendererPositions;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "IndexFinger")
        {
            indexFinger = other.gameObject;

            if(Vector3.Distance(lastDrawnPosition, indexFinger.transform.position) > sensibility)
            {
                addPointToLine(indexFinger.transform.position);
                lastDrawnPosition = indexFinger.transform.position;
            } 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "IndexFinger")
        {
            indexFinger = other.gameObject;

            if (Vector3.Distance(lastDrawnPosition, indexFinger.transform.position) > sensibility)
            {
                addPointToLine(indexFinger.transform.position);
                lastDrawnPosition = indexFinger.transform.position;
            }
        }
    }

    /// <summary>
    /// Gets a position as parameter, increases the position count of the line renderer and adds the new position to it.
    /// </summary>
    /// <param name="position">Position information as Vector3.</param>
    private void addPointToLine(Vector3 position)
    {
        lineRendererPositions++;
        lineRenderer_whiteBoardUser.positionCount = lineRendererPositions;

        Vector3 positionCorrection = new Vector3(this.transform.position.x + 0.01f, position.y, position.z);

        lineRenderer_whiteBoardUser.SetPosition(lineRendererPositions-1, positionCorrection);
    }
}
