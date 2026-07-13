using UnityEngine;

public class TowerTiltManager: MonoBehaviour
{
    [SerializeField] private float maxTiltAngle = 30f;   
    [SerializeField] private float checkDelay = 0.5f;    
    private bool gameOver;

    void Start()
    {
        InvokeRepeating(nameof(CheckTilt), checkDelay, 0.2f); // check on time
    }

    void CheckTilt()
    {
        if (gameOver) return;
        float angle = Vector3.Angle(Vector3.up, transform.up);
        if (angle > maxTiltAngle)
        {
            gameOver = true;
            StackManager sm = FindFirstObjectByType<StackManager>();
            if (sm != null) sm.GameOver();
        }
    }
}