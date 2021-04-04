using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageHandler : MonoBehaviour
{
    public float damage;

    //Handles collision between weapon and player in the case of melee weapons
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();



        if (collision.CompareTag("Player") && Input.GetMouseButtonDown(0))
        {
            //Passes the damage to the player properties
            player.getPlayerProperties().damage(damage);
        }


    }
}
