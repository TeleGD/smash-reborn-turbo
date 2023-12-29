using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shieldbar : MonoBehaviour
{

    public Slider slidershield;


    public void SetMaxshield(float maxshield)
    {
        slidershield.maxValue = maxshield;
    }
    


    public void Setshield(float shield)
    {
        slidershield.value = shield;
    }

}
