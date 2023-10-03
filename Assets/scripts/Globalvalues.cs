using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Globalvalues : MonoBehaviour
{
    //coordonn�es de respawn pour P1 et P2.
    [Header("Spawn/respawn")]
    public float xstartp1;
    public float ystartp1;
    public float xstartp2;
    public float ystartp2;

    //variables concernant le shield
    [Header("Shield variables")]
    public int shieldmax; //correspond au bouclier maximal
    public int shielddimrate; //correspond � la diminution passive du shield lorsqu'il est actif
    public int shieldrecharge; //correspond � la vitesse de rechargement du bouclier
    public int shieldbreakCD; //correspond au temps pendant lequel le bouclier est inactif si il est cass�

    [Header("player variables")]
    public int playermaxhp; //nombre de vies des joueurs
    public int respawniframes;
    public string player1char;
    public string player2char;


    // Update is called once per frame
    void Awake()
    {
        
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this);
    }




   
}
