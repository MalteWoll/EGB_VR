using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationObject : MonoBehaviour
{
    private float time_counter = 0; /* variable for measuring the time the object exists */
    private float time_freezeAfter = 5; /* number of seconds after the object gets frozen */

    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // When the object has existed for a specified number of seconds, freeze it to reduce performance loss
        time_counter += Time.deltaTime;
        if (time_counter > time_freezeAfter)
        {
            m_Rigidbody.isKinematic = true;
            enabled = false; /* Disable all functions, as the object is not moving anymore */
        }
    }
}
