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

    public void UpdateConnector()
    {

        transform.position = (startJoint.position + endJoint.position) / 2;


        transform.LookAt(endJoint);

      
        Vector3 scale = transform.localScale;
        scale.y = scaleY;
        scale.x = scaleX;
        scale.z = Vector3.Distance(startJoint.localPosition, endJoint.localPosition);
        transform.localScale = scale;
    }
}