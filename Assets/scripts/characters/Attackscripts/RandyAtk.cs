using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class RandyAtk : MonoBehaviour
{
    PlayerControls controls;


    public PlayerInput Attack;

    private Animator playeranim;


    private bool grabed; //sert à déterminer si un perso est grab. Ce bool est récupéré du script charavalues

    //size of the attack 
    [Header("hitbox of the tilt attack")]
    public Transform tiltattackpoint;
    public float tiltrange;


    //attack variables
    [Header("attack variables")]
    public int percent;
    public int attackdelay; //number of frames between attacks
    public int delaycounter;
    public int tiltshielddamage;


    [Header("recoil variables")]
    private Rigidbody2D enemyrb;
    private float playerx;
    public float baserecoil;

    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        playeranim = GetComponent<Animator>();
    }


    void OnAttack()
    {
        if (!grabed)
        {
            if (this.CompareTag("Player1"))
            {
                if (delaycounter == 0 && !GetComponent<charavalues>().shielded)
                {
                    fctTiltAttack();
                    delaycounter = attackdelay;
                }
            }
        }
        
    }

    void OnAttack1()
    {
        if(!grabed)
        {
            if (this.CompareTag("Player2"))
            {
                if (delaycounter == 0 && !GetComponent<charavalues>().shielded)
                {
                    fctTiltAttack();
                    delaycounter = attackdelay;
                }
            }
        }
        
    }


    void Update()
    {

        grabed = GetComponent<charavalues>().grabed;

        grounded = GetComponent<Charamov>().grounded;
        //attack cooldown
        if (delaycounter>0)
        {
            delaycounter -= 1;
        }

    }


    void fctTiltAttack()
    {
        //attack animation
        playeranim.SetTrigger("attack");

        //get enemies in range
        Collider2D[] hitenemies = Physics2D.OverlapCircleAll(tiltattackpoint.position,tiltrange);

        foreach (Collider2D enemy in hitenemies)
        {
            if (((this.CompareTag("Player2") && enemy.tag == "Player1") || (this.CompareTag("Player1") && enemy.tag == "Player2")) && enemy.GetComponent<charavalues>().iframes == 0)
            {
                if(enemy.GetComponent<charavalues>().shielded)
                {
                    enemy.GetComponent<charavalues>().shield -= tiltshielddamage;
                }
                else
                {
                    enemy.GetComponent<charavalues>().percent += percent;
                    enemyrb = enemy.GetComponent<Rigidbody2D>();
                    playerx = GetComponent<Rigidbody2D>().position.x;


                    if (transform.position.x >= enemy.transform.position.x)
                    {
                        enemyrb.AddForce(new Vector2(-baserecoil * enemy.GetComponent<charavalues>().percent, 0));

                    }
                    else
                    {
                        enemyrb.AddForce(new Vector2(baserecoil * enemy.GetComponent<charavalues>().percent, 0));
                    }
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
                }

            }
            
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(tiltattackpoint.position, tiltrange);
    }
//    void OnEnable()
  //  {
    //    controls.gameplay.Enable();
    //}
    //void OnDisable()
    //{
     //   controls.gameplay.Disable();
    //}
}
