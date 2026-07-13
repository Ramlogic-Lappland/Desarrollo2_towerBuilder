using UnityEngine;
using UnityEngine.InputSystem;

public class StackManager : MonoBehaviour
{


    [Header("References")]
    [SerializeField] private Vector3 startPosition = new Vector3(0, 0.25f, 0);
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform stackParent;  
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Stack Settings")]
    [SerializeField] private float blockHeight = 0.5f;
    [SerializeField] private float perfectTolerance = 0.02f;
    [SerializeField] private float craneHeight = 10f;

    [Header("Physics")]
    [SerializeField] private float stabilizationDelay = 0.1f; // freeze when snap

    private MyInputSystem _controls;
    private Block _previousBlock;
    private GameObject _currentMovingBlock;
    private Rigidbody _currentRB;
    private bool _isDropping = false;
    
    void Start()
    {
        _previousBlock = FindFirstObjectByType<Block>();
        if (_previousBlock == null) //  TODO: Choose if Spawn a block at base position Or can add a black to the Scene
        {
            GameObject baseObj = Instantiate(blockPrefab, startPosition, Quaternion.identity, stackParent);
            Block baseBlock = baseObj.GetComponent<Block>();
            baseBlock.SetWidth(2f);
            baseBlock.Freeze();
            _previousBlock = baseBlock;
        }
        SpawnMovingBlock();
    }

    void SpawnMovingBlock()
    {
        // previous block pos
        Vector3 spawnPos = _previousBlock.Position + Vector3.up * (craneHeight);
        _currentMovingBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        _currentRB = _currentMovingBlock.GetComponent<Rigidbody>();
        _currentRB.isKinematic = true;
        
        _currentMovingBlock.GetComponent<Block>().SetWidth(_previousBlock.Width);
        
        CraneArmController movement = _currentMovingBlock.AddComponent<CraneArmController>();
        movement.enabled = true;
        
        if (cameraFollow != null)
            cameraFollow.SetTarget(_previousBlock.Position);
    }

    void Awake()
    {
        _controls = new MyInputSystem();
    }

    void OnEnable()
    {
        _controls.Player.Enable();
        _controls.Player.Drop.performed += OnDropPerformed;
    }

    void OnDisable()
    {
        _controls.Player.Drop.performed -= OnDropPerformed;
        _controls.Player.Disable();
    }
    
    private void OnDropPerformed(InputAction.CallbackContext context)
    {
        if (!_isDropping)
            DropBlock();
    }
    
    void DropBlock()
    {
        if (_currentMovingBlock == null) return;
        _isDropping = true;
        
        CraneArmController movement = _currentMovingBlock.GetComponent<CraneArmController>();
        if (movement != null) movement.enabled = false;
        
        _currentRB.isKinematic = false;
        _currentRB.linearVelocity = new Vector3(_currentRB.linearVelocity.x, -2f, 0);
    } 
    
public void OnBlockLanded(Block landedBlock)
{
    if (landedBlock.gameObject != _currentMovingBlock) return;

    Block newBlock = landedBlock.GetComponent<Block>();
    float deltaX = newBlock.Position.x - _previousBlock.Position.x;
    float overlap = _previousBlock.Width - Mathf.Abs(deltaX);

    if (overlap <= 0f)
    {
        GameOver();
        return;
    }

    bool perfect = Mathf.Abs(deltaX) < perfectTolerance;

    if (perfect)
    {
        Vector3 snapPos = _previousBlock.Position + Vector3.up * blockHeight;
        newBlock.SnapTo(snapPos);
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        StartCoroutine(StabilizeBlock(rb));
    }
    newBlock.transform.SetParent(stackParent);

    // Score
    if (perfect) scoreManager.AddPerfectDrop();
    else scoreManager.AddNormalDrop();
    
    _previousBlock = newBlock;
    _isDropping = false;

    if (cameraFollow != null)
        cameraFollow.SetTarget(_previousBlock.Position);

    SpawnMovingBlock();
}

    public void GameOver()
    {
        Debug.Log("Game Over – tower collapsed!");
        // TODO: Disable input, stop spawning, show UI, etc.
        _controls.Player.Drop.performed -= OnDropPerformed;
        this.enabled = false;
        
    }
    
    private System.Collections.IEnumerator StabilizeBlock(Rigidbody rb)
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(stabilizationDelay);
        rb.isKinematic = false;
    }

}
