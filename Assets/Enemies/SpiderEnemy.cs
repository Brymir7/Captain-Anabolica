using UnityEngine;
namespace Enemies
{
    public class SpiderEnemy : Enemy
    {

        protected override void OnDirectionUpdate(Vector3 newDirection)
        {
            OffsetMovementRandomly(newDirection);
        }

        private void OffsetMovementRandomly(Vector3 playerDirection)
        {
            Vector3 towardsPlayer = playerDirection.normalized;
            Vector3 perpendicular = Vector3.Cross(towardsPlayer, Vector3.up).normalized;
            float randomOffset = Random.Range(-1f, 1f);
            Vector3 lateralOffset = perpendicular * (randomOffset * MoveSpeed);
            Velocity += lateralOffset;
            Velocity = Velocity.normalized * MoveSpeed;
        }

    }
}