using UnityEngine;

public class Block : MonoBehaviour
{
    private float _width;   
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _width = transform.localScale.x;
    }

    public float Width => _width;
    public Vector3 Position => transform.position;

    public void SetWidth(float newWidth)
    {
        _width = newWidth;
        Vector3 scale = transform.localScale;
        scale.x = newWidth;
        transform.localScale = scale;
           
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
            col.size = new Vector3(newWidth, scale.y, scale.z);
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
