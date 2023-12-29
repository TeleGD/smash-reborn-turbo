using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class optionvalues : MonoBehaviour
{
    public float musicvol;
    public float soundvol;
    public string p1char;
    public string p2char;
    public TextMeshProUGUI soundvoltxt;
    public TextMeshProUGUI musicvoltxt;
    public Slider soundslider;
    public Slider musicslider;


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {

        if(musicslider != null)
        {
            if(musicslider.value==0)
            {
                if(musicvol==0)
                {
                    musicvol = 1;
                }
                musicslider.value = musicvol;
            }
            
            musicvol = musicslider.value;
        }
        if(soundslider != null)
        {
            if (soundslider.value == 0)
            {
                if (soundvol == 0)
                {
                    soundvol = 1;
                }
                soundslider.value = soundvol;
            }

            soundvol = soundslider.value;
        }
        if(musicvoltxt != null)
        {
            musicvoltxt.text = Mathf.Round(musicvol * 100).ToString()+"%";
        }
        if (soundvoltxt != null)
        {
            soundvoltxt.text = Mathf.Round(soundvol * 100).ToString() + "%";
        }
        
        
        
    }
}
