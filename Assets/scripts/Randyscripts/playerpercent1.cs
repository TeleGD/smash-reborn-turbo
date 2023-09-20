using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerpercent1 : MonoBehaviour
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
        percent = GameObject.FindWithTag("Player1").GetComponent<PlayerHP>().player1percent;
        TextUI.text = percent.ToString();
    }
}
