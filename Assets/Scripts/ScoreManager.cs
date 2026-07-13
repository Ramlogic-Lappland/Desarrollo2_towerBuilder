using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int pointsPerBlock = 10;
    [SerializeField] private int perfectBonusBase = 5;
    [SerializeField] private float streakMultiplier = 0.5f;
    

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI towerHeightText;
    
    private int _score;
    private int _streak;
    private int _bestStreak;
    private int _blocksPlaced;
    private float _blockHeight = 1.0f;

    public int Score => _score;
    public int Streak => _streak;

    void Start()
    {
        UpdateUI();
    }

    public void AddPerfectDrop()
    {
        _streak++;
        int bonus = Mathf.FloorToInt(perfectBonusBase + (_streak * streakMultiplier * perfectBonusBase));
        _score += pointsPerBlock + bonus;
        Debug.Log($"Perfect! Streak: {_streak}, Bonus: {bonus}, Total: {_score}");
        UpdateUI();
    }

    public void AddNormalDrop()
    {
        _streak = 0;
        _score += pointsPerBlock;
        Debug.Log($"Normal drop. Streak reset. Score: {_score}");
        UpdateUI();
    }

    public void ResetScore()
    {
        _score = 0;
        _streak = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + _score.ToString();
        if (streakText != null)
            streakText.text = "Streak: " + _streak.ToString();
        if (towerHeightText != null)
            towerHeightText.text = "Height: " + GetTowerHeight().ToString("F1") + " m";
    }
    
    public void AddBlockPlaced()
    {
        _blocksPlaced++;
        UpdateUI();
    }
    public float GetTowerHeight()
    {
        return _blocksPlaced * _blockHeight;
    }
}
