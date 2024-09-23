using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class DefaultBullet : MonoBehaviour
{
    public float speed = 100f;
    public float damage = 1f;
    private Rigidbody _rb;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_rb.position.magnitude > 100f)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetDirection(Vector3 direction)
    {
        _rb.velocity = direction * speed;
    }
}

