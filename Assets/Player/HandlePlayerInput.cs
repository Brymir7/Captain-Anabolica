using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class HandlePlayerInput : MonoBehaviour
{
    private Rigidbody _rb = null;
    private WalkingAnimation walkingAnimation;
    private WeaponHandling weapon_handler;
    private Camera playerCamera;
    private PlayerMovement _playerMovement;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // Prevent tipping over
        walkingAnimation = GetComponent<WalkingAnimation>(); // Adjust if necessary
        weapon_handler = GetComponent<WeaponHandling>();
        _playerMovement = GetComponent<PlayerMovement>();
        playerCamera = Camera.main;
    }

    
    public float moveSpeed = 15f;

    void Update()
    {
        _playerMovement.UpdateMoveDirection(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetMouseButton(1)) // right mouse button
        {
            weapon_handler.HoldWeapon();
        }
        else
        {
            weapon_handler.StopHoldingWeapon();
        }
        if (Input.GetMouseButton(0)) // left mouse button
        {
            weapon_handler.ShootWeapon(playerCamera);
        }
    }
}
