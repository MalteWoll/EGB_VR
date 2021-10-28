using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectCounter : MonoBehaviour
{
    [SerializeField]
    private GameObject calculator; /* The object in which the calculations for the exponential function are made in */
    [SerializeField]
    private GameObject uiText; /* The UI text field in which the current number of objects is displayed */

    private TextMeshPro textField;

    // Start is called before the first frame update
    void Start()
    {
        textField = uiText.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        textField.text = calculator.GetComponent<FunctionCalculator>().roundedY.ToString();
    }
}
