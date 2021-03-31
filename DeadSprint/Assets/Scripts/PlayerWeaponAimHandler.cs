using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponAimHandler : MonoBehaviour
{
    private Transform transformAim;
    private Transform whereToFireFromTransform;
    private float meleeDamage;
    
    public event EventHandler<onFiringArgs> onFiring;
    public class onFiringArgs : EventArgs
        {
        public Vector3 weaponEndPointPos;
        public Vector3 shootPos;
    }
    

    //finding the empty game object slots needed
    private void Awake()
    {
        
        transformAim = transform.Find("Aim");
        
    }

    // Update is called once per frame
    void Update()
    {
        handleAiming();
        handleShooting();
        checkToExpireWeapon();
    }

    //Handles collision between weapon and player in the case of melee weapons
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        //Only inflicts damage on this collision if weapon is melee
        if (collision.CompareTag("Player") && Input.GetMouseButtonDown(0) && isMelee())
        {
            player.getPlayerProperties().damage(meleeDamage);
        }
    }


    //Handles shooting event invoking conditions
    private void handleShooting()
    {
        if (Input.GetMouseButtonDown(0) && GetComponent<PlayerWeaponShootingHandler>().getCurrentAmmo() > 0 && isMelee() == false)
        {
            
            Vector3 mousePos = getWorldMousePos();
            onFiring?.Invoke(this, new onFiringArgs
            {
                weaponEndPointPos = whereToFireFromTransform.position,
                shootPos = mousePos
            });
            Debug.Log(whereToFireFromTransform.position);
        } 
    }
    //Handles aiming mechanics ensuring Weapon is alligned with the mouse position
    private void handleAiming()
    {
        Vector3 mousePos = getWorldMousePos();

        //rotates the weapon according to mouse position
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transformAim.eulerAngles = new Vector3(0, 0, angle);
    }

    //Calculates current mouse position in relation to the world
    private static Vector3 getWorldMousePos()
    {
        Vector3 vector = getWorldPositionWithZ(Input.mousePosition, Camera.main);
        vector.z = 0f;
        return vector;
    }

    //calculates the current position in the world
    public static Vector3 getWorldPositionWithZ(Vector3 screenPos, Camera worldCam)
    {
        Vector3 worldPos = worldCam.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    //instantiates a clone of the weapon picked up off the ground
    public void updateWeapon(GameObject newWeapon, Sprite weaponLook)
    {
        Instantiate(newWeapon, transformAim.position,Quaternion.identity,transformAim);
        Debug.Log(newWeapon.transform.Find("WhereToFireFrom").position);
        whereToFireFromTransform = newWeapon.transform.Find("WhereToFireFrom");
    }

   //Checks to see if the weapon is melee
    private bool isMelee()
    {
        if (transformAim.Find("weapon1(Clone)")) {
            return true;
        } else {
            return false;
        }
    }

    //Deletes all new weapons that enter the Aim game object slots
    public void enforceCurrentWeapon()
    {
        Destroy(transformAim.GetChild(0));
    }

    //Checks if there's a weapon in the Aim game object
    public bool isWeaponThere()
    {
        if (transformAim.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Removes weapon from player after a set requirement is met
    private void checkToExpireWeapon()
    {
        if (isWeaponThere())
        {
            if (isMelee())
            {
                //Removes the melee weapon after 8 seconds 
                Destroy(gameObject.transform.Find("Aim").Find("weapon1(Clone)").gameObject, 8f);
            }
            else
            {
                if (GetComponent<PlayerWeaponShootingHandler>().getCurrentAmmo() == 0)
                {
                    //Removes the weapon after it runs out of bullets 
                    Destroy(gameObject.transform.Find("Aim").Find("gum(Clone)").gameObject);
                }
            }
        } 
    }

    }
