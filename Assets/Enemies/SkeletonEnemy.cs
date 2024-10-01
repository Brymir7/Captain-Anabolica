public class SkeletonEnemy : Enemy
{
    private BipedalIK _bipedal;

    public void Start()
    {
        _bipedal = GetComponent<BipedalIK>();
    }

    public override void Attack()
    {
    }

    public override void Move()
    {
        transform.position += velocity;
        _bipedal.SetVelocity(velocity);
    }
}