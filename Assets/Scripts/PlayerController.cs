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

    private bool jumpDisabled;
    private PlayerProperties playerProperties;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (other.gameObject.tag.Equals("death")) //some reason this does not work
        {
            playerProperties.damage(100);
        }

        if (other.gameObject.tag.Equals("gum"))
        {
            Debug.Log($"[[[]]]====-");
            playerProperties.useStamina(80);
            isFroze = true;
            animator.SetBool("frozen", true);
            Destroy(other.gameObject);
            StartCoroutine(waitFrozenTime());
        }

        if (other.gameObject.tag.Equals("weapon"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("weaponEquip", RpcTarget.Others, other.gameObject.GetInstanceID());
                Weapon weapon = other.gameObject.GetComponent<Weapon>();
                playerProperties.setWeapon(weapon);
                weapon.gameObject.SetActive(false);
            }
        }

        if (other.gameObject.tag.Equals("Laser"))
        {
            playerProperties.damage(25);
        }

        if (other.gameObject.tag.Equals("Platforms"))
        {
            jumpDisabled = false;
        }
    }

    IEnumerator waitFrozenTime()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        isFroze = false;
        animator.SetBool("frozen", false);
    }



    private void move()
    {
        Debug.Log($"player is frozen? {isFroze}");
        if(playerProperties.getStamina() > 0 && !isFroze)
        {
            float x = Input.GetAxisRaw("Horizontal"); //x = key A (-1) , key D (1)
            if (x == -1)
            {
                transform.rotation = new Quaternion(0f, -180f, 0f, 0f);
                animator.SetBool("frozen", false);
                animator.SetBool("isJumping", false);
            }

            if (x == 1)
            {
                transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                animator.SetBool("frozen", false);
                animator.SetBool("isJumping", false);
            }

            if (x == 0)
            {
                animator.SetBool("frozen", true);
            }



            float moveBy = x * speed;
            rb.velocity = new Vector2(moveBy, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }

    }

    private void Jump()
    {
        if (jumpDisabled) return;

        CheckIfGrounded();
        if (photonView.IsMine && !isFroze && Input.GetKeyDown(KeyCode.W) && playerProperties.getStamina() > 20f)
        {
            //Debug.Log("jump happened");
            animator.SetBool("isJumping", true);
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            playerProperties.useStamina(20);

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

        if (playerProperties.isPlayerDead() || gameObject.transform.position.x <= -14f)
        {
            gameObject.transform.position = new Vector3(-7f, -2.5f);
        }
    }

    [PunRPC]
    private void weaponEquip(int _test)
    {
        GameObject[] _weaponObjects = GameObject.FindGameObjectsWithTag("weapon");
        for (int i = 0; i < _weaponObjects.Length; i++)
        {
            if (_weaponObjects[i].GetInstanceID().Equals(_test))
            {
                Debug.Log($"weapon instance id: {_test}");
                playerProperties.setWeapon(_weaponObjects[i].gameObject.GetComponent<Weapon>());
                Debug.Log("weapon set, damage: " + playerProperties.getCurrentWeapon().getDamage());
                _weaponObjects[i].gameObject.SetActive(false);
            }
        }
    }

}
