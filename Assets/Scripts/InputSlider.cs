using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputSlider : MonoBehaviour
{
    [SerializeField]
    private GameObject sliderRail;
    private float sliderRailScale;

    [SerializeField]
    private GameObject displayTextObject;
    private TextMeshProUGUI displayText;

    private float currentPosition;
    public float currentValue;

    [SerializeField]
    private float scaleMax;

    // Start is called before the first frame update
    void Start()
    {
        sliderRailScale = sliderRail.transform.localScale.x;

        Debug.Log("Rail scale: " + sliderRailScale.ToString());

        displayText = displayTextObject.GetComponent<TextMeshProUGUI>();
        displayText.text = (scaleMax / 2).ToString("F2");
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = transform.localPosition.x;
        float temp;
        float percentage;

        if(currentPosition < 0)
        {
            temp = (-currentPosition / (sliderRailScale / 2));
            percentage = 1 - temp;
            currentValue = (scaleMax/2) * percentage;
        } 

        if(currentPosition > 0)
        {
            temp = (currentPosition / (sliderRailScale / 2));
            percentage = 1 - temp;
            currentValue = (scaleMax / 2) - (scaleMax / 2) * percentage + (scaleMax / 2);
        }

        if(currentValue < 0) { currentValue = 0; }
        if(currentValue > scaleMax) { currentValue = scaleMax;}
        displayText.text = currentValue.ToString("F2");

        // Handling for the slider on the edges
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
        // When the controller touches the slider, move it on its x axis according to the controller's position
        if(transform.localPosition.x > -sliderRailScale/2 && transform.localPosition.x < sliderRailScale/2)
        {
            transform.position = new Vector3(other.transform.position.x, transform.position.y, transform.position.z);      
        }
    }
}