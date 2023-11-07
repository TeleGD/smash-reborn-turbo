using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using UnityEngine.SocialPlatforms;

public class Charaper : MonoBehaviour
{

    public TextMeshProUGUI Textper;
    public TextMeshProUGUI TextHP;
    public bool hptxt;

    public int percent;
    public int hp;

    void Start()
    {
        
        if(hptxt)
        {
            TextHP = GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Textper = GetComponent<TextMeshProUGUI>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.CompareTag("P1per"))
        {
            percent = GameObject.FindWithTag("Player1").GetComponent<charavalues>().percent;
            hp= GameObject.Find("Global values").GetComponent<Globalvalues>().p1HP;
        }
        else
        {
            percent = GameObject.FindWithTag("Player2").GetComponent<charavalues>().percent;
            hp = GameObject.Find("Global values").GetComponent<Globalvalues>().p2HP;
        }
        if (hptxt)
        {
            TextHP.text = hp.ToString();
        }
        else
        {
            Textper.text = percent.ToString();
        }
        
        

    }
}
