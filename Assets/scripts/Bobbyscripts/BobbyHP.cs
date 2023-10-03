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

    //coordonn�es de respawn
    private float startx;
    private float starty;

    //Healthbar
    

    private Rigidbody2D rb2D;

    //entier qui correspond aux pourcent du personnager
    public int enemyperc;

    public int iframes; //compte le nombre d'iframes qu'il reste au personnage apr�s respawn



    void Start()
    {
        //initialisation des PVs, de la barre de vie, de l'animator et du RigidBody2D.
        enemymaxhp = GameObject.Find("Global values").GetComponent<Globalvalues>().playermaxhp;
        enemyhp = enemymaxhp;
        healthbar.SetMaxhealth(enemymaxhp);
        enemyanim = GetComponent<Animator>();
        rb2D = transform.GetComponent<Rigidbody2D>();

        //coordonn�es de respawn
        startx = GameObject.Find("Global values").GetComponent<Globalvalues>().xstartp2;
        starty = GameObject.Find("Global values").GetComponent<Globalvalues>().ystartp2;

    }

    void Update()
    {


        if(iframes > 0) //si le perso a des iframes, on en enl�ve.
        {
            iframes-=1;
        }

        //update of the healthbar
        healthbar.SetHealth(enemyhp);

        //diminution de HP et d�faite si ejection
        if (rb2D.position.x >= 7 || rb2D.position.y <= -3 || rb2D.position.y >= 4 || rb2D.position.x <= -3)
        {
            if (enemyhp > 1)
            {
                enemyhp -= 1;
                transform.position = new Vector2(startx, starty);
                rb2D.velocity = new Vector2(0, 0);
                iframes = GameObject.Find("Global values").GetComponent<Globalvalues>().respawniframes;
                enemyperc = 0;

            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}