using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationObject : MonoBehaviour
{
    private float time_counter = 0; /* variable for measuring the time the object exists */
    private float time_freezeAfter = 5; /* number of seconds after the object gets frozen */

    private Rigidbody m_Rigidbody;
    private bool objThroughFloor = false;

    private float objectHeight;
    private bool movedThrough = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        objectHeight = this.GetComponent<Collider>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        // When the object has existed for a specified number of seconds, freeze it to reduce performance loss
        /*time_counter += Time.deltaTime;
        if (time_counter > time_freezeAfter && !objThroughFloor)
        {
            setKinematic();
        }*/

        if(objThroughFloor && this.transform.position.y < objectHeight/2 && !movedThrough)
        {
            this.transform.Translate(Vector3.up * Time.deltaTime);
        } else /* Make sure to stop moving the object when it has moved through the floor at least once, to stop jittering (because of rounding errors?) */
        {
            movedThrough = true;
        }
    }

    /// <summary>
    /// Method to clarify if the object needs to be moved through the floor by other classes, for example when spawning the objects underneath the floor.
    /// </summary>
    public void moveObjectThroughFloor()
    {
        objThroughFloor = true;
    }

    /// <summary>
    /// Set the object to kinematic.
    /// </summary>
    public void setKinematic()
    {
        m_Rigidbody.isKinematic = true;
    }

    public void disableKinematic()
    {
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
    }

    public void enableKinematic()
    {
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
    }
}
