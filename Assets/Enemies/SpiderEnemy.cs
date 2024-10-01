using UnityEngine;

public class SpiderEnemy : Enemy
{
    public override void Attack()
    {
    }

    public override void LookAtPlayer()
    {
        Vector3 direction = Player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

}