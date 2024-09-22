using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class DefaultBullet : MonoBehaviour
{
    private float _speed = 1f;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_rb.position.magnitude > 100f)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetDirection(Vector3 direction)
    {
        _rb.velocity = direction * _speed;
    }
}

