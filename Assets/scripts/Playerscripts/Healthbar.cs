using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    public Slider sliderHP;
    public Slider sliderNRG;


    public void SetMaxhealth(float maxhealth)
    {
        sliderHP.maxValue = maxhealth;
    }
    


    public void SetHealth(float health)
    {
        sliderHP.value = health;
    }

    public void SetMaxEnergy(float maxenergy)
    {
        sliderNRG.maxValue = maxenergy;
    }



    public void SetEnergy(float energy)
    {
        sliderNRG.value = energy;
    }

}
