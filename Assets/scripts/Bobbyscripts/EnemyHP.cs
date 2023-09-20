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
    private int tempHP;
    public bool rez = false;
    public Healthbar healthbar;



    private Animator enemyanim;

    public float startx;
    public float starty;

    //Healthbar
    

    private Rigidbody2D rb2D;


    public int enemyperc;



    void Start()
    {
        //setting enemy's max heatlth and energy
        enemyhp = enemymaxhp;
        healthbar.SetMaxhealth(enemymaxhp);
        enemyanim = GetComponent<Animator>();
        rb2D = transform.GetComponent<Rigidbody2D>();

    }

    void Update()
    {




        //update of the healthbar
        healthbar.SetHealth(enemyhp);

        
        tempHP = enemyhp;
        if (rb2D.position.y <= -3)
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