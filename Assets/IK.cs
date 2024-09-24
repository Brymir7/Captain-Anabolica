using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IK : MonoBehaviour
{
    public List<Transform> joints = new List<Transform>();
    public Transform target;
    public float epsilon_for_target = 0.1f;
    void FixedUpdate()
    {
        if (joints.Count <= 0)
        {
            Debug.LogError("IK without joints");
            return;
        }
        float dist_to_target = Vector3.Distance(target.position, joints[0].position);
        List<float> joint_lengths = new List<float>();
        for (int i = 0; i < joints.Count - 1; i++)
        {
            joint_lengths.Add(Vector3.Distance(joints[i].position, joints[i + 1].position));
        }

        if (dist_to_target > joint_lengths.Sum())
        {
            Debug.Log("Cannot reach target position with joint length" + joint_lengths.Sum());
            Debug.Log("Distance: " + dist_to_target);
            for (int i = 0; i < joints.Count - 1; i++)
            {
                float dist_target_joint = Vector3.Distance(target.position, joints[i].position);
                float nxt_joint_dist_rel_target = joint_lengths[i] / dist_target_joint;

                joints[i + 1].position = (1 - nxt_joint_dist_rel_target) * joints[i].position +
                                         nxt_joint_dist_rel_target * target.position;
            }
            return;
        }
        else
        {
            Debug.Log("Can reach target position");
            Vector3 anchor_joint = joints[0].position;
            float distance_last_joint_target = Vector3.Distance(joints[joints.Count - 1].position, target.position);
            while (distance_last_joint_target > epsilon_for_target)
            {
                {
                    joints[joints.Count - 1].position = target.position;
                    for (int i = joints.Count - 2; i >= 0; i--)
                    {   
                        float dist_to_next_joint = Vector3.Distance(joints[i].position, joints[i + 1].position);
                        float original_dist_rel_curr_d = joint_lengths[i] / dist_to_next_joint;
                        joints[i].position = (1-original_dist_rel_curr_d)*joints[i+1].position + original_dist_rel_curr_d*joints[i].position;
                    }

                    joints[0].position = anchor_joint;
                    for (int i = 0; i < joints.Count - 1; i++)
                    {
                        float dist_to_next_joint = Vector3.Distance(joints[i].position, joints[i + 1].position);
                        float original_dist_rel_curr_d = joint_lengths[i] / dist_to_next_joint;
                        joints[i+1].position = (1-original_dist_rel_curr_d)*joints[i].position + original_dist_rel_curr_d*joints[i+1].position;
                    }
                    distance_last_joint_target = Vector3.Distance(joints[joints.Count - 1].position, target.position);
                }
            }
        }
    }
}