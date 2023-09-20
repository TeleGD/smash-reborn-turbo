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
        enanim = GetComponent<Animator>();
    }


    void OnAttack1()
    {
        if (delaycounter==0)
        {
            TiltAttack();
            delaycounter = attackdelay;
        }
    }

    void Update()
    {

        grounded = GetComponent<EnJumpV3>().grounded;
        //attack cooldown
        if (delaycounter>0)
        {
            delaycounter -= 1;
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
            if (enemy.tag == "Player")
            {
                enemy.GetComponent<PlayerHP>().player1percent += percent;
                enemyrb = enemy.GetComponent<Rigidbody2D>();
                playerx = GetComponent<Rigidbody2D>().position.x;

           
                
                
                if(transform.position.x>= enemy.transform.position.x)
                {
                    enemyrb.AddForce(new Vector2(-baserecoil* enemy.GetComponent<PlayerHP>().player1percent, 0));
                    
                }
                else
                {
                    enemyrb.AddForce(new Vector2(baserecoil* enemy.GetComponent<PlayerHP>().player1percent, 0));
                }
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);


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
