using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class IK : MonoBehaviour
{
    public List<Transform> joints;
    public Vector3 target;
    public Transform pole;
    public bool keepAnchorPosition;
    private bool _poleExists;
    private int _endEffector;
    private int _maxIterations = 10;
    private const int AnchorJoint = 0;
    private List<float> _boneLengths = new List<float>();
    [HideInInspector] public float totalBoneLength = 0;

    [FormerlySerializedAs("epsilon_for_target")]
    public float epsilonForTarget = 0.1f;


    public void RemoveLastJoint()
    {
        joints.RemoveAt(joints.Count - 1);
        _endEffector = joints.Count - 1;
    }

    public void Awake()
    {
        if (joints.Count <= 0)
        {
            Debug.Log("IK without joints");
        }

        _endEffector = joints.Count - 1;
        for (int i = AnchorJoint; i < _endEffector; i++)
        {
            _boneLengths.Add(Vector3.Distance(joints[i].position, joints[i + 1].position));
        }

        if (pole != null) _poleExists = true;
        totalBoneLength = _boneLengths.Sum();
    }

    void Update()
    {
        float distToTarget = Vector3.Distance(target, joints[AnchorJoint].position);
        if (distToTarget > totalBoneLength && keepAnchorPosition)
        {
            for (int i = AnchorJoint + 1; i <= _endEffector; i++)
            {
                Vector3 direction = (target - joints[AnchorJoint].position).normalized;
                joints[i].position = joints[i - 1].position + direction * _boneLengths[i - 1];
            }

            return;
        }
        else
        {
            float distanceLastJointTarget = Vector3.Distance(joints[_endEffector].position, target);
            int iterations = 0;
            Vector3 originalAnchorPosition = joints[AnchorJoint].position;
            while (iterations < _maxIterations && distanceLastJointTarget > epsilonForTarget)
            {
                iterations += 1;
                joints[_endEffector].position = target;
                for (int i = _endEffector - 1; i >= AnchorJoint; i--)
                {
                    Vector3 normAdjustment = (joints[i].position - joints[i + 1].position).normalized;
                    joints[i].position = joints[i + 1].position +
                                         normAdjustment *
                                         _boneLengths[i];
                }

                if (_poleExists)
                {
                    for (int i = 1; i < _endEffector; i++)
                    {
                        Vector3 currJointPos = joints[i].position;
                        Vector3 nextJointPos = joints[i + 1].position;
                        Vector3 prevJointPos = joints[i - 1].position;
                        var plane = new Plane(nextJointPos - prevJointPos,
                            prevJointPos);
                        var projectedPole = plane.ClosestPointOnPlane(pole.position);
                        var projectedBone = plane.ClosestPointOnPlane(joints[i].position);
                        var angle = Vector3.SignedAngle(projectedBone - prevJointPos,
                            projectedPole - prevJointPos, plane.normal);
                        joints[i].position =
                            Quaternion.AngleAxis(angle, plane.normal) * (joints[i].position - prevJointPos) +
                            prevJointPos;
                    }
                }

                if (keepAnchorPosition)
                {
                    joints[AnchorJoint].position = originalAnchorPosition;
                    for (int i = AnchorJoint + 1; i <= _endEffector; i++)
                    {
                        joints[i].position = joints[i - 1].position +
                                             (joints[i].position - joints[i - 1].position).normalized *
                                             _boneLengths[i - 1];
                    }
                }

                distanceLastJointTarget = Vector3.Distance(joints[_endEffector].position, target);
            }
        }
    }
}