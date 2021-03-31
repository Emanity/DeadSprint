using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 shootDirection;
    public float damage;

   public void Setup(Vector3 shootDirection)
    {
        this.shootDirection = shootDirection;
        //Makes sure bullet is facing the correct direction as it transforms
        transform.eulerAngles = new Vector3(0, 0, floatVectorAngle(shootDirection));
        //Handles Bullet life time
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        //Bullet travel speed
        float moveSpeed = 10f;
        //Handles bullet movement
        transform.position += shootDirection * moveSpeed * Time.deltaTime;
    }

    //Converts the Vector3 value into one that can be understood by the EulerAngles
    public float floatVectorAngle(Vector3 direction)
    {
        direction = direction.normalized;
        float n = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (n < 0) n += 360;
        return n;
    }

    //Checks for bullet collision
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();



        if (collision.CompareTag("Player"))
        {
            player.getPlayerProperties().damage(damage);
            Destroy(gameObject);
        }


    }
}
