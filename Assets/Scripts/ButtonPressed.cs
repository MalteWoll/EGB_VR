using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OVR;

/// <summary>
/// This script is placed in the fingertip of the VR rig. It detects collision with other objects and, if those are buttons, triggers the buttons animations and functionality.
/// </summary>
public class ButtonPressed : MonoBehaviour
{
    [SerializeField]
    private GameObject controllerObject;
    private IntroController introController;
    private MainController mainController;

    [SerializeField]
    private GameObject buttonSoundParent;
    private ButtonSound buttonSound;

    [SerializeField]
    private GameObject parentController;

    private Animator buttonAnimator; /* The animator for the button, playing an animation where the button gets pressed in */

    [SerializeField]
    private GameObject ageTextField;
    [SerializeField]
    private GameObject genderTextField;

    private TextMeshProUGUI ageText;
    private TextMeshProUGUI genderText;

    // Start is called before the first frame update
    void Start()
    {
        buttonSound = buttonSoundParent.GetComponent<ButtonSound>();

        if(controllerObject.name == "IntroController")
        {
            ageText = ageTextField.GetComponent<TextMeshProUGUI>(); /* Get the text mesh pro component for the field the age of the player appears in */
            genderText = genderTextField.GetComponent<TextMeshProUGUI>(); /* Get the same component for the gender input */
            introController = controllerObject.GetComponent<IntroController>(); /* Get the intro controller script from the GameObject */
        } else /* Replaced this class with another one for the main part of the application, so the else path should never be used */
        {
            mainController = controllerObject.GetComponent<MainController>();
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        // If a collision happens, check for the tag of the colliding object and add the appropriate number to the text field
        // For buttons, play a short animation of the button being pressed in
        if (other.gameObject.tag == "ButtonNumber" || other.gameObject.tag == "Button" || other.gameObject.tag == "ButtonGender" || other.gameObject.tag == "ButtonClearGender")
        {
            buttonAnimator = other.transform.parent.GetComponent<Animator>();
            buttonAnimator.SetTrigger("ButtonPressed");
            StartCoroutine(vibrateController());
            buttonSound.playSound();
        }

        // If the button is a number, add it to the text field, if there are not yet 3 numbers already
        if (other.gameObject.tag == "ButtonNumber")
        {
            string number = other.name;
            StartCoroutine(changeButtonColor(other.gameObject));

            if (ageText.text.Length < 3)
            {
                ageText.text = ageText.text + number;
            }
        }

        if (other.gameObject.tag == "ButtonGender")
        {
            StartCoroutine(changeButtonColor(other.gameObject));
            string gender = other.name;
            genderText.text = gender;
        }

        if (other.gameObject.tag == "ButtonClearGender")
        {
            genderText.text = "";
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
                Debug.Log("NumConfirm pressed");
            }

            if(other.gameObject.name == "Confirm") /* This button lets the player proceed to the next input/stage, depending on the number of the stage the player is in currently */
            {
                introController.confirmButtonPressed();
                Debug.Log("Confirm pressed");
            }

            if(other.gameObject.name == "Back")
            {
                introController.backButtonPressed();
                Debug.Log("Back pressed");
            }
        }
    }

    private IEnumerator vibrateController()
    {
        if(parentController.name == "CustomHandLeft")
        {
            OVRInput.SetControllerVibration(.5f, .2f, OVRInput.Controller.LTouch);
            yield return new WaitForSeconds(.2f);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        }

        if (parentController.name == "CustomHandRight")
        {
            OVRInput.SetControllerVibration(.5f, .2f, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(.2f);
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }

    private IEnumerator changeButtonColor(GameObject button)
    {
        Renderer renderer = button.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.green);
        yield return new WaitForSeconds(.5f);
        renderer.material.SetColor("_Color", Color.white);
    }
}
