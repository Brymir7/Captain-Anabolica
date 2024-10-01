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
    private Vector3 _moveDirection = Vector3.zero;

    public void UpdateMoveDirection(float moveHorizontal, float moveVertical)
    {
        _moveDirection = (transform.right * moveHorizontal + transform.forward * moveVertical);
    }

    public void FixedUpdate()
    {
        _rb.MovePosition(transform.position + _moveDirection * moveSpeed);
    }
}