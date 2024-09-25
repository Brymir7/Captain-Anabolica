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
    }
}
