using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    PlayerControls controls;


    public PlayerInput Attack;

    private Animator playeranim;
  

  

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
        if (delaycounter==0 && !GetComponent<PlayerMovement>().shielded)
        {
            fctTiltAttack();
            delaycounter = attackdelay;
        }
    }

    void Update()
    {

        grounded = GetComponent<PlayerJumpV3>().grounded;
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
            if (enemy.tag != "Player1")
            {
                if(enemy.GetComponent<EnMovement>().shielded)
                {
                    enemy.GetComponent<EnMovement>().shield -= tiltshielddamage;
                }
                else
                {
                    enemy.GetComponent<EnemyHP>().enemyperc += percent;
                    enemyrb = enemy.GetComponent<Rigidbody2D>();
                    playerx = GetComponent<Rigidbody2D>().position.x;


                    if (transform.position.x >= enemy.transform.position.x)
                    {
                        enemyrb.AddForce(new Vector2(-baserecoil * enemy.GetComponent<EnemyHP>().enemyperc, 0));

                    }
                    else
                    {
                        enemyrb.AddForce(new Vector2(baserecoil * enemy.GetComponent<EnemyHP>().enemyperc, 0));
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
