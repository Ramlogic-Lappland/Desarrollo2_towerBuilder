using UnityEngine;

public class Block : MonoBehaviour
{
    private float _width;
    private Rigidbody _rb;

    [SerializeField] private float baseWidth = 1f;
    private Vector3 _baseColliderSize;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            baseWidth = mf.sharedMesh.bounds.size.x;

        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
            _baseColliderSize = col.size;
        
        _width = transform.localScale.x * baseWidth;
    }

    public float Width => _width;
    public Vector3 Position => transform.position;

    public void SetWidth(float newWidth)
    {
        _width = newWidth;
        float scaleX = newWidth / baseWidth;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            col.size = new Vector3(_baseColliderSize.x * scaleX, _baseColliderSize.y, _baseColliderSize.z);
        }
    }
    

    public void Freeze()
    {
        _rb.isKinematic = true;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void EnablePhysics()
    {
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.None;
    }

    public void SnapTo(Vector3 targetPosition)
    {
        _rb.MovePosition(targetPosition);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        SoundManager.instance.PLaySound3D("bonk", transform.position);
        if (collision.gameObject.CompareTag("Ground"))
        {
            StackManager stackManager = FindFirstObjectByType<StackManager>();
            if (stackManager != null)
            {
                stackManager.GameOver();
                return;
            }
        }
        
        if (collision.gameObject.CompareTag("StackBlock"))
        {
            StackManager stackManager = FindFirstObjectByType<StackManager>();
            if (stackManager != null)
                stackManager.OnBlockLanded(GetComponent<Block>());
        }
    }
}
