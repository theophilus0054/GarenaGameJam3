using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public class MainData
{
    public int highestStageReached = 1;
    public int maxStage = 5;
}


public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    public bool physicsModeActive { get; private set; }

    [Header("Game Data")]
    public MainData mainData;

    [Header("UI Panels")]
    public GameObject stageSelectMenu;

    [Header("Physics Buttons")]
    public PhysicsButtonReset[] menuButtons;

    [Header("Settings")]
    public VolumeBarController volumeBar;

    private bool stageSelectOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }

    // ================= PLAY =================
    public void OpenStageSelect()
    {
        if (stageSelectOpen) return;
        if (stageSelectMenu == null) return;

        volumeBar.HideSettings();

        DOVirtual.DelayedCall(0.3f, () =>
        {
            physicsModeActive = true;

            foreach (var btn in menuButtons) {
                btn.EnablePhysics();
                btn.SetColor(Hex("#848484"));
            }
        });

        

        // Delay 0.6 detik sebelum set stageSelectOpen = true
        DOVirtual.DelayedCall(1.5f, () =>
        {
            stageSelectOpen = true;
            stageSelectMenu.SetActive(true);
        });
    }

    public void CloseStageSelect()
    {
        stageSelectOpen = false;
        stageSelectMenu.SetActive(false);
        physicsModeActive = false;

        foreach (var btn in menuButtons) {
            btn.DisablePhysicsAndReset();
            btn.SetColor(Hex("#FFFFFF"));
        }
    }

    // ================= SETTINGS =================
    public void ToggleSettings()
    {
        volumeBar.Toggle();
    }

    // ================= GAME =================
    public void StartGame()
    {
        SceneManager.LoadScene("SteakLevel");
        Debug.Log("Starting Game...");
    }

    public void StartGame(int stage)
    {
        if (stage < 1 || stage > mainData.maxStage)
        {
            Debug.LogError("Invalid stage!");
            return;
        }

        if (stage > mainData.highestStageReached)
        {
            Debug.LogError("Stage locked!");
            return;
        }

        SceneManager.LoadScene($"Level{stage}");
    }

    // ================= QUIT =================
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
