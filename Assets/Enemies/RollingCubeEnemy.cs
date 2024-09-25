using UnityEngine;

public class RollingCubeEnemy : Enemy
{
    private Rigidbody _rb;
    public float _toppleForce = 2.1f;
    private bool _isToppling = false;
    public float next_step_after_s = 0.7f;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void LookAtPlayer()
    {
    }

    public override void Move()
    {
        if (!_isToppling)
        {
            Vector3 direction = Vector3.zero;   
            float abs_x = Mathf.Abs(velocity.x);
            float abs_z = Mathf.Abs(velocity.z);
            float maxComponent = Mathf.Max(abs_x, abs_z);

            if (maxComponent == abs_z)
            {
                direction = velocity.z > 0 ? Vector3.forward : Vector3.back;
            }
            else if (maxComponent == abs_x)
            {
                direction = velocity.x > 0 ? Vector3.right : Vector3.left;
            }

            if (direction != Vector3.zero)
            {
                Topple(direction);
            }
        } 
    }
    public override void Attack()
    {
    }
    void Topple(Vector3 direction)
    {
        _isToppling = true;
        _rb.AddForceAtPosition(direction * _toppleForce, transform.position + Vector3.up * 0.5f, ForceMode.Impulse);
        Invoke("ResetTopple", next_step_after_s);
    }
    public override void TargetPlayer()
    {
        velocity = (player.transform.position - transform.position).normalized;
    }

    void ResetTopple()
    {
        _isToppling = false;
    }
    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Bullet")
        {
            DefaultBullet bullet = other.GetComponent<DefaultBullet>();
            Destroy(other.gameObject);
            TakeDamage(bullet.damage);
        }
    }
}