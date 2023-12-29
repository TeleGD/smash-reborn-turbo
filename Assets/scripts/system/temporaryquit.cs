using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class temporaryquit : MonoBehaviour
{
    // Update is called once per frame
    PlayerControls controls;


    void OnMenu()
    {
        Debug.Log("Ciao");
        Application.Quit();

    }
}
