using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{

    Rigidbody2D rb;
    public float speed;
    public float jumpPower;
    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float rememberGroundedFor;
    public SpriteRenderer spriteRenderer;
    float lastTimeGrounded;
    public Animator animator;

    public bool isFroze;
    

    PhotonView photonView;
    private PlayerProperties playerProperties;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        playerProperties = new PlayerProperties(PhotonNetwork.LocalPlayer);
    }

    private void Start()
    {
        playerProperties = new PlayerProperties(PhotonNetwork.LocalPlayer);
    }

    private void Update()
    {
        Jump();
    }

    

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        isPlayerDead();
        move();
        //Jump();
        //BetterJump();
        CheckIfGrounded();
        regenStamina();
        
    }


    private void attack(Collision2D other)
    {
        if (playerProperties.gethasEquipped() &&  Input.GetKey(KeyCode.H))
        {
            Debug.Log("attack!");
            float dmge = playerProperties.getCurrentWeapon().getDamage();
            other.gameObject.GetComponent<PlayerController>().playerProperties.damage(dmge);
        }
    }

    public PlayerProperties getPlayerProperties()
    {
        return playerProperties;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player") && !playerProperties.gethasEquipped())
        {
            Physics2D.IgnoreCollision(other.collider, GetComponent<Collider2D>());
        } else if (other.gameObject.tag.Equals("Player") )
        {
            attack(other);
        }
        if (other.gameObject.tag.Equals("death"))
        {
            playerProperties.damage(100);
        }

        if (other.gameObject.tag.Equals("gum"))
        {
            playerProperties.damage(80);
            isFroze = true;
            animator.SetBool("frozen", true);
            other.gameObject.SetActive(false);
            StartCoroutine("waitFrozenTime");
        }

        if (other.gameObject.tag.Equals("weapon"))
        {
            playerProperties.setWeapon(other.gameObject.GetComponent<Weapon>());
            Debug.Log("weapon set, damage: " + playerProperties.getCurrentWeapon().getDamage());
            other.gameObject.SetActive(false);
        }
    }

    IEnumerator waitFrozenTime()
    {
        yield return new WaitForSecondsRealtime(5f);
        isFroze = false;
        animator.SetBool("frozen", false);
    }



    private void move()
    {
        if(playerProperties.getStamina() > 0 && !isFroze)
        {
            float x = Input.GetAxisRaw("Horizontal"); //x = key A (-1) , key D (1)

            if (x == -1)
            {
                transform.rotation = new Quaternion(0f, -180f, 0f, 0f);
                animator.SetBool("isJumping", false);
            }

            if (x == 1)
            {
                transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                animator.SetBool("isJumping", false);
            }



            float moveBy = x * speed;
            rb.velocity = new Vector2(moveBy, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }

    }

    private void Jump()
    {
        CheckIfGrounded();
        if (photonView.IsMine && !isFroze && Input.GetKeyDown(KeyCode.W) && playerProperties.getStamina() > 0)
        {
            //Debug.Log("jump happened");
            animator.SetBool("isJumping", true);
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            
            
            
        } 
    }

    private void regenStamina()
    {
        if (playerProperties.getStamina() < 100)
        {
            playerProperties.regenStamina();
        }
    }
   

    private void CheckIfGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (collider != null)
        {
            isGrounded = true;
        } else {
            if (isGrounded) {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }

    private void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * ((fallMultiplier - 1) * Time.deltaTime);
        } else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.W)) {
            rb.velocity += Vector2.up * Physics2D.gravity * ((lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }


    private void isPlayerDead()
    {

        if (playerProperties.isPlayerDead() || (photonView.IsMine && Input.GetKeyDown(KeyCode.R)))
        {
            gameObject.transform.position = new Vector3(-8f,-2.5f);
        }
    }
}
