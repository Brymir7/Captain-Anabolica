using UnityEngine;

public class Connector : MonoBehaviour
{
    public Transform startJoint;
    public Transform endJoint;
    public float scaleX = 0.1f;
    public float scaleY = 0.1f;
    void Update()
    {
        if (startJoint != null && endJoint != null)
        {
            UpdateConnector();
        }
    }

    private void UpdateConnector()
    {
        // Set position to the midpoint between the joints
        transform.position = (startJoint.position + endJoint.position) / 2;

        // Make the connector look at the end joint
        transform.LookAt(endJoint);

        // Adjust the scale of the connector based on the distance between joints
        Vector3 scale = transform.localScale;
        scale.y = scaleY;
        scale.x = scaleX;
        scale.z = Vector3.Distance(startJoint.localPosition, endJoint.localPosition);
        transform.localScale = scale;
    }
}