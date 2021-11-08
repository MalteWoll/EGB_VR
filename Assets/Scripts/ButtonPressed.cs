using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonPressed : MonoBehaviour
{
    private Animator buttonAnimator; /* The animator for the button, playing an animation where the button gets pressed in */

    [SerializeField]
    GameObject ageTextField;

    TextMeshProUGUI ageText;


    // Start is called before the first frame update
    void Start()
    {
        ageText = ageTextField.GetComponent<TextMeshProUGUI>(); /* Get the text mesh pro component for the field the age of the player appears in */
    }


    private void OnTriggerEnter(Collider other)
    {
        // If a collision happens, check for the tag of the colliding object and add the appropriate number to the text field
        // For buttons, play a short animation of the button being pressed in
        if (other.gameObject.tag == "ButtonNumber" || other.gameObject.tag == "Button")
        {
            buttonAnimator = other.transform.parent.GetComponent<Animator>();
            buttonAnimator.SetTrigger("ButtonPressed");
        }

        // If the button is a number, add it to the text field, if there are not yet 3 numbers already
        if (other.gameObject.tag == "ButtonNumber")
        {
            string number = other.name;

            if (ageText.text.Length < 3)
            {
                ageText.text = ageText.text + number;
            }
        }

        // If the button is not a number, check for the different functionality and execute the appropriate commands
        if (other.gameObject.tag == "Button")
        {
            if (other.gameObject.name == "NumBack") /* This button deletes the last number */
            {
                if (ageText.text.Length > 0)
                {
                    ageText.text = ageText.text.Remove(ageText.text.Length - 1);
                }
            }

            if (other.gameObject.name == "NumConfirm")
            {

            }
        }
    }
}
