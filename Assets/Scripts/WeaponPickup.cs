using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (collision.CompareTag("Player") && collision.GetComponent<PlayerWeaponAimHandler>().isWeaponThere() == true)
        {
            //if weapon is there it deletes the weapon it collides with immediately without deleting it off the environment so other players can still access it
            collision.GetComponent<PlayerWeaponAimHandler>().enforceCurrentWeapon();
        } else {
            //if no weapon is present allows for the pickup of the weapon
            collision.GetComponent<PlayerWeaponAimHandler>().updateWeapon(gameObject, gameObject.GetComponent<Sprite>());
            Destroy(gameObject);
        }
    }

}
