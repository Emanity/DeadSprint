using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject activeWeapon;
    Weapon wpn;
    // Start is called before the first frame update
    void Start()
    {
        wpn = activeWeapon.GetComponent<Weapon>();
        //GetComponent<SpriteRenderer>().sprite = wpn.sprite;
    }

    // Update is called once per frame
    void Update()
    {
/*        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 rotation = transform.parent.localScale.x == 1 ? Vector3.zero : Vector3.forward * 180;
            GameObject projectile = (GameObject)Instantiate(wpn.projectile, transform.position + activeWeapon.transform.GetChild(0).localPosition, Quaternion.Euler(rotation));
            
            if (wpn.projectileMode == Weapon.Modes.projectile)
            {
                projectile.GetComponent<Rigidbody2D>().velocity = transform.parent.localScale.x * Vector2.right * wpn.projectileSpeed;
            }
        }*/
    }
}
