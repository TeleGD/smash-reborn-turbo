using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    PlayerControls controls;


    public PlayerInput Attack;

    private Animator playeranim;
  

  

    //size of the attack 
    [Header("hitbox of the attack")]
    public Transform attackpoint;
    public float range;


    //attack variables
    [Header("attack variables")]
    public int percent;
    public int attackdelay; //number of frames between attacks
    public int delaycounter;


    [Header("recoil variables")]
    private int size;
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
        if (delaycounter==0)
        {
            fctAttack();
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


    void fctAttack()
    {
        //attack animation
        playeranim.SetTrigger("attack");

        //get enemies in range
        Collider2D[] hitenemies = Physics2D.OverlapCircleAll(attackpoint.position,range);

        foreach (Collider2D enemy in hitenemies)
        {
            if (enemy.tag == "Player2")
            {
                enemy.GetComponent<EnemyHP>().enemyperc += percent;
                enemyrb = enemy.GetComponent<Rigidbody2D>();
                playerx = GetComponent<Rigidbody2D>().position.x;

           
                
                
                if(transform.position.x>= enemy.transform.position.x)
                {
                    enemyrb.AddForce(new Vector2(-baserecoil* enemy.GetComponent<EnemyHP>().enemyperc, 0));
                    
                }
                else
                {
                    enemyrb.AddForce(new Vector2(baserecoil* enemy.GetComponent<EnemyHP>().enemyperc, 0));
                }
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);


            }
            
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackpoint.position, range);
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
