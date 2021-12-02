using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OVR;

public class IndexFingerMain : MonoBehaviour
{
    [SerializeField]
    private GameObject controllerObject;
    private MainController mainController;

    [SerializeField]
    private GameObject parentController;

    private Animator buttonAnimator; /* The animator for the button, playing an animation where the button gets pressed in */

    // Start is called before the first frame update
    void Start()
    {
        mainController = controllerObject.GetComponent<MainController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        // If a collision happens, check for the tag of the colliding object and add the appropriate number to the text field
        // For buttons, play a short animation of the button being pressed in
        if (other.gameObject.tag == "Button")
        {
            buttonAnimator = other.transform.parent.GetComponent<Animator>();
            buttonAnimator.SetTrigger("ButtonPressed");
            StartCoroutine(vibrateController());
            mainController.buttonPressed(other.gameObject.name);
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
