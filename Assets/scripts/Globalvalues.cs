using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Globalvalues : MonoBehaviour
{
    //coordonnées de respawn pour P1 et P2.
    [Header("Spawn/respawn")]
    public float xstartp1;
    public float ystartp1;
    public float xstartp2;
    public float ystartp2;

    [Header("death zone")]
    public float deathleft;
    public float deathright;
    public float deathup;
    public float deathdown;

    //variables concernant le shield
    [Header("Shield variables")]
    public int shieldmax; //correspond au bouclier maximal
    public int shielddimrate; //correspond à la diminution passive du shield lorsqu'il est actif
    public int shieldrecharge; //correspond à la vitesse de rechargement du bouclier
    public int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cassé

    [Header("player variables")]
    public int playermaxhp; //nombre de vies des joueurs
    public int respawniframes;
    public string player1char;
    public string player2char;

    public bool playable;


    // Update is called once per frame
    void Awake()
    {
        
        Application.targetFrameRate = 60;

        if (playable)
        {
            player2char = GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char;
            player1char = GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char;

            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char == "Bobby")
            {
                UnityEngine.Debug.Log("bobby1");
                GameObject.Find("Randy1").SetActive(false);

            }
            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char == "Randy")
            {
                UnityEngine.Debug.Log("randy1");
                
                GameObject.Find("Bobby1").SetActive(false);

            }
            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char == "Bobby")
            {
                UnityEngine.Debug.Log("bobby2");
                GameObject.Find("Randy2").SetActive(false);

            }
            if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char == "Randy")
            {
                UnityEngine.Debug.Log("randy2");
                GameObject.Find("Bobby2").SetActive(false);
            }
        }
        



        DontDestroyOnLoad(this);
    }




   
}
