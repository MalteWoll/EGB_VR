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
        ageText = ageTextField.GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ButtonNumber" || other.gameObject.tag == "Button")
        {
            Debug.Log("Collision with " + other.transform.parent.name);

            buttonAnimator = other.transform.parent.GetComponent<Animator>();
            buttonAnimator.SetTrigger("ButtonPressed");
        }

        if(other.gameObject.tag == "ButtonNumber")
        {
            string number = other.name;

            ageText.text = ageText.text + number;
        }

        if(other.gameObject.tag == "Button")
        {
            if(other.gameObject.name == "NumBack")
            {
                if(ageText.text.Length > 0)
                {
                    ageText.text = ageText.text.Remove(ageText.text.Length - 1);
                }
            }

            if(other.gameObject.name == "NumConfirm")
            {

            }
        }
    }
}
