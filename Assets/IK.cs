using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IK : MonoBehaviour
{
    public List<Transform> joints = new List<Transform>();
    public Vector3 target;
    public Transform pole;
    public bool keepAnchorPosition = true;
    private bool pole_exists = false;
    public float epsilon_for_target = 0.1f;
    private int _endEffector;
    private int maxIterations = 10;
    private const int AnchorJoint = 0;
    [HideInInspector] public float totalBoneLength = 0;
    private List<float> bone_lengths = new List<float>();

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
            bone_lengths.Add(Vector3.Distance(joints[i].position, joints[i + 1].position));
        }

        if (pole != null) pole_exists = true;
        totalBoneLength = bone_lengths.Sum();
    }

    void FixedUpdate()
    {
        float dist_to_target = Vector3.Distance(target, joints[AnchorJoint].position);
        if (dist_to_target > totalBoneLength && keepAnchorPosition)
        {
            for (int i = AnchorJoint + 1; i <= _endEffector; i++)
            {
                Vector3 direction = (target - joints[AnchorJoint].position).normalized;
                joints[i].position = joints[i - 1].position + direction * bone_lengths[i - 1];
            }

            return;
        }
        else
        {
            float distance_last_joint_target = Vector3.Distance(joints[_endEffector].position, target);
            int iterations = 0;
            Vector3 original_anchor_position = joints[AnchorJoint].position;
            while (iterations < maxIterations && distance_last_joint_target > epsilon_for_target)
            {
                iterations += 1;
                joints[_endEffector].position = target;
                for (int i = _endEffector - 1; i >= AnchorJoint; i--)
                {
                    Vector3 norm_adjustment = (joints[i].position - joints[i + 1].position).normalized;
                    joints[i].position = joints[i + 1].position +
                                         norm_adjustment *
                                         bone_lengths[i];
                }

                if (pole_exists)
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
                    joints[AnchorJoint].position = original_anchor_position;
                    for (int i = AnchorJoint + 1; i <= _endEffector; i++)
                    {
                        joints[i].position = joints[i - 1].position +
                                             (joints[i].position - joints[i - 1].position).normalized *
                                             bone_lengths[i - 1];
                    }
                }

                distance_last_joint_target = Vector3.Distance(joints[_endEffector].position, target);
            }
        }
    }
}