using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// More or less a test script for the test tablet in the virtual reality, for testing what can be done. Could also be used for a simple tutorial.
/// </summary>
public class TabletScript : MonoBehaviour
{
    [SerializeField]
    private Material greenMat;
    [SerializeField]
    private Material redMat;
    [SerializeField]
    private GameObject tabletLamp;

    private bool colorRed = true;



    // Start is called before the first frame update
    void Start()
    {
        if(colorRed)
        {
            tabletLamp.GetComponent<Renderer>().material = redMat;
        } else
        {
            tabletLamp.GetComponent<Renderer>().material = greenMat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchColor()
    {
        if(colorRed)
        {
            colorRed = false;
            tabletLamp.GetComponent<Renderer>().material = greenMat;
        } else
        {
            colorRed = true;
            tabletLamp.GetComponent<Renderer>().material = redMat;
        }
    }
}
