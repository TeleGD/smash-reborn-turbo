using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class Charaper : MonoBehaviour
{

    public TextMeshProUGUI TextUI;

    public int percent;

    void Start()
    {
        TextUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.CompareTag("P1per"))
        {
            percent = GameObject.FindWithTag("Player1").GetComponent<charavalues>().percent;
        }
        else
        {
            percent = GameObject.FindWithTag("Player2").GetComponent<charavalues>().percent;
        }
        TextUI.text = percent.ToString();

    }
}
