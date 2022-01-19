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
    private int introState = 0; /* The state the introduction is in, increases when the user presses the 'Confirm' button and decreases if they press the 'Back' button */

    // The following gameobjects are used to let the user enter their age, gender, and provide some instructions. The parents of the canvases and text objects are provided, to set them (and all their children)
    // to active/inactive.
    [SerializeField]
    private GameObject ageInputParent; /* Parent of the numpad letting the user enter their age */
    [SerializeField]
    private GameObject confirmBackParent; /* Parent of the buttons to continue or go back */
    [SerializeField]
    private GameObject genderInputParent; /* Parent of the gender selection for the user */
    [SerializeField]
    private GameObject instructionsParent00;
    [SerializeField]
    private GameObject instructions01Parent; /* Parent of the first set of instructions */

    [SerializeField]
    private GameObject centerEye; /* The center eye object of the VR rig, to be used for height information */
    [SerializeField]
    private Vector3 playerEyeHeight; /* The height of the player */
    [SerializeField]
    private float heightOffset;

    // Text GameObjects and their components
    [SerializeField]
    private GameObject ageTextObject;
    private TextMeshProUGUI ageText;
    [SerializeField]
    private GameObject genderTextObject;
    private TextMeshProUGUI genderText;

    // The parents for the slider and some interactable objects
    [SerializeField]
    private GameObject sliderParent;
    [SerializeField]
    private GameObject interactables;

    // Variables for setting the height of the objects after a short delay
    private bool firstUpdate = true;
    private float setHeightTimer = 0;

    void Start()
    {
        // Clear PlayerPrefs, just in case
        PlayerPrefs.SetInt("age", 0);
        PlayerPrefs.SetString("gender", "");
        PlayerPrefs.Save();

    }

    void Update()
    {
        // Wait for 2 seconds to set the height of the interactable GameObjects according to the height of the VR device
        if (firstUpdate) 
        {
            setHeightTimer += Time.deltaTime;
            if (setHeightTimer > 2)
            {
                readHeightAndSetToObject(ageInputParent, heightOffset);
                readHeightAndSetToObject(confirmBackParent, heightOffset);
                readHeightAndSetToObject(genderInputParent, heightOffset);
                readHeightAndSetToObject(instructions01Parent, heightOffset);
                readHeightAndSetToObject(sliderParent, heightOffset);
                readHeightAndSetToObject(instructionsParent00, heightOffset);
                firstUpdate = false; /* After setting the height, don't call this again */
            }
        }

        // If the height was not correct and the object heights are uncomfortable or unusable for the player, reset them by pressing the "A" or "X" button on the controllers
        if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three))
        {
            readHeightAndSetToObject(ageInputParent, heightOffset);
            readHeightAndSetToObject(confirmBackParent, heightOffset);
            readHeightAndSetToObject(genderInputParent, heightOffset);
            readHeightAndSetToObject(instructions01Parent, heightOffset);
            readHeightAndSetToObject(sliderParent, heightOffset);
            readHeightAndSetToObject(instructionsParent00, heightOffset);
        }

        // For debugging on the PC
        if(Input.GetKeyDown(KeyCode.C))
        {
            confirmButtonPressed();
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            backButtonPressed();
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
    /// For example: introState == 0 means start of the application and first instructional text, introState == 1 is after the user pressed the 'Confirm' button once, displaying the age prompt now.
    /// </summary>
    public void confirmButtonPressed()
    {
        switch(introState)
        {
            case 0:
                instructionsParent00.SetActive(false);
                ageInputParent.SetActive(true);
                introState++;
                break;
            case 1:
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
            case 2:
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

                // Enable the slider and interactables
                sliderParent.SetActive(true); 
                interactables.SetActive(true);

                introState++;
                break;
            case 3:
                SceneManager.LoadScene(1); /* Load the main scene */
                break;
        }
        Debug.Log("IntroState: " + introState.ToString());
    }

    /// <summary>
    /// Listener function for the back button, same functionality as the previous listener, but in the other 'direction'.
    /// </summary>
    public void backButtonPressed()
    {
        if(introState > 0) { introState--; }
        switch(introState)
        {
            case 0:
                ageInputParent.SetActive(false);
                instructionsParent00.SetActive(true);
                break;
            case 1:
                genderInputParent.SetActive(false);
                ageInputParent.SetActive(true);
                break;
            case 2:
                instructions01Parent.SetActive(false);
                genderInputParent.SetActive(true);
                break;
        }
        Debug.Log("IntroState: " + introState.ToString());
    }
}
