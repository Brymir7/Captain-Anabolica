using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class JointInfo
{
    public Transform joint;
    public Vector3 constraint;
}

public class IK : MonoBehaviour
{
    public List<JointInfo> joints = new List<JointInfo>();
    public Transform target;
    public Transform pole;
    private bool pole_exists = false;
    public float epsilon_for_target = 0.1f;
    private int _endEffector;
    private int maxIterations = 10;
    private const int AnchorJoint = 0;
    [HideInInspector] public float totalBoneLength = 0;
    private List<float> bone_lengths = new List<float>();

    void Awake()
    {
        if (joints.Count <= 0)
        {
            Debug.LogError("IK without joints");
            return;
        }

        _endEffector = joints.Count - 1;
        for (int i = AnchorJoint; i < _endEffector; i++)
        {
            bone_lengths.Add(Vector3.Distance(joints[i].joint.position, joints[i + 1].joint.position));
        }

        if (pole != null) pole_exists = true;
        totalBoneLength = bone_lengths.Sum();
    }

    void FixedUpdate()
    {
        float dist_to_target = Vector3.Distance(target.position, joints[AnchorJoint].joint.position);
        if (dist_to_target > totalBoneLength)
        {
            for (int i = AnchorJoint + 1; i <= _endEffector; i++)
            {
                Vector3 direction = (target.position - joints[AnchorJoint].joint.position).normalized;
                joints[i].joint.position = joints[i - 1].joint.position + direction * bone_lengths[i - 1];
            }

            return;
        }
        else
        {
            float distance_last_joint_target = Vector3.Distance(joints[_endEffector].joint.position, target.position);
            int iterations = 0;
            while (iterations < maxIterations && distance_last_joint_target > epsilon_for_target)
            {
                iterations += 1;
                joints[_endEffector].joint.position = target.position;
                for (int i = _endEffector - 1; i > AnchorJoint; i--)
                {
                    Vector3 norm_adjustment = (joints[i].joint.position - joints[i + 1].joint.position).normalized;
                    joints[i].joint.position = joints[i + 1].joint.position +
                                               norm_adjustment *
                                               bone_lengths[i];
                }

                if (pole_exists)
                {
                    for (int i = 1; i < _endEffector; i++)
                    {
                        Vector3 currJointPos = joints[i].joint.position;
                        Vector3 nextJointPos = joints[i + 1].joint.position;
                        Vector3 prevJointPos = joints[i - 1].joint.position;
                        var plane = new Plane(nextJointPos - prevJointPos,
                            prevJointPos);
                        var projectedPole = plane.ClosestPointOnPlane(pole.position);
                        var projectedBone = plane.ClosestPointOnPlane(joints[i].joint.position);
                        var angle = Vector3.SignedAngle(projectedBone - prevJointPos,
                            projectedPole - prevJointPos, plane.normal);
                        joints[i].joint.position =
                            Quaternion.AngleAxis(angle, plane.normal) * (joints[i].joint.position - prevJointPos) +
                            prevJointPos;
                    }
                }


                for (int i = AnchorJoint + 1; i <= _endEffector; i++)
                {
                    joints[i].joint.position = joints[i - 1].joint.position +
                                               (joints[i].joint.position - joints[i - 1].joint.position).normalized *
                                               bone_lengths[i - 1];
                }

                distance_last_joint_target = Vector3.Distance(joints[_endEffector].joint.position, target.position);
            }
        }
    }
}