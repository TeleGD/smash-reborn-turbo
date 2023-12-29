using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class soundeffectmanager : MonoBehaviour
{
    public AudioSource deathsound;
    public AudioSource hurtsound;
    public bool playdeath=false;
    public bool playhurt=false;
    public float deathvoladjjust;
    public float hurtvoladjjust;





    // Update is called once per frame
    void Update()
    {
        deathsound.volume = GameObject.Find("Optionvalues").GetComponent<optionvalues>().soundvol*deathvoladjjust;
        hurtsound.volume = GameObject.Find("Optionvalues").GetComponent<optionvalues>().soundvol*hurtvoladjjust;

        if (playdeath)
        {
            deathsound.Play();
            playdeath = false;
        }

        if (playhurt)
        {
            hurtsound.Play();
            playhurt = false;

        }
    }
}
