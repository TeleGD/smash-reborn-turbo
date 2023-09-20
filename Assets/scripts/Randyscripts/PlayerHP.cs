using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerHP : MonoBehaviour
{
    public float Eldonhp;
    public float Eldonmaxhp;
    public int player1percent;
    [SerializeField] private Transform groundcheck;
    private Rigidbody2D rb;


    public float radOcircle;
    public Healthbar healthbar;

    // Start is called before the first frame update
    void Start()
    {
        Eldonhp = Playervalues.EldonHP;
        Eldonmaxhp = Playervalues.EldonmaxHP;
        healthbar.SetMaxhealth(Eldonmaxhp);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    
    void Update()
    {
       

        healthbar.SetHealth(Eldonhp);

        if (Eldonhp <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
       
        if (rb.position.x>=7 || rb.position.y <= -3 || GetComponent<Rigidbody2D>().position.y >= 4 || GetComponent<Rigidbody2D>().position.x <= -3)
        {
            if(Eldonhp>1) 
            {
                Eldonhp= Eldonhp - 1;
                transform.position = new Vector2(1,1);
                rb.velocity = new Vector2(0,0);
                player1percent = 0;

            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            
        }
    }
}