using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class playgame : MonoBehaviour
{
    PlayerControls controls;
    public void Playgame()
    {
        SceneManager.LoadScene("BreedingGrounds");
    }
    public void Quitgame()
    {
        Debug.Log("Ciao");
        Application.Quit();
    }

    public void continuegame()
    {

    }


   

}

