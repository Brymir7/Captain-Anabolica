using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewControl : MonoBehaviour
{
    public Transform player;
    private float mouseSensitivity = 100f;
    public Vector2 _rotationLimits = new Vector2(-90f, 90f);
    private float _verticalRotation = 0f;
    private float _horizontalRotation = 0f;
    private Vector3 offset = new Vector3(2, 0f, -3f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Horizontal rotation rotates the player
        _horizontalRotation += mouseX;
        player.Rotate(Vector3.up * mouseX); // Rotates the player horizontally

        // Vertical rotation for the camera
        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, _rotationLimits.x, _rotationLimits.y);

        // Modify offset based on vertical rotation to move camera up/down
        float verticalOffset = 0f;
        if (_verticalRotation > 0)
        {
            verticalOffset = Mathf.Lerp(0f, 2f, _verticalRotation / _rotationLimits.y);
        }
        else if (_verticalRotation < 0f)
        {
            verticalOffset = Mathf.Lerp(0f, -2f - offset.y, _verticalRotation / _rotationLimits.x);
        }

        float distanceModifier = Mathf.Lerp(1f, 0f, MathF.Abs( _verticalRotation) / _rotationLimits.y);
        Vector3 adjustedOffset = new Vector3(offset.x, verticalOffset + offset.y, offset.z * distanceModifier);


        transform.position = player.position + player.transform.rotation * adjustedOffset;


        transform.LookAt(player.position);


        transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }
}