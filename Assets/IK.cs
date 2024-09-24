using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IK : MonoBehaviour
{
    public List<Transform> joints = new List<Transform>();
    public Transform target;
    public float epsilon_for_target = 0.1f;
    private int _endEffector ;
    private const int AnchorJoint = 0;
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
            bone_lengths.Add(Vector3.Distance(joints[i].position, joints[i + 1].position));
        }
    }
    void FixedUpdate()
    {
        float dist_to_target = Vector3.Distance(target.position, joints[AnchorJoint].position);
        if (dist_to_target > bone_lengths.Sum())
        {
            Debug.Log("Cannot reach target position with joint length" + bone_lengths.Sum());
            Debug.Log("Distance: " + dist_to_target);
            for (int i = AnchorJoint + 1; i < _endEffector; i++)
            {
                Vector3 direction = (target.position - joints[AnchorJoint].position).normalized;
                joints[i].position = joints[i - 1].position + direction * bone_lengths[i - 1];
            }

            return;
        }
        else
        {
            Debug.Log("Can reach target position");
            float distance_last_joint_target = Vector3.Distance(joints[_endEffector].position, target.position);
            while (distance_last_joint_target > epsilon_for_target)
            {
                {
                    joints[_endEffector].position = target.position;
                    for (int i = _endEffector - 1; i > AnchorJoint; i--)
                    {   
                        joints[i].position = joints[i+1].position + (joints[i].position - joints[i+1].position).normalized * bone_lengths[i];
                    }
                    for (int i = AnchorJoint + 1; i < _endEffector - 1; i++)
                    {   
                        joints[i].position = joints[i-1].position + (joints[i].position - joints[i-1].position).normalized * bone_lengths[i-1];
                    }
                    distance_last_joint_target = Vector3.Distance(joints[_endEffector].position, target.position);
                }
            }
        }

    }
}