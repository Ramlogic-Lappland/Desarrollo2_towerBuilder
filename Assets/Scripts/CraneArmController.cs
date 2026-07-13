using UnityEngine;

public class CraneArmController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveRange = 3f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 startPosition;                      

    void OnEnable()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float xOffset = Mathf.PingPong(Time.time * moveSpeed, moveRange * 2) - moveRange;
        Vector3 newPos = startPosition + new Vector3(xOffset, 0, 0);
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
    }
}
