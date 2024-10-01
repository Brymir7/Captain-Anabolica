using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerViewControl : MonoBehaviour
{
    public Transform player;
    private float _mouseSensitivity = 100f;
    [FormerlySerializedAs("_rotationLimits")] public Vector2 rotationLimits = new Vector2(-90f, 60f);
    private float _verticalRotation = 0f;
    private float _horizontalRotation = 0f;
    public Vector3 offset = new Vector3(2, 0f, -1.5f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _horizontalRotation += mouseX;
        player.Rotate(Vector3.up * mouseX); // Rotates the player horizontally

        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, rotationLimits.x, rotationLimits.y);

        // Modify offset based on vertical rotation to move camera up/down
        float verticalOffset = 0f;
        if (_verticalRotation > 0)
        {
            verticalOffset = Mathf.Lerp(0f, 2f, _verticalRotation / rotationLimits.y);
        }
        else if (_verticalRotation < 0f)
        {
            verticalOffset = Mathf.Lerp(0f, -2f - offset.y, _verticalRotation / rotationLimits.x);
        }
        float distanceModifier = Mathf.Lerp(1f, 0f, MathF.Abs( _verticalRotation) / rotationLimits.y);
        Vector3 adjustedOffset = new Vector3(offset.x, verticalOffset + offset.y, offset.z * distanceModifier);

        transform.position = player.position + player.transform.rotation * adjustedOffset;
        transform.LookAt(player.position);
        transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }
}