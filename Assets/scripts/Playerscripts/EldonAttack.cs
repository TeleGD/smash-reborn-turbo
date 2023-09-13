using System.Collections;
using System.Collections.Generic;
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
    public int nrgdamage;
    public bool slayermode; //true:slayer false:eater
    public int attackdelay; //number of frames between attacks
    public int delaycounter;


    [Header("recoil variables")]
    private int size;
    private Rigidbody2D enemyrb;
    private float playerx;
    public float smallrecoil;
    public float medrecoil;
    public float bigrecoil;

    public bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        eldonanim = GetComponent<Animator>();
        slayermode = true;
        GameObject.Find("slayertext").GetComponent<UnityEngine.UI.Text>().enabled = true;
        GameObject.Find("eatertext").GetComponent<UnityEngine.UI.Text>().enabled = false;

    }

    //Slayer and Eater modes

    void OnModechange()

    {
        Modeswitch();
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


    void Modeswitch()
    {
        if(slayermode)
        {
            slayermode = false;
            GameObject.Find("slayertext").GetComponent<UnityEngine.UI.Text>().enabled = false;
            GameObject.Find("eatertext").GetComponent<UnityEngine.UI.Text>().enabled = true;
        }
            else
        {
            slayermode = true;
            GameObject.Find("slayertext").GetComponent<UnityEngine.UI.Text>().enabled = true;
            GameObject.Find("eatertext").GetComponent<UnityEngine.UI.Text>().enabled = false;
        }
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
            if (enemy.tag == "enemy")
            {
                size = enemy.GetComponent<EnemyHP>().size;
                enemyrb = enemy.GetComponent<Rigidbody2D>();
                playerx = GetComponent<Rigidbody2D>().position.x;

                if (slayermode)
                {
                    enemy.GetComponent<EnemyHP>().enemyhp -= hpdamage;
                    enemy.GetComponent<EnemyHP>().enemyNRG -= (nrgdamage * 1/10);
                    GameObject.Find("player1").GetComponent<PlayerHP>().EldonNRG += nrgdamage * 6 /10 ;
                }
                else
                {
                    enemy.GetComponent<EnemyHP>().enemyhp -= hpdamage * 1/10;
                    enemy.GetComponent<EnemyHP>().enemyNRG -= nrgdamage;
                    GameObject.Find("player1").GetComponent<PlayerHP>().EldonNRG += nrgdamage;
                }
                if (enemyrb.position.x < playerx & enemy.GetComponent<EnemyHP>().enemyhp > 0)
                {
                    if (size==1)
                    {
                        //enemyrb.velocity = new Vector2(smallrecoil, enemyrb.velocity.y);
                        enemyrb.AddForce(new Vector2(-smallrecoil, 0));
                    }
                    if (size == 2)
                    {
                        //enemyrb.velocity = new Vector2(enemyrb.velocity.x - medrecoil, enemyrb.velocity.y);
                    }
                    if (size == 3)
                    {
                        //enemyrb.velocity = new Vector2(enemyrb.velocity.x - bigrecoil, enemyrb.velocity.y);
                    }
                }
                if (enemyrb.position.x > playerx & enemy.GetComponent<EnemyHP>().enemyhp > 0)
                {
                    if (size == 1)
                    {
                        // enemyrb.velocity = new Vector2(smallrecoil, enemyrb.velocity.y);
                        enemyrb.AddForce(new Vector2(smallrecoil, 10f));
                    }
                    if (size == 2)
                    {
                       // enemyrb.velocity = new Vector2(enemyrb.velocity.x + medrecoil, enemyrb.velocity.y);
                    }
                    if (size == 3)
                    {
                       // enemyrb.velocity = new Vector2(enemyrb.velocity.x + bigrecoil, enemyrb.velocity.y);
                    }
                }

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
