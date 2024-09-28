using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class HandlePlayerInput : MonoBehaviour
{
    private Rigidbody _rb = null;
    private WeaponHandling weapon_handler;
    private Camera playerCamera;
    private PlayerMovement _playerMovement;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // Prevent tipping over
        weapon_handler = GetComponent<WeaponHandling>();
        _playerMovement = GetComponent<PlayerMovement>();
        playerCamera = Camera.main;
    }

    
    public float moveSpeed = 15f;

    void Update()
    {
        _playerMovement.UpdateMoveDirection(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetMouseButton(0)) // left mouse button
        {
            weapon_handler.ShootWeapon(playerCamera);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            weapon_handler.SwitchWeapon();
        }
    }
}
