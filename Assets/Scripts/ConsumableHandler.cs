using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableHandler : MonoBehaviour
{
    private Transform handTransform;
    private GameObject consumable;
    private PlayerWeaponAimHandler weaponcheck;
    private int trackClicks;

    void Awake()
    {
        handTransform = gameObject.transform.Find("Aim");
    }
    // Update is called once per frame
    void Update()
    {
        //Tracks the number of clicks to assign different actions to per click
        trackClicks = 0;

        if (consumable != null)
        {
            if (Input.GetMouseButtonDown(0) && consumable.tag == "Consumable")
            {
                //First click consumes the consumable and changes it's sprite to one representative of the consumed object
                consumable.GetComponent<Consumable>().changeSprite();
                consumable.GetComponent<Consumable>().getPlayer().getPlayerProperties().regenStamina();
                trackClicks++;
                if (Input.GetMouseButtonDown(0) && trackClicks == 1)
                {
                    //Second click removes the consumed object allowing the player to have a free slot again
                    Destroy(consumable, 1f);
                }
            }
        }
    }

    //Adds the Consumable object passed through as the object in the slot
    public void updateObject(GameObject consumable)
    {
        this.consumable = consumable;
        //Makes the game object a child of the handTransform
        consumable.transform.SetParent(handTransform);
        //Moves the object from the floor to where the hand transform is 
        consumable.transform.position = handTransform.position;
    }
}
