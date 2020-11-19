using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UIElements;

public class PickableObject : MonoBehaviour
{
    //change this later to use this as a parent class for multiple picakble object
    //private string objectType;
    //private bool isEquippable;
    private int id;
    private GameObject pickableObject;
    private Vector3 objectPosition;
    private int healAmount = 30;
    private float staminaAmount = 25;

    public void inititialize(int _id, Vector3 _position, GameObject _gameObject)
    {
        id = _id;
        objectPosition = _position;
        pickableObject = _gameObject;
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.tag == "Player")
        {
            Player _player = _collider.gameObject.GetComponent<Player>();
            _player.heal(healAmount);
            _player.recoverStamina(staminaAmount);
            Destroy(gameObject);
            Destroy(this);
        }
    }
}
