using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BobbyHP : MonoBehaviour
{


  

    //HP variables
    [Header("HP variables")]
    public int enemyhp; 
    private int enemymaxhp;
    public Healthbar healthbar;



    private Animator enemyanim;

    //coordonnées de respawn
    private float startx;
    private float starty;

    //Healthbar
    

    private Rigidbody2D rb2D;

    //entier qui correspond aux pourcent du personnager
    public int enemyperc;

    //public int iframes; //compte le nombre d'iframes qu'il reste au personnage après respawn déplacé charavalues.


    void Start()
    {
        //initialisation des PVs, de la barre de vie, de l'animator et du RigidBody2D.
        enemymaxhp = GameObject.Find("Global values").GetComponent<Globalvalues>().playermaxhp;
        enemyhp = enemymaxhp;
        healthbar.SetMaxhealth(enemymaxhp);
        enemyanim = GetComponent<Animator>();
        rb2D = transform.GetComponent<Rigidbody2D>();

        //coordonnées de respawn
        if(this.CompareTag("Player1"))
        {
            startx = GameObject.Find("Global values").GetComponent<Globalvalues>().xstartp1;
            starty = GameObject.Find("Global values").GetComponent<Globalvalues>().ystartp1;
        }
        else
        {
            startx = GameObject.Find("Global values").GetComponent<Globalvalues>().xstartp2;
            starty = GameObject.Find("Global values").GetComponent<Globalvalues>().ystartp2;
        }



    }

    void Update()
    {


        if(GetComponent<charavalues>().iframes > 0) //si le perso a des iframes, on en enlève.
        {
            GetComponent<charavalues>().iframes -= 1;
        }

        //update of the healthbar
        healthbar.SetHealth(enemyhp);

        //diminution de HP et défaite si ejection
        if (rb2D.position.x >= GameObject.Find("Global values").GetComponent<Globalvalues>().deathright || rb2D.position.y <= GameObject.Find("Global values").GetComponent<Globalvalues>().deathdown || rb2D.position.y >= GameObject.Find("Global values").GetComponent<Globalvalues>().deathup || rb2D.position.x <= GameObject.Find("Global values").GetComponent<Globalvalues>().deathleft)
        {
            if (enemyhp > 1)
            {
                enemyhp -= 1;
                transform.position = new Vector2(startx, starty);
                rb2D.velocity = new Vector2(0, 0);
                GetComponent<charavalues>().iframes = GameObject.Find("Global values").GetComponent<Globalvalues>().respawniframes;
                GetComponent<charavalues>().percent = 0;

            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
