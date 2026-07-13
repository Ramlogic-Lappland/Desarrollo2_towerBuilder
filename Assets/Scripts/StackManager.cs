using UnityEngine;

public class StackManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform stackParent; 
    //[SerializeField] private CameraFollow cameraToFollow;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Stack Settings")]
    [SerializeField] private float blockHeight = 0.5f;
    [SerializeField] private float perfectTolerance = 0.02f;
    [SerializeField] private Vector3 startPosition = new Vector3(0, 0.25f, 0);

    private Block _previousBlock;
    private GameObject _currentMovingBlock;
    private Rigidbody _currentRB;
    private bool _isDropping = false;
    
    void Start()
    {
        _previousBlock = FindObjectOfType<Block>();
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
        Vector3 spawnPos = _previousBlock.Position + Vector3.up * (blockHeight);
        _currentMovingBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        _currentRB = _currentMovingBlock.GetComponent<Rigidbody>();
        _currentRB.isKinematic = true;
        
        _currentMovingBlock.GetComponent<Block>().SetWidth(_previousBlock.Width);
        
        CraneArmController movement = _currentMovingBlock.AddComponent<CraneArmController>();
        movement.enabled = true;
        
        if (cameraFollow != null)
            cameraFollow.SetTarget(_previousBlock.Position);
    }

    void Update()
    {
        if (_isDropping) return;

        
        if (Input.GetMouseButtonDown(0)) // Left Click 
        {
            DropBlock();
        }
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
        
        if (landedBlock.gameObject != _currentMovingBlock) return; // Ignore if already processed
        
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
            newBlock.SetWidth(_previousBlock.Width);
            newBlock.Freeze();
            newBlock.transform.SetParent(stackParent);
            
            scoreManager.AddPerfectDrop();
        }
        else
        {
            Destroy(newBlock.gameObject);

            // Center the block
            Vector3 centerPos = _previousBlock.Position + Vector3.up * blockHeight;
            centerPos.x = _previousBlock.Position.x + deltaX / 2f;
            GameObject centerObj = Instantiate(blockPrefab, centerPos, Quaternion.identity, stackParent);
            Block centerBlock = centerObj.GetComponent<Block>();
            centerBlock.SetWidth(overlap);
            centerBlock.Freeze();
            
            float leftOverhang = Mathf.Max(0, -deltaX); // Left overhang
            if (leftOverhang > 0)
            {
                Vector3 leftPos = centerPos - new Vector3((overlap + leftOverhang) / 2f, 0, 0);
                GameObject leftObj = Instantiate(blockPrefab, leftPos, Quaternion.identity);
                leftObj.GetComponent<Block>().SetWidth(leftOverhang);
                leftObj.GetComponent<Block>().EnablePhysics();
                leftObj.transform.SetParent(null); // fall
            }
            
            float rightOverhang = Mathf.Max(0, deltaX); // Right overhang
            if (rightOverhang > 0)
            {
                Vector3 rightPos = centerPos + new Vector3((overlap + rightOverhang) / 2f, 0, 0);
                GameObject rightObj = Instantiate(blockPrefab, rightPos, Quaternion.identity);
                rightObj.GetComponent<Block>().SetWidth(rightOverhang);
                rightObj.GetComponent<Block>().EnablePhysics();
                rightObj.transform.SetParent(null); // fall
            }
            
            scoreManager.AddNormalDrop();
        }
        
        _previousBlock = centerBlock; 
        _isDropping = false;
        
        if (cameraFollow != null)
            cameraFollow.SetTarget(_previousBlock.Position);
        
        SpawnMovingBlock();
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        // TODO: Stop everything, show UI, etc.
    }

}
