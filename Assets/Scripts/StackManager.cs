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
    [SerializeField] private PauseMenu pauseMenu; 
    
    [Header("Stack Settings")]
    [SerializeField] private float blockHeight = 0.5f;
    [SerializeField] private float perfectTolerance = 0.02f;
    [SerializeField] private float craneHeight = 10f;

    [Header("Physics")]
    [SerializeField] private float stabilizationDelay = 0.1f; // freeze when snap
    
    [Header("UI")]
    [SerializeField] private GameOverMenu gameOverMenu; 

    private MyInputSystem _controls;
    private Block _previousBlock;
    private GameObject _currentMovingBlock;
    private Rigidbody _currentRB;
    private bool _isDropping = false;
    
    private static bool _isGameOver = false;  
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _isGameOver = false;

        _previousBlock = FindFirstObjectByType<Block>();
        if (_previousBlock == null)
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
    scoreManager.AddBlockPlaced(); 
}

    public void GameOver()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        if (pauseMenu != null)
            pauseMenu.OnGameOver();

        Debug.Log("Game Over – tower collapsed!");

        // Disable input
        if (_controls != null)
        {
            _controls.Player.Drop.performed -= OnDropPerformed;
            _controls.Player.Disable();
        }
        
        this.enabled = false;
        
        if (_currentMovingBlock != null)
        {
            Destroy(_currentMovingBlock);
            _currentMovingBlock = null;
        }

        _isDropping = false;

        // Show the game‑over UI
        if (gameOverMenu != null)
            gameOverMenu.EnableGameOverScreen();

        // Unlock cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // crumble tower
        Rigidbody[] allRbs = stackParent.GetComponentsInChildren<Rigidbody>(); 
        foreach (Rigidbody rb in allRbs)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
    
    private System.Collections.IEnumerator StabilizeBlock(Rigidbody rb)
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(stabilizationDelay);
        rb.isKinematic = false;
    }

}
