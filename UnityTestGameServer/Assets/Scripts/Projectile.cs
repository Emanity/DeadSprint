using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    Rigidbody projectileRB;
    // Start is called before the first frame update
    void Start()
    {
        projectileRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        projectileRB.AddForce(new Vector3(2, 0));
    }
}
