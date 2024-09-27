using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class WormEnemy : Enemy
{
    public int amountOfSegments = 3;
    public GameObject jointPrefab;
    public GameObject connectorPrefab;
    public GameObject target;
    private List<GameObject> joints = new List<GameObject>();
    private List<GameObject> connectors = new List<GameObject>();
    private IK ik;

    public override void Attack()
    {
    }

    public override void TargetPlayer()
    {
        velocity = (player.transform.position - ik.target.position).normalized;
        velocity.y = 0;
    }

    public override void LookAtPlayer()
    {
    }

    public override void Move()
    {
        ik.target.position += velocity * moveSpeed;
    }

    void Start()
    {
        ik = GetComponent<IK>();
        Assert.IsTrue(ik != null);
        CreateWorm();
    }

    void CreateWorm()
    {
        if (ik.joints.Count > 0)
        {
            return;
        }

        Vector3 startPosition = transform.position;
        for (int i = 0; i < amountOfSegments; i++)
        {
            GameObject newJoint = Instantiate(jointPrefab, startPosition, Quaternion.identity);
            newJoint.transform.parent = transform;
            ik.joints.Add(newJoint.transform);
            joints.Add(newJoint);
            if (i > 0)
            {
                GameObject newConnector = Instantiate(connectorPrefab, startPosition, Quaternion.identity);
                newConnector.transform.parent = transform;
                Connector connectorScript = newConnector.GetComponent<Connector>();
                Assert.IsTrue(connectorScript != null);
                connectorScript.startJoint = joints[i - 1].transform;
                connectorScript.endJoint = newJoint.transform;
                connectorScript.UpdateConnector();
                BoxCollider boxCollider = newConnector.AddComponent<BoxCollider>();
                boxCollider.size = newConnector.transform.localScale;
                connectors.Add(newConnector);
            }

            startPosition += transform.forward;
        }

        if (target == null)
        {
            target = new GameObject("WormTarget", typeof(Transform));
            target.transform.parent = transform;
            target.transform.position = joints[joints.Count - 1].transform.position;
        }

        ik.target = target.transform;
        ik.Awake();
    }
}