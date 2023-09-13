using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerHP : MonoBehaviour
{
    public float Eldonhp;
    public float Eldonmaxhp;
    public float EldonNRG;
    public float EldonmaxNRG;
    [SerializeField] private LayerMask whatisspike;
    [SerializeField] private Transform groundcheck;
    private bool inv = false;
    private int iframe;
    public int invicibilityframes;
    public float hitjumpforce;
    private Rigidbody2D rb;


    public float radOcircle;
    public Healthbar healthbar;

    // Start is called before the first frame update
    void Start()
    {
        Eldonhp = Playervalues.EldonHP;
        Eldonmaxhp = Playervalues.EldonmaxHP;
        EldonNRG = 0;
        EldonmaxNRG = Playervalues.EldonmaxNRG;
        healthbar.SetMaxhealth(Eldonmaxhp);
        healthbar.SetMaxEnergy(EldonmaxNRG);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag =="enemy" & !inv)
        {
            Eldonhp -= other.gameObject.GetComponent<EnemyHP>().enemydamage;
            inv = true;
            iframe = invicibilityframes;
            rb.velocity = new Vector2(rb.velocity.x, hitjumpforce);
        }
    }
    void Update()
    {
        if (Physics2D.OverlapCircle(groundcheck.position, radOcircle, whatisspike) & inv == false)
        {
            Eldonhp = Eldonhp - 5;
            inv = true;
            iframe = invicibilityframes;
            rb.velocity=new Vector2(rb.velocity.x, hitjumpforce);
        }

        healthbar.SetHealth(Eldonhp);
        healthbar.SetEnergy(EldonNRG);

        if (Eldonhp <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (inv == true)
        {
            iframe = iframe - 1;
            if (iframe == 0)
            {
                inv = false;
            }
        }
        if (GetComponent<Rigidbody2D>().position.x>=7 || GetComponent<Rigidbody2D>().position.y <= -3 || GetComponent<Rigidbody2D>().position.y >= 4 || GetComponent<Rigidbody2D>().position.x <= -3)
        {
            if(Eldonhp>1) 
            {
                Eldonhp= Eldonhp - 1;
                GameObject.Find("player1").transform.position = new Vector2(1,1);

            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
            
        }
    }
}