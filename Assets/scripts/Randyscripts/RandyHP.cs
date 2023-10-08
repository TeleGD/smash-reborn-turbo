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
    private Rigidbody2D rb2D;

    private float startx;
    private float starty;


    public float radOcircle;
    public Healthbar healthbar;


    // Start is called before the first frame update
    void Start()
    {
        playermaxhp = GameObject.Find("Global values").GetComponent<Globalvalues>().playermaxhp;
        healthbar.SetMaxhealth(playermaxhp);
        rb2D = GetComponent<Rigidbody2D>();
        playerhp = playermaxhp;


        startx = GameObject.Find("Global values").GetComponent<Globalvalues>().xstartp1;
        starty = GameObject.Find("Global values").GetComponent<Globalvalues>().ystartp1;
    }

    // Update is called once per frame

    
    void Update()
    {

        if (GetComponent<charavalues>().iframes > 0) //si le perso a des iframes, on en enlève.
        {
            GetComponent<charavalues>().iframes -= 1;
        }

        healthbar.SetHealth(playerhp);

        if (playerhp <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
       
        if (rb2D.position.x >= GameObject.Find("Global values").GetComponent<Globalvalues>().deathright || rb2D.position.y <= GameObject.Find("Global values").GetComponent<Globalvalues>().deathdown || rb2D.position.y >= GameObject.Find("Global values").GetComponent<Globalvalues>().deathup || rb2D.position.x <= GameObject.Find("Global values").GetComponent<Globalvalues>().deathleft)
        {
            if(playerhp>1) 
            {
                playerhp= playerhp - 1;
                transform.position = new Vector2(startx,starty);
                rb2D.velocity = new Vector2(0,0);
                GetComponent<charavalues>().percent = 0;
                GetComponent<charavalues>().iframes = GameObject.Find("Global values").GetComponent<Globalvalues>().respawniframes;

            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            
        }
    }
}