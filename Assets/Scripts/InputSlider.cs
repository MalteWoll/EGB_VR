using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputSlider : MonoBehaviour
{
    [SerializeField]
    private GameObject sliderRail; /* The GameObject the moveable slider is placed on physically */
    private float sliderRailScale; /* The size of the rail */

    // Text object and GameObject it is placed in for displaying the current value of the slider
    [SerializeField]
    private GameObject displayTextObject;
    private TextMeshProUGUI displayText;

    // Current position and corresponding value of the slider
    private float currentPosition;
    public float currentValue;

    // Maximum and minimum value for the slider
    [SerializeField]
    private float scaleMax;
    [SerializeField]
    private float minSliderValue;

    public bool touched = false; /* Helper variable for checking if the slider is being touched */

    // Start is called before the first frame update
    void Start()
    {
        sliderRailScale = sliderRail.transform.localScale.x; /* Get the x-value of the scale of the slider rail */
        Debug.Log("Rail scale: " + sliderRailScale.ToString());

        displayText = displayTextObject.GetComponent<TextMeshProUGUI>(); /* Get the text component */
        displayText.text = (scaleMax / 2).ToString("F0"); /* Because the slider is located in the middle of the rail at the start, the start value is half the maximum value of the slider */

        this.GetComponent<Renderer>().material.SetColor("_Color", Color.red); /* The color of the idle slider is red */
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = transform.localPosition.x; /* Get the current position of the slider in object space */

        // Helper variables for calculation
        float temp;
        float percentage;

        // Because values 'left' of the middle are negative in object space, we need to dinstinguish between positive and negative values
        if(currentPosition < 0)
        {
            // Calculate the value according to the position of the slider and the maximum value that is used
            temp = (-currentPosition / (sliderRailScale / 2)); /* Use -currentPosition as it is a negative value */
            percentage = 1 - temp;
            currentValue = (scaleMax/2) * percentage;
        } 
        if(currentPosition > 0)
        {
            // Calculate the value according to the position of the slider and the maximum value that is used
            temp = (currentPosition / (sliderRailScale / 2));
            percentage = 1 - temp;
            currentValue = (scaleMax / 2) - (scaleMax / 2) * percentage + (scaleMax / 2) + minSliderValue;
        }

        // Prevent the scale from displaying values above the maximum or below 0
        if(currentValue < 0) { currentValue = 0; }
        if(currentValue > scaleMax) { currentValue = scaleMax; }

        if (touched)
        {
            displayText.text = (currentValue+minSliderValue).ToString("F0");
        } else
        {
            displayText.text = ((scaleMax+minSliderValue) / 2).ToString("F0"); /* If the slider has not been touched yet, display half the maximum value */
        }

        // Handling for the slider on the edges: Prevent going over either of the ends
        if (transform.localPosition.x <= -sliderRailScale / 2)
        {
            transform.localPosition = new Vector3(0.01f, 0, 0) + transform.localPosition;
        }
        else
        {
            if (transform.localPosition.x >= sliderRailScale / 2)
            {
                transform.localPosition = transform.localPosition - new Vector3(0.01f, 0, 0);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        touched = true;

        // When the controller touches the slider, move it on its x axis according to the controller's position
        if(transform.localPosition.x > -sliderRailScale/2 && transform.localPosition.x < sliderRailScale/2)
        {
            transform.position = new Vector3(other.transform.position.x, transform.position.y, transform.position.z);

            // Make the slider change color while being touched
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        this.GetComponent<Renderer>().material.SetColor("_Color", Color.red); /* Change color back to red when not touching the slider */
    }

    /// <summary>
    /// Sets the minimum and maximum value of the slider.
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public void setSliderValues(float minValue, float maxValue)
    {
        minSliderValue = minValue;
        scaleMax = maxValue;
    }

    /// <summary>
    ///  Resets the position of the slider and the boolean that indicated if the slider has been touched yet.
    /// </summary>
    public void resetSlider()
    {
        touched = false;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }
}
