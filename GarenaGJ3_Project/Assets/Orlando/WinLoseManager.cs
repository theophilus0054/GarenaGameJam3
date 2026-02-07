using UnityEngine;
using UnityEngine.UI;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    [Header("UI")]
    [SerializeField] private Image winUI;
    [SerializeField] private Image loseUI;

    void Awake()
    {
        // Singleton logic
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // Uncomment if UI persists across scenes
    }

    // ================= GAME RESULT =================

    public void Win()
    {
        ShowWinIndicator();
    }

    public void Lose()
    {
        ShowLoseIndicator();
    }

    // ================= UI CONTROL =================

    private void ShowWinIndicator()
    {
        if (winUI != null)
            winUI.gameObject.SetActive(true);
    }

    void HideWinIndicator()
    {
        if (winUI != null)
            winUI.gameObject.SetActive(false);
    }

    private void ShowLoseIndicator()
    {
        if (loseUI != null)
            loseUI.gameObject.SetActive(true);
    }

    void HideLoseIndicator()
    {
        if (loseUI != null)
            loseUI.gameObject.SetActive(false);
    }
}
