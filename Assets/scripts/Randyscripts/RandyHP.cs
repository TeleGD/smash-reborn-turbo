using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]

public class RandyHP : MonoBehaviour
{
    public float playerhp;
    private float playermaxhp;
    public int player1percent;
    [SerializeField] private Transform groundcheck;
    private Rigidbody2D rb;

    private float startx;
    private float starty;


    public float radOcircle;
    public Healthbar healthbar;

    public int iframes;

    // Start is called before the first frame update
    void Start()
    {
        playermaxhp = GameObject.Find("Global values").GetComponent<Globalvalues>().playermaxhp;
        healthbar.SetMaxhealth(playermaxhp);
        rb = GetComponent<Rigidbody2D>();
        playerhp = playermaxhp;


        startx = GameObject.Find("Global values").GetComponent<Globalvalues>().xstartp1;
        starty = GameObject.Find("Global values").GetComponent<Globalvalues>().ystartp1;
    }

    // Update is called once per frame

    
    void Update()
    {

        if (iframes > 0) //si le perso a des iframes, on en enlève.
        {
            iframes -= 1;
        }

        healthbar.SetHealth(playerhp);

        if (playerhp <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
       
        if (rb.position.x>=7 || rb.position.y <= -3 || GetComponent<Rigidbody2D>().position.y >= 4 || GetComponent<Rigidbody2D>().position.x <= -3)
        {
            if(playerhp>1) 
            {
                playerhp= playerhp - 1;
                transform.position = new Vector2(startx,starty);
                rb.velocity = new Vector2(0,0);
                player1percent = 0;
                iframes = GameObject.Find("Global values").GetComponent<Globalvalues>().respawniframes;

            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            
        }
    }
}