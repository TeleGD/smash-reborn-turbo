using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class charadisplay : MonoBehaviour
{

    public TextMeshProUGUI Text;

    public string P1char;
    public string P2char;
    public bool P1;




    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        P1char = GameObject.Find("Optionvalues").GetComponent<optionvalues>().p1char;
        P2char = GameObject.Find("Optionvalues").GetComponent<optionvalues>().p2char;
        if (P1)
        {
            Text.text = P1char.ToString();
        }
        else
        {
            Text.text = P2char.ToString();
        }
        
        
    }
}
