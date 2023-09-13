using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optionvalues : MonoBehaviour
{
    public static float musicvol;
    public static float soundvol;


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (GameObject.Find("Options") != null && GameObject.Find("Options").activeSelf==true)
        {
            musicvol = GameObject.Find("Options").GetComponent<soundoptions>().musicvolume;
            musicvol = GameObject.Find("Options").GetComponent<soundoptions>().soundvolume;
        }
    }
}
