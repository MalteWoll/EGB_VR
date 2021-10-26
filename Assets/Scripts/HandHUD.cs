using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHUD : MonoBehaviour
{
    [SerializeField]
    GameObject handController;

    [SerializeField]
    GameObject m_hudText;

    // Start is called before the first frame update
    void Start()
    {
        m_hudText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = handController.transform.rotation.eulerAngles;
        Debug.Log(rot.z);
        if(rot.z < 250 && rot.z > 190)
        {
            m_hudText.SetActive(true);
        } else
        {
            m_hudText.SetActive(false);
        }
    }
}
