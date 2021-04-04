using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite changeToSprite;
    private PlayerController player;
    private PlayerWeaponAimHandler weaponCheck;

    private void Awake() => spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Collects the necessary data from the collision object and holds them in a global variables
        player = collision.GetComponent<PlayerController>();
        weaponCheck = collision.GetComponent<PlayerWeaponAimHandler>();
        ConsumableHandler consumer = collision.GetComponent<ConsumableHandler>();

        //Checks if it has collided with a player
        if (collision.CompareTag("Player"))
        {
            //Checks if the slot is occupyed 
            if (weaponCheck.isWeaponThere() == false)
            {
                //If slot is free add the consumable object to the slot
                consumer.updateObject(gameObject);
                
            }
        }
    }

   //Updates the sprite renderer's current sprite to the change to sprite passes in as a variable
    public void changeSprite()
    {
        spriteRenderer.sprite = changeToSprite;
    }

    //Accessor method for the player properties class
    public PlayerController getPlayer()
    {
        return player;
    }

}
