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
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // Prevent tipping over
        walkingAnimation = GetComponent<WalkingAnimation>(); // Adjust if necessary
        weapon_handler = GetComponent<WeaponHandling>();
        playerCamera = Camera.main;
    }

    
    public float moveSpeed = 15f;

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveVertical) * (moveSpeed * Time.deltaTime);
    
        if (Input.GetMouseButton(1)) // right mouse button
        {
            weapon_handler.HoldWeapon();
        }
        else
        {
            weapon_handler.StopHoldingWeapon();
        }
        if (Input.GetMouseButton(0)) // right mouse button
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var hit_sth = Physics.Raycast(ray, out RaycastHit hit);
            Vector3 target;
            if (!hit_sth)
            {
                target = ray.GetPoint(1000.0f); // If nothing is hit, shoot towards far point.
            }
            else
            {
                target = hit.point; // Set direction towards the hit point.
            }
            
            weapon_handler.ShootWeapon(target);
        }

        
        // Movement
         if (movement == Vector3.zero)
         {
             walkingAnimation.UpdateAnimationSpeed(0f);
             return;
         }
        _rb.MovePosition(transform.position + movement);
        walkingAnimation.UpdateAnimationSpeed(moveSpeed); 
    }
}
