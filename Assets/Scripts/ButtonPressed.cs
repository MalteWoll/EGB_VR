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
    private GameObject controllerObject; /* The GameObject holding the IntroController object */
    private IntroController introController; /* The IntroController object */

    [SerializeField]
    private GameObject buttonSoundParent; /* Gameobject that holds the ButtonSound object */
    private ButtonSound buttonSound; /* Object for controlling and playing sound */

    [SerializeField]
    private GameObject parentController; /* The controller of the virtual reality rig */

    private Animator buttonAnimator; /* The animator for the button, playing an animation where the button gets pressed in */

    // GameObjects holding the TextMeshPro objects
    [SerializeField]
    private GameObject ageTextField;
    [SerializeField]
    private GameObject genderTextField;

    // Text objects for displaying age and gender
    private TextMeshProUGUI ageText;
    private TextMeshProUGUI genderText;

    // Start is called before the first frame update
    void Start()
    {
        buttonSound = buttonSoundParent.GetComponent<ButtonSound>(); /* Get the component for playing and controlling sound */
        ageText = ageTextField.GetComponent<TextMeshProUGUI>(); /* Get the text mesh pro component for the field the age of the player appears in */
        genderText = genderTextField.GetComponent<TextMeshProUGUI>(); /* Get the same component for the gender input */
        introController = controllerObject.GetComponent<IntroController>(); /* Get the intro controller script from the GameObject */
    }


    private void OnTriggerEnter(Collider other)
    {
        // If a collision happens, check for the tag of the colliding object
        if (other.gameObject.tag == "ButtonNumber" || other.gameObject.tag == "Button" || other.gameObject.tag == "ButtonGender" || other.gameObject.tag == "ButtonClearGender")
        {
            buttonAnimator = other.transform.parent.GetComponent<Animator>(); /* Get the animator component of the button that was touched */
            buttonAnimator.SetTrigger("ButtonPressed"); /* If the colliding object is a button, play a short animation of it being pressed in */
            StartCoroutine(vibrateController()); /* For additional feedback, vibrate the controller that touched the button */
            buttonSound.playSound(); /* Play a short sound as audio feedback */
        }

        // If the button has a number tag, add it to the text field
        if (other.gameObject.tag == "ButtonNumber")
        {
            string number = other.name; /* Get the value of the number from the name of the button object */
            StartCoroutine(changeButtonColor(other.gameObject)); /* Change the colour of the button for a short time, to give additional visual feedback */

            if (ageText.text.Length < 3) /* If there are not yet 3 numbers in the text field, add the number to the text */
            {
                ageText.text = ageText.text + number;
            }
        }

        if (other.gameObject.tag == "ButtonGender") /* Do the same as for the number buttons, only with gender */
        {
            StartCoroutine(changeButtonColor(other.gameObject));
            string gender = other.name; /* Get the gender from the name of the object that was touched */
            genderText.text = gender; /* Change the text in the text field */
        }

        if (other.gameObject.tag == "ButtonClearGender") /* Button for deleting the gender input and clearing the text field */
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
                    ageText.text = ageText.text.Remove(ageText.text.Length - 1); /* Replace the text in the field with the same string, but the last element deleted */
                }
            }

            if(other.gameObject.name == "Confirm") /* This button lets the player proceed to the next input/stage, depending on the number of the stage the player is in currently */
            {
                introController.confirmButtonPressed(); /* Call the function for confirming in the introController object */
                Debug.Log("Confirm pressed");
            }

            if(other.gameObject.name == "Back") /* Do the same for the back button, but in the other direction */
            {
                introController.backButtonPressed();
                Debug.Log("Back pressed");
            }
        }
    }

    /// <summary>
    /// Vibrates the controller this script is placed in for a short time.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Changes button colour for a short time to visualize feedback for pressing a button.
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    private IEnumerator changeButtonColor(GameObject button)
    {
        Renderer renderer = button.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.green);
        yield return new WaitForSeconds(.5f);
        renderer.material.SetColor("_Color", Color.white);
    }
}
