using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class EldonAttack : MonoBehaviour
{
    PlayerControls controls;

    public PlayerInput Modechange;
    public PlayerInput Attack;

    private Animator eldonanim;
  

    [Header("wall variables")]
    public bool istherewall;
    [SerializeField] private LayerMask whatiswall;

    //size of the attack 
    [Header("hitbox of the attack")]
    public Transform attackpoint;
    public float range;
    public Collider2D walljumped;


    //attack variables
    [Header("attack variables")]
    public int nrgregen;
    public int hpdamage;
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
        eldonanim = GetComponent<Animator>();
    }


    void OnAttack()
    {
        if (delaycounter==0 && !GameObject.Find("player1").GetComponent<PlayerJumpV3>().stuckinwall)
        {
            fctAttack();
            delaycounter = attackdelay;
        }
    }

    void Update()
    {

        grounded = GameObject.Find("player1").GetComponent<PlayerJumpV3>().grounded;
        //attack cooldown
        if (delaycounter>0)
        {
            delaycounter -= 1;
        }

        istherewall = Physics2D.OverlapCircle(attackpoint.position, range,whatiswall);

    }


    void fctAttack()
    {
        //attack animation
        eldonanim.SetTrigger("attack");

        //get enemies in range
        Collider2D[] hitwalls = Physics2D.OverlapCircleAll(attackpoint.position, range,whatiswall);
        Collider2D[] hitenemies = Physics2D.OverlapCircleAll(attackpoint.position,range);

        foreach (Collider2D enemy in hitenemies)
        {
            if (enemy.tag == "Player2")
            {
                enemy.GetComponent<EnemyHP>().enemyperc += percent;
                enemyrb = enemy.GetComponent<Rigidbody2D>();
                playerx = GetComponent<Rigidbody2D>().position.x;

           
                
                
                if(GameObject.Find("player1").transform.position.x>= enemy.transform.position.x)
                {
                    enemyrb.AddForce(new Vector2(-baserecoil* enemy.GetComponent<EnemyHP>().enemyperc, 0));
                    
                }
                else
                {
                    enemyrb.AddForce(new Vector2(baserecoil* enemy.GetComponent<EnemyHP>().enemyperc, 0));
                }
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
                UnityEngine.Debug.Log(enemy.GetComponent<EnemyHP>().enemyperc);


            }
            
        }
        if (istherewall && !grounded)
        {
            GameObject.Find("player1").GetComponent<PlayerJumpV3>().stuckinwall = true;

        }
        foreach (Collider2D wall in hitwalls)
        {
            walljumped = wall;
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
