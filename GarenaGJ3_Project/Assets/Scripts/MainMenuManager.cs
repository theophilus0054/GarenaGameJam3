using UnityEngine;
using UnityEngine.SceneManagement;

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

    // ================= PLAY =================
    public void OpenStageSelect()
    {
        if (stageSelectOpen) return;
        if (stageSelectMenu == null) return;

        if (volumeBar.isVolumeBarVisible)
            ToggleSettings();

        stageSelectOpen = true;
        stageSelectMenu.SetActive(true);
        physicsModeActive = true;

        foreach (var btn in menuButtons)
            btn.EnablePhysics();
    }

    public void CloseStageSelect()
    {
        stageSelectOpen = false;
        stageSelectMenu.SetActive(false);
        physicsModeActive = false;

        foreach (var btn in menuButtons)
            btn.DisablePhysicsAndReset();
    }

    // ================= SETTINGS =================
    public void ToggleSettings()
    {
        volumeBar.Toggle();
    }

    // ================= GAME =================
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
