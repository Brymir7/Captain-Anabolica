using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb = null;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // Prevent tipping over
    }

    public  float moveSpeed = 0.15f;
    private Vector3 moveDirection = Vector3.zero;

    public void UpdateMoveDirection(float moveHorizontal, float moveVertical)
    {
        moveDirection = (transform.right * moveHorizontal + transform.forward * moveVertical);
    }

    public void FixedUpdate()
    {
        _rb.MovePosition(transform.position + moveDirection * moveSpeed);
    }
}