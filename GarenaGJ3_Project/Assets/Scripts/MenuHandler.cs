using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public Button retryButton;
    public Button mainMenuButton;
    public Button resumeButton;
    public GameObject pauseMenuUI;

    private void Awake()
    {
        // Assign button click listeners
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryButtonClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
    }

    private void OnRetryButtonClicked()
    {
        Time.timeScale = 1f;
        // Restart the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    private void OnResumeButtonClicked()
    {
        Time.timeScale = 1f;
        // Hide the pause menu
        pauseMenuUI.SetActive(false);
    }

}
