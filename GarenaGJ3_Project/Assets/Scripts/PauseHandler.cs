using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public GameObject pauseMenu;

    void Awake()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    public void TogglePause()
    {
        if (pauseMenu != null)
        {
            bool isActive = pauseMenu.activeSelf;
            pauseMenu.SetActive(!isActive);
            Time.timeScale = isActive ? 1 : 0;
        }
    }

    void OnMouseDown()
    {
        TogglePause();
    }
}
