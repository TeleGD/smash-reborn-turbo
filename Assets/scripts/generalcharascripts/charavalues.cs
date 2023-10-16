using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class charavalues : MonoBehaviour
{

    public bool shielded;
    public int shield;
    public int percent;
    public int iframes;


    // Update is called once per frame
    void Update()
    {
        if (iframes > 0)
        {
            iframes -= 1;
        }
       
    }
}
