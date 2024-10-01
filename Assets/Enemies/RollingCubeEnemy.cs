using UnityEngine;
using UnityEngine.Serialization;

public class RollingCubeEnemy : Enemy
{
    private Rigidbody _rb;
    [FormerlySerializedAs("_toppleForce")] public float toppleForce = 2.1f;
    private bool _isToppling = false;
    [FormerlySerializedAs("next_step_after_s")] public float nextStepAfterS = 0.7f;
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
            float absX = Mathf.Abs(velocity.x);
            float absZ = Mathf.Abs(velocity.z);
            float maxComponent = Mathf.Max(absX, absZ);

            if (maxComponent == absZ)
            {
                direction = velocity.z > 0 ? Vector3.forward : Vector3.back;
            }
            else if (maxComponent == absX)
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
        _rb.AddForceAtPosition(direction * toppleForce, transform.position + Vector3.up * 0.5f, ForceMode.Impulse);
        Invoke("ResetTopple", nextStepAfterS);
    }
    public override void TargetPlayer()
    {
        velocity = (Player.transform.position - transform.position).normalized;
    }

    void ResetTopple()
    {
        _isToppling = false;
    }

}