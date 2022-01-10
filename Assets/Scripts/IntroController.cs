using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OVR;
using TMPro;

/// <summary>
/// The class that controls the tutorial/intro, places objects in the environment according to the height of the player, activates them, etc.
/// </summary>
public class IntroController : MonoBehaviour
{
    [SerializeField]
    private GameObject centerEye; /* The center eye object of the VR rig, to be used for height information */

    // The following gameobjects are used to let the user enter their age, gender, and provide some instructions. The parents of the canvases and text objects are provided, to set them (and all their children)
    // to active/inactive.
    [SerializeField]
    private GameObject ageInputParent; /* Parent of the numpad letting the user enter their age */
    [SerializeField]
    private GameObject confirmBackParent; /* Parent of the buttons to continue or go back */
    [SerializeField]
    private GameObject genderInputParent; /* Parent of the gender selection for the user */
    [SerializeField]
    private GameObject instructions01Parent; /* Parent of the first set of instructions */

    [SerializeField]
    private Vector3 playerEyeHeight; /* The height of the player */

    [SerializeField]
    private float heightOffset;

    private int introState = 0; /* The state the introduction is in, increases when the user presses the 'Confirm' button and decreases if they press the 'Back' button */

    [SerializeField]
    private GameObject ageTextObject;
    private TextMeshProUGUI ageText;

    [SerializeField]
    private GameObject genderTextObject;
    private TextMeshProUGUI genderText;

    [SerializeField]
    private GameObject sliderParent;
    [SerializeField]
    private GameObject interactables;

    void Start()
    {
        // Clear PlayerPrefs, just in case
        PlayerPrefs.SetInt("age", 0);
        PlayerPrefs.SetString("gender", "");
        PlayerPrefs.Save();

    }

    // Update is called once per frame
    void Update()
    {
        // If the height was not correct and the object heights are uncomfortable or unusable for the player, reset them by pressing the "A" or "X" button on the controllers
        if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three))
        {
            readHeightAndSetToObject(ageInputParent, heightOffset);
            readHeightAndSetToObject(confirmBackParent, heightOffset);
            readHeightAndSetToObject(genderInputParent, heightOffset);
            readHeightAndSetToObject(instructions01Parent, heightOffset);
            readHeightAndSetToObject(sliderParent, heightOffset);
        }
    }

    /// <summary>
    /// Reads out the height of the VR device on the players head and sets the gameobjects (for example the age input object) to an according height. This allows for users with different heights.
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

    /// <summary>
    /// Listener function for the confirm button. In the intro, we ask for age and gender, since the user can go back and forth, the introState variable keeps track of where they are.
    /// For example: introState == 0 means start of the application and age prompt, introState == 1 is after the user pressed the 'Confirm' button once, displaying the gender prompt now.
    /// </summary>
    public void confirmButtonPressed()
    {
        switch(introState)
        {
            case 0:
                // Read out the age value on the text component, then save to PlayerPrefs
                ageText = ageTextObject.GetComponent<TextMeshProUGUI>();
                int age = 0;
                if (ageText.text != "")
                {
                    age = int.Parse(ageText.text);
                }
                PlayerPrefs.SetInt("age", age); /* Saving is done in the main experiment, to transfer the data to that scene, it is saved in the PlayerPrefs */
                PlayerPrefs.Save();
                ageInputParent.SetActive(false);
                genderInputParent.SetActive(true);
                introState++;
                break;
            case 1:
                // Do the same for the gender
                genderText = genderTextObject.GetComponent<TextMeshProUGUI>();
                string gender = "";
                if(genderText.text != "")
                {
                    gender = genderText.text;
                }
                PlayerPrefs.SetString("gender", gender);
                PlayerPrefs.Save();
                genderInputParent.SetActive(false);
                instructions01Parent.SetActive(true);

                sliderParent.SetActive(true);
                interactables.SetActive(true);

                introState++;
                break;
            case 2:
                SceneManager.LoadScene(1); /* Load the main scene */
                break;
        }
    }

    /// <summary>
    /// Listener function for the back button, same functionality as the previous listener, but in the other 'direction'.
    /// </summary>
    public void backButtonPressed()
    {
        switch(introState)
        {
            case 0:
                break;
            case 1:
                genderInputParent.SetActive(false);
                ageInputParent.SetActive(true);
                introState--;
                break;
            case 2:
                instructions01Parent.SetActive(false);
                genderInputParent.SetActive(true);
                introState--;
                break;
        }
    }
}
