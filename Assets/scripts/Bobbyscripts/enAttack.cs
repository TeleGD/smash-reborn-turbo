using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class enAttack : MonoBehaviour
{
    PlayerControls controls;

    public PlayerInput Attack;

    private Animator enanim;
  


    //size of the attack 
    [Header("hitbox of the attack")]
    public Transform tiltattackpoint;
    public float tiltrange;


    //attack variables
    [Header("Tiltattack variables")]
    public int tiltpercent;
    public int tiltattackdelay; //number of frames between attacks
    public int tiltdelaycounter;
    public int tiltshielddamage;


    [Header("recoil variables")]
    private Rigidbody2D enemyrb;
    private float playerx;
    public float baserecoil;

    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        enanim = GetComponent<Animator>();
    }


    void OnAttack1()
    {
        if (tiltdelaycounter==0 && !GetComponent<EnMovement>().shielded)
        {
            TiltAttack();
            tiltdelaycounter = tiltattackdelay;
        }
    }

    void Update()
    {

        grounded = GetComponent<EnJumpV3>().grounded;
        //attack cooldown
        if (tiltdelaycounter>0)
        {
            tiltdelaycounter -= 1;
        }

    }


    void TiltAttack()
    {
        //attack animation
        enanim.SetTrigger("attack");

        //get enemies in range
        Collider2D[] hitenemies = Physics2D.OverlapCircleAll(tiltattackpoint.position,tiltrange);

        foreach (Collider2D enemy in hitenemies)
        {
            if (enemy.tag != "Player2")
            {
                if(enemy.GetComponent<PlayerMovement>().shielded)
                {
                    enemy.GetComponent<PlayerMovement>().shield -= tiltshielddamage;
                }
                else
                {
                    enemy.GetComponent<PlayerHP>().player1percent += tiltpercent;
                    enemyrb = enemy.GetComponent<Rigidbody2D>();
                    playerx = GetComponent<Rigidbody2D>().position.x;

                    if (transform.position.x >= enemy.transform.position.x)
                    {
                        enemyrb.AddForce(new Vector2(-baserecoil * enemy.GetComponent<PlayerHP>().player1percent, 0));

                    }
                    else
                    {
                        enemyrb.AddForce(new Vector2(baserecoil * enemy.GetComponent<PlayerHP>().player1percent, 0));
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
