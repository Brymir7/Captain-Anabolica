using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : Enemy
{
    public override void Attack()
    {
    }

    public override void LookAtPlayer()
    {
        transform.rotation = Quaternion.LookRotation(-velocity);
        transform.rotation = new Quaternion(0.0f, transform.rotation.y, 0.0f, transform.rotation.w);
    }
}
