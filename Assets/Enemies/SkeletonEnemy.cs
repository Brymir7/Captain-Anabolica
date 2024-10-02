using UnityEngine;

namespace Enemies
{
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
            transform.position += Velocity;
            _bipedal.SetVelocity(Velocity);
        }
    }
}