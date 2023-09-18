using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class enpercent : MonoBehaviour
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
        percent = GameObject.Find("Petitslime1").GetComponent<EnemyHP>().enemyperc;
        TextUI.text = percent.ToString();
    }
}
