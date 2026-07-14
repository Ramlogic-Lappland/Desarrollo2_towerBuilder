using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] private float smoothSpeed = 5f;

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    public void SetTarget(Vector3 stackTop)
    {
        targetPosition = stackTop + offset;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
