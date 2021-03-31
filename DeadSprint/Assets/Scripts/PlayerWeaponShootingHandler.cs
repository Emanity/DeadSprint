using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponShootingHandler : MonoBehaviour
{
    [SerializeField] private Transform pfGum;
    public int ammunitionLimit = 10;
    private int currentAmmunition;

    //Assigns the onfiring event in the player weapon handles to the onfiring event in this class
    private void Awake()
    {
        GetComponent<PlayerWeaponAimHandler>().onFiring += Pwdah_onFiring;
        // loading the chewing gum weapon with gum ammo
        currentAmmunition = ammunitionLimit;
    } 
        

    private void Pwdah_onFiring(object sender, PlayerWeaponAimHandler.onFiringArgs e)
    {
        Debug.Log("shooting");
        //Spawns the bullet
        Transform gumTransform =  Instantiate(pfGum, e.weaponEndPointPos, Quaternion.identity);
        //decrement ammunition every time gum is instantiated
        currentAmmunition--;
        //Calculates a shoot direction which is the current direction the bullet should be facing 
        Vector3 shootDirection = (e.shootPos - e.weaponEndPointPos).normalized;
        //Passes the value to the Bullet class
        gumTransform.GetComponent<Bullet>().Setup(shootDirection);
    }

    public int getCurrentAmmo()
    {
        return currentAmmunition;
    }
   
}


