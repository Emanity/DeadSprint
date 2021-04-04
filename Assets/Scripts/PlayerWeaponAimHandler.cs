using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponAimHandler : MonoBehaviour
{
    private Transform transformAim;
    private Transform whereToFireFromTransform;
    private float meleeDamage;
    private GameObject newWeapon;
    
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

    //Handles shooting event invoking conditions
    private void handleShooting()
    {
        if (whereToFireFromTransform != null)
        {
            if (Input.GetMouseButtonDown(0) && GetComponent<PlayerWeaponShootingHandler>().getCurrentAmmo() > 0 && isMelee() == false)
            {
                Vector3 mousePos = getWorldMousePos();
                onFiring?.Invoke(this, new onFiringArgs
                {
                    weaponEndPointPos = whereToFireFromTransform.position,
                    shootPos = mousePos
                });
                Debug.Log(GetComponent<PlayerWeaponShootingHandler>().getCurrentAmmo());
            }
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

    //changes the position of the weapon from the ground to the empty gameObject on the player
    public void updateWeapon(GameObject newWeapon)
    {
        if (newWeapon.CompareTag("weapon"))
        {
            this.newWeapon = newWeapon;
            newWeapon.transform.SetParent(transformAim);
            newWeapon.transform.position = transformAim.position;
            whereToFireFromTransform = newWeapon.transform.Find("WhereToFireFrom");
        }
    }

   //Checks to see if the weapon is melee
    private bool isMelee()
    {
        if (transformAim.Find("weapon1")) {
            return true;
        } else {
            return false;
        }
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
                Destroy(newWeapon, 8f);
            }
            else
            {
                
                if (GetComponent<PlayerWeaponShootingHandler>().getCurrentAmmo() == 0)
                {
                    //Removes the weapon after it runs out of bullets 
                    Destroy(newWeapon);
                }
                
            }
        } 
    }

    }
