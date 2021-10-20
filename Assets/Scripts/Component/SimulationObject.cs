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
        time_counter += Time.deltaTime;
        if (time_counter > time_freezeAfter && !objThroughFloor)
        {
            setKinematic();
        }

        if(objThroughFloor && this.transform.position.y < objectHeight/2)
        {
            this.transform.Translate(Vector3.up * Time.deltaTime);
        }
    }

    public void moveObjectThroughFloor()
    {
        objThroughFloor = true;
    }

    public void setKinematic()
    {
        m_Rigidbody.isKinematic = true;
    }
}
