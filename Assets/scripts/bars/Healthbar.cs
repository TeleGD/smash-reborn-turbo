using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    public Slider sliderHP;


    public void SetMaxhealth(float maxhealth)
    {
        sliderHP.maxValue = maxhealth;
    }
    


    public void SetHealth(float health)
    {
        sliderHP.value = health;
    }

}
