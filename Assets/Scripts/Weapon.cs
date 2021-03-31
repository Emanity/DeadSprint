using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Modes
    { melee, projectile, throwable}

    /*    public Sprite sprite;
        public GameObject projectile;
        public float projectileSpeed;
        public float fireRate;
        public Modes projectileMode;*/

    public float damage = 20f;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    public float getDamage()
    {
        return damage;
    }
    public void setDamage(float _damage)
    {
        damage = _damage;
    }


} 
