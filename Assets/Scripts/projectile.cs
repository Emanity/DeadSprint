using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class projectile : MonoBehaviour
{
    public float damage;

    private Player p;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        
        

        if (player != null)
        {
            player.getPlayerProperties().damage(damage);
            Destroy(gameObject);
        }

        
    }
    
}
