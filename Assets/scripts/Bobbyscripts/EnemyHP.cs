using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHP : MonoBehaviour
{


  

    //HP variables
    [Header("HP variables")]
    public int enemyhp; 
    public int enemymaxhp;
    public Healthbar healthbar;



    private Animator enemyanim;

    //coordonners de respawn
    public float startx;
    public float starty;

    //Healthbar
    

    private Rigidbody2D rb2D;

    //entier qui correspond aux pourcent du personnager
    public int enemyperc;



    void Start()
    {
        //initialisation des PVs, de la barre de vie, de l'animator et du RigidBody2D.
        enemyhp = enemymaxhp;
        healthbar.SetMaxhealth(enemymaxhp);
        enemyanim = GetComponent<Animator>();
        rb2D = transform.GetComponent<Rigidbody2D>();

    }

    void Update()
    {




        //update of the healthbar
        healthbar.SetHealth(enemyhp);

        //diminution de HP et défaite si ejection
        if (rb2D.position.x >= 7 || rb2D.position.y <= -3 || rb2D.position.y >= 4 || rb2D.position.x <= -3)
        {
            if (enemyhp > 1)
            {
                enemyhp -= 1;
                transform.position = new Vector2(startx, starty);
                rb2D.velocity = new Vector2(0, 0);
                enemyperc = 0;

            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
