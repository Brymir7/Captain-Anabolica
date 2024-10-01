using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WormEnemy : Enemy
{
    public int amountOfSegments = 3;
    public GameObject jointPrefab;
    public GameObject connectorPrefab;
    public GameObject target;
    private List<GameObject> _joints = new List<GameObject>();
    private List<GameObject> _connectors = new List<GameObject>();
    private IK _ik;

    public override void Attack()
    {
    }

    public override void TargetPlayer()
    {
        velocity = (Player.transform.position - _ik.target).normalized;
        velocity.y = 0;
    }

    public override void LookAtPlayer()
    {
    }

    public override void Move()
    {
        _ik.target += velocity * MoveSpeed;
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

        if (target == null)
        {
            target = new GameObject("WormTarget");
            target.transform.parent = transform;
            target.transform.position = _joints[_joints.Count - 1].transform.position;
        }

        _ik.target = target.transform.position;
        _ik.Awake();
    }

    new void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            ProjectileBase bullet = other.GetComponent<ProjectileBase>();
            var jointToRemove = _joints[_joints.Count - 1];
            var connectorToRemove = _connectors[_connectors.Count - 1];
            var dmg = bullet.GetDamage();
            Destroy(other.gameObject);
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