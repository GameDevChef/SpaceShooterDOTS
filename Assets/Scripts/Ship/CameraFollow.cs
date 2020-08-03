using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{    
    public float rotateSpeed = 90.0f;
    public float moveSpeed = 90.0f;

    public Transform target;
    private Vector3 startOffset;
    private Vector3 velocity;

    private void Start()
    {
        startOffset = transform.position - target.position;
    }

    private void LateUpdate()
    {
            UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (target != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.TransformPoint(startOffset), ref velocity, moveSpeed * Time.deltaTime);
            var targetRoation = Quaternion.Slerp(transform.rotation, target.rotation, rotateSpeed * Time.deltaTime);
            transform.rotation = targetRoation;
        }
    }
}
