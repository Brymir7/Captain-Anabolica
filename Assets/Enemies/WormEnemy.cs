using System.Collections.Generic;
using Player.Weapons;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemies
{
    public class WormEnemy : Enemy
    {
        public int amountOfSegments = 12;
        public GameObject jointPrefab;
        public GameObject connectorPrefab;
        private GameObject _target;
        private List<GameObject> _joints = new List<GameObject>();
        private List<GameObject> _connectors = new List<GameObject>();
        private IK _ik;

        public override void Attack()
        {
        }

        public override void TargetPlayer()
        {
            Velocity = (Player.transform.position - _ik.target).normalized;
            Velocity.y = 0;
        }

        public override void LookAtPlayer()
        {
        }

        public override void Move()
        {
            _ik.target += Velocity * MoveSpeed;
        }

        void Start()
        {
            _ik = GetComponent<IK>();
            Assert.IsTrue(_ik != null);
            CreateWorm();
        }

        void CreateWorm()
        {
            Vector3 startPosition = transform.position;
            for (int i = 0; i < amountOfSegments; i++)
            {
                GameObject newJoint = Instantiate(jointPrefab, startPosition, Quaternion.identity);
                newJoint.transform.parent = transform;
                _ik.joints.Add(newJoint.transform);
                _joints.Add(newJoint);
                if (i > 0)
                {
                    GameObject newConnector = Instantiate(connectorPrefab, startPosition, Quaternion.identity);
                    newConnector.transform.parent = transform;
                    Connector connectorScript = newConnector.GetComponent<Connector>();
                    Assert.IsTrue(connectorScript != null);
                    connectorScript.startJoint = _joints[i - 1].transform;
                    connectorScript.endJoint = newJoint.transform;
                    connectorScript.UpdateConnector();
                    BoxCollider boxCollider = newConnector.AddComponent<BoxCollider>();

                    _connectors.Add(newConnector);
                }

                startPosition += transform.forward;
            }

            
            _target = new GameObject("WormTarget");
            _target.transform.parent = transform;
            _target.transform.position = _joints[_joints.Count - 1].transform.position;
            _ik.target = _target.transform.position;
            _ik.Awake();
        }

        protected override void BulletOnTriggerEnterCallback(Collider other)
        {
            ProjectileBase bullet = other.GetComponent<ProjectileBase>();
            HandleBulletInteraction(bullet);
            Instantiate(onHitVFX, other.transform.position, Quaternion.identity);
        }

        protected override void BulletOnCollisionEnterCallback(Collision collision)
        {
            ProjectileBase bullet = collision.gameObject.GetComponent<ProjectileBase>();
            HandleBulletInteraction(bullet);
            Instantiate(onHitVFX, collision.contacts[0].point, Quaternion.identity);
        }

        private void HandleBulletInteraction(ProjectileBase bullet)
        {
            var jointToRemove = _joints[_joints.Count - 1];
            var connectorToRemove = _connectors[_connectors.Count - 1];
            var dmg = bullet.GetDamage();
            _joints.Remove(jointToRemove);
            _connectors.Remove(connectorToRemove);
            Destroy(jointToRemove);
            Destroy(connectorToRemove);

            amountOfSegments -= dmg;
            if (amountOfSegments <= 1)
            {
                dmg += 1;
            }

            _ik.RemoveLastJoint();
            TakeDamage(dmg);
        }
    }
}