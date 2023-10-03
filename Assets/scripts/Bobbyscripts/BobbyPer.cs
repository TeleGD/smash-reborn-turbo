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
        percent = GameObject.FindWithTag("Player2").GetComponent<BobbyHP>().enemyperc;
        TextUI.text = percent.ToString();
    }
}
