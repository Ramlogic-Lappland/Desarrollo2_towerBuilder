using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
     [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameOverMenu gameOverMenu;

    [Header("Input")]
    [SerializeField] private MyInputSystem pauseAction;

    private bool _isPaused = false;
    private bool _isGameOver = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.Player.Pause.performed += OnPausePerformed;
    }

    void OnDisable()
    {
        pauseAction.Player.Pause.performed -= OnPausePerformed;
        pauseAction.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (_isGameOver) return;
        _isPaused = !_isPaused;

        if (_isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isPaused = false;
    }
    
    public void OnResumeButton()
    {
        TogglePause();
    }
    
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/MainMenu");
    }
    
    public void OnGameOver()
    {
        _isGameOver = true;
        if (pausePanel != null) pausePanel.SetActive(false);
    }
}
