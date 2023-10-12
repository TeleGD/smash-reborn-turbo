using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class BobbyPer : MonoBehaviour
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
        if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char == "Bobby")
        {
            percent = GameObject.FindWithTag("Player1").GetComponent<charavalues>().percent;
        }
        if (GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char == "Bobby")
        {
            percent = GameObject.FindWithTag("Player2").GetComponent<charavalues>().percent;
        }
        TextUI.text = percent.ToString();
    }
}
