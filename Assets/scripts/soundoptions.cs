using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class soundoptions : MonoBehaviour
{

    public float soundvolume;
    public float musicvolume;
    public Slider soundslider;
    public Slider musicslider;

    private void Awake()
    {
        soundslider.value = soundvolume;
        musicslider.value = musicvolume;

    }


    // Update is called once per frame
    void Update()
    {
        soundvolume = soundslider.value;
        musicvolume = musicslider.value;
    }
}
