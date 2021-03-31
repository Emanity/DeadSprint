using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class PlayerProperties
{
    private Player player;
    private float health;
    private float stamina;
    private float staminaRegenRate;
    private bool hasEquipped;
    private Weapon weaponEquipped;
    private GameObject playerObject;

    
    public PlayerProperties(Player _player)
    {
        player = _player;
        health = 100;
        stamina = 100;
        staminaRegenRate = 0.1f; //must choose wisely on how much it regens per tick
        hasEquipped = false;
        weaponEquipped = null;
    }

    #region  functions to change player data
    //I added for any data change for the player we will need to pass the 
    //playerID this is to ensure we are changing it to the right player
    public void damage(float _damage)
    {
        //if (!matchID(_player)) return;
        stamina -= _damage;
        //!sync! send damage update to other clients on PUN
    }

    public void damage(Player _player, float _damage)
    {
        if (!matchID(_player)) return;
        health -= _damage;
        //!sync! send damage update to other clients on PUN
    }

    //note: we dont have a sprint mechanic yet
    public void useStamina(Player _player, float _staminaUsed)
    {
        if (!matchID(_player)) return;
        stamina -= _staminaUsed;
    }

    public void setWeapon(Player _player, Weapon _weapon)
    {
        if (!matchID(_player)) return;
        weaponEquipped = _weapon;
        hasEquipped = true;
        //!sync! send weapon equipped update to other clients on PUN

    }

    public void regenStamina()
    {
        stamina = stamina + staminaRegenRate;
        if (stamina > 100)
        {
            stamina = 100;
        }
    }

    public void setWeapon(Weapon _weapon)
    {
        weaponEquipped = _weapon;
        hasEquipped = true;
    }

    //can use overload to give no parameters to set it to no weapon equipped
    //or we can just change this to an appropriate name
    public void setWeapon(Player _player)
    {
        if (!matchID(_player)) return;
        weaponEquipped = null;
        hasEquipped = false;
        //!sync! send weapon equipped update to other clients on PUN
    }


    #endregion
    
    #region functions to get player data
    /*
    * we will need to do this to prevent direct change to the player properties
    */
    public float getHealth()
    {
        return health;
    }

    public float getStamina()
    {
        return stamina;
    }

    public float getStaminaRegenRate()
    {
        return staminaRegenRate;
    }

    public Weapon getCurrentWeapon()
    {
        return weaponEquipped;
    }


    #endregion

    private bool matchID(Player _player)
    {
        return _player.Equals(player);
    }

    public bool gethasEquipped()
    {
        return hasEquipped;
    }

    public bool isPlayerDead()
    {
        if (stamina <= 0)
        {
            stamina = 100;
            return true;
        }

        return false;
    }
}
