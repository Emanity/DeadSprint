using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHandler : MonoBehaviour
{
    private Transform transformAim;
    public float damage;

    // Start is called before the first frame update
    private void Awake() => transformAim = transform.Find("Aim");

    // Update is called once per frame
    void Update()
    {
        handleMelee();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && Input.GetMouseButtonDown(0))
        {
            player.getPlayerProperties().damage(damage);
        }


    }

    private void handleMelee()
    {
        Vector3 mousePos = getWorldMousePos();

        //rotates the weapon according to mouse position
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transformAim.eulerAngles = new Vector3(0, 0, angle);
    }

    //calculates current mouse position in relation to the world
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
}
