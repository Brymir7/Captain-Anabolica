using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Enemies
{
    [System.Serializable]
    public class LegIKPair
    {
        public Transform leg;
        public Vector3 offsetFromBody;

        [HideInInspector] public IK ikComponent; // Store the IK component
    }

    public class SpiderIK : MonoBehaviour
    {
        public float maxDistanceOfLegTillSnapback;
        [SerializeField] private List<LegIKPair> legIKPairs;

        void Start()
        {
            foreach (var pair in legIKPairs)
            {
                pair.ikComponent = pair.leg.gameObject.GetComponent<IK>();
                if (pair.ikComponent != null)
                {
                    if (pair.ikComponent.joints.Count > 0)
                    {
                        Assert.IsTrue(pair.offsetFromBody.y == 0,
                            "LegIKPair.offsetFromBody.y == 0, as its gonna be raycast to the ground");
                        Transform lastJoint = pair.ikComponent.joints[pair.ikComponent.joints.Count - 1];
                        GameObject targetClone = new GameObject($"{pair.leg.name}_Target");
                        targetClone.transform.parent = transform;
                        targetClone.transform.position = lastJoint.position + pair.offsetFromBody;
                        pair.ikComponent.target = targetClone.transform.position;
                    }
                    else
                    {
                        Debug.LogError($"IK component on {pair.leg.name} does not have any joints assigned.");
                    }
                }
                else
                {
                    Debug.LogError($"Leg {pair.leg.name} is missing an IK component.");
                }
            }
        }

        void Update()
        {
            foreach (var pair in legIKPairs)
            {
                var goalTargetXZ = transform.position + pair.offsetFromBody + Cmul(
                    (pair.ikComponent.joints[0].position - transform.position).normalized,
                    transform.lossyScale);
                Ray ray = new Ray(goalTargetXZ, Vector3.down);
                RaycastHit hit;
                Vector3 targetPosition;
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Ground"))
                {
                    targetPosition = hit.point;
                }
                else
                {
                    targetPosition = ray.GetPoint(pair.ikComponent.totalBoneLength);
                }

                if (Vector3.Distance(targetPosition, pair.ikComponent.target) > maxDistanceOfLegTillSnapback)
                {
                    pair.ikComponent.target = targetPosition;
                }
            }
        }

        Vector3 Cmul(Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}