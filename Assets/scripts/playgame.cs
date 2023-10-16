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
        SceneManager.LoadScene("Chaselect1");
    }

    public void Mainmenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Quitgame()
    {
        Debug.Log("Ciao");
        Application.Quit();
    }

    public void Bobby()
    {
        if (SceneManager.GetActiveScene().name=="Chaselect1")
        {
            GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char="Bobby";
            SceneManager.LoadScene("Chaselect2");
        }
        if (SceneManager.GetActiveScene().name == "Chaselect2")
        {
            GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char = "Bobby";
            SceneManager.LoadScene("charaselect-to-game");
        }
    }

    public void Randy()
    {
        if (SceneManager.GetActiveScene().name == "Chaselect1")
        {
            GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char = "Randy";
            SceneManager.LoadScene("Chaselect2");
        }
        if (SceneManager.GetActiveScene().name == "Chaselect2")
        {
            GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char = "Randy";
            SceneManager.LoadScene("charaselect-to-game");
        }
    }

    public void continuegame()
    {

    }


   

}

