using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerWeaponAimHandler player = collision.gameObject.GetComponent<PlayerWeaponAimHandler>();

        //Checks if it has collided with a player
        if(collision.CompareTag("Player") && player.isWeaponThere() == false && gameObject.CompareTag("weapon"))
        {
            //Calls the update function passing the game object this script is attached to
            player.updateWeapon(gameObject);
        }
    }
}
