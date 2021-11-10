using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

/// <summary>
/// The class that controls the tutorial/intro, places objects in the environment according to the height of the player, activates them, etc.
/// </summary>
public class IntroController : MonoBehaviour
{
    [SerializeField]
    private GameObject centerEye; /* The center eye object of the VR rig, to be used for height information */

    [SerializeField]
    private GameObject ageInputParent;
    [SerializeField]
    private GameObject confirmBackParent;
    [SerializeField]
    private GameObject genderInputParent;

    [SerializeField]
    private Vector3 playerEyeHeight; /* The height of the player */

    [SerializeField]
    private float heightOffset;

    private int introState = 0; /* The state the intro level is in, increases when the user presses the 'Confirm' button */

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the height was not correct and the object is uncomfortable or unusable for the player, reset it by pressing the "A" or "X" button on the controller
        if (OVRInput.Get(OVRInput.Button.One))
        {
            readHeightAndSetToObject(ageInputParent, heightOffset);
            readHeightAndSetToObject(confirmBackParent, heightOffset);
            readHeightAndSetToObject(genderInputParent, heightOffset);
        }
    }

    /// <summary>
    /// Reads out the height of the VR device on the players head and sets the gameobject (for example the age input object) to an according height
    /// </summary>
    /// <param name="gameObject">The GameObject which has its height changed</param>
    /// <param name="offset">An optional offset value to be added to the height</param>
    private void readHeightAndSetToObject(GameObject gameObject, float offset)
    {
        playerEyeHeight = centerEye.transform.position; /* Read out the initial height of the player from the center eye point of the VR device */
        gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                                                playerEyeHeight.y + offset,
                                                gameObject.transform.position.z); /* Set the age input parent height accordingly */
    }

    public void confirmButtonPressed()
    {
        if(introState == 0)
        {
            ageInputParent.SetActive(false);
            genderInputParent.SetActive(true);
            introState++;
        }
    }

    public void backButtonPressed()
    {
        if(introState == 0)
        {
            // Do nothing
        } else
        {
            if(introState == 1)
            {
                genderInputParent.SetActive(false);
                ageInputParent.SetActive(true);
                introState--;
            }
        }        
    }
}
