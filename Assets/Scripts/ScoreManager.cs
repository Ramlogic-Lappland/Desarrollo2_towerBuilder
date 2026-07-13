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

    private int score;
    private int streak;
    private int bestStreak;

    public int Score => score;
    public int Streak => streak;

    void Start()
    {
        UpdateUI();
    }

    public void AddPerfectDrop()
    {
        streak++;
        int bonus = Mathf.FloorToInt(perfectBonusBase + (streak * streakMultiplier * perfectBonusBase));
        score += pointsPerBlock + bonus;
        Debug.Log($"Perfect! Streak: {streak}, Bonus: {bonus}, Total: {score}");
        UpdateUI();
    }

    public void AddNormalDrop()
    {
        streak = 0;
        score += pointsPerBlock;
        Debug.Log($"Normal drop. Streak reset. Score: {score}");
        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0;
        streak = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
        if (streakText != null)
            streakText.text = "Streak: " + streak.ToString();
    }
}
