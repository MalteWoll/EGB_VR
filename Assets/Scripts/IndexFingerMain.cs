using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OVR;

/// <summary>
/// Class located in a GameObject in the fingertips of the index fingers in the virtual environment. Used for detecting collisions with other objects that can be interacted with, like buttons.
/// </summary>
public class IndexFingerMain : MonoBehaviour
{
    [SerializeField]
    private GameObject controllerObject; /* The GameObject the MainController class is attached to */
    private MainController mainController; /* The class controlling the experiment */

    // For a specific interaction with the interactive visualization, the VisualizationInteractive object is required
    [SerializeField]
    private GameObject interactiveParent;
    private VisualizationInteractive visualizationInteractive;

    [SerializeField]
    private GameObject parentController; /* The parent controller of the finger this class is located on, to identify which hand was used (necessary for haptic feedback) */

    private Animator buttonAnimator; /* The animator for the button, playing an animation where the button gets pressed in */

    // Start is called before the first frame update
    void Start()
    {
        // Get the required components from the parent objects
        mainController = controllerObject.GetComponent<MainController>();
        visualizationInteractive = interactiveParent.GetComponent<VisualizationInteractive>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If a collision happens, check for the tag of the colliding object. For buttons, play a short animation of the button being pressed in.
        if (other.gameObject.tag == "Button" || other.gameObject.tag == "ButtonNumber" || other.gameObject.tag == "ButtonNumberDelete" || other.gameObject.tag == "NumPadConfirm"
            || other.gameObject.tag == "ButtonPickInvestment")
        {
            Debug.Log("Button pressed: " + other.gameObject.tag);
            buttonAnimator = other.transform.parent.GetComponent<Animator>(); /* Get the animator component of the button that was touched */
            buttonAnimator.SetTrigger("ButtonPressed"); /* Play the animation on the button by activating the trigger for the animation */
            StartCoroutine(vibrateController()); /* Activate the haptic feedback on the controller that touched the button */
            mainController.buttonPressed(other.gameObject); /* Go to the main controller to activate the functionality of the button that was pressed */
        }

        // If the user touches a gold bar, call the function to disable kinematic and enable gravity for all gold bars
        if(other.gameObject.tag == "GoldBar")
        {
            visualizationInteractive.disableKinematicForAll();
        }
    }

    /// <summary>
    /// Short haptic feedback on the controller which hit the collider.
    /// </summary>
    /// <returns></returns>
    private IEnumerator vibrateController()
    {
        if (parentController.name == "CustomHandLeft")
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
}
