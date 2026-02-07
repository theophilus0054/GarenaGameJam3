using DG.Tweening;
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
    public UISlideFromTop uiSlide;

    [Header("Physics Buttons")]
    public PhysicsButtonReset[] menuButtons;

    [Header("Sprite Renderers (Buttons with SR)")]
    public GameObject[] hoverSprites; // isi 3 button (atau lebih) yang punya SpriteRenderer

    [Header("Settings")]
    public VolumeBarController volumeBar;

    [Header("Button Colors")]
    [SerializeField] private string normalHex = "#FFFFFF";
    [SerializeField] private string physicsOnHex = "#848484";
    [SerializeField] private string hoverHex = "#FFD966";
    [SerializeField] private string clickHex = "#FF6B6B";

    private bool stageSelectOpen = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

        if (volumeBar != null)
            volumeBar.HideSettings();

        DOVirtual.DelayedCall(0.3f, () =>
        {
            physicsModeActive = true;

            if (menuButtons != null)
            {
                foreach (var btn in menuButtons)
                {
                    if (btn == null) continue;
                    btn.EnablePhysics();
                    btn.SetColor(Hex(physicsOnHex));
                }
            }

            // kalau mau sprite list ikut berubah saat physics on:
            SetSpritesColor(Hex(physicsOnHex));
        });

        DOVirtual.DelayedCall(1.5f, () =>
        {
            stageSelectOpen = true;
            if (stageSelectMenu != null) {
                stageSelectMenu.SetActive(true);
                uiSlide.PlayShowAnimation();
            }
        });
    }

    public void CloseStageSelect()
    {
        stageSelectOpen = false;

        if (stageSelectMenu != null)
            stageSelectMenu.SetActive(false);

        physicsModeActive = false;

        if (menuButtons != null)
        {
            foreach (var btn in menuButtons)
            {
                if (btn == null) continue;
                btn.DisablePhysicsAndReset();
                btn.SetColor(Hex(normalHex));
            }
        }

        SetSpritesColor(Hex(normalHex));
        uiSlide.PlayHideAnimation();
    }

    // ================= SETTINGS =================
    public void ToggleSettings()
    {
        if (volumeBar != null)
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
        if (mainData == null)
        {
            Debug.LogError("MainData is null!");
            return;
        }

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

    // ================= BUTTON FEEDBACK =================
    private void SetSpritesColor(Color c)
    {
        if (hoverSprites == null) return;

        foreach (var go in hoverSprites)
        {
            if (go == null) continue;

            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            sr.color = c;
        }
    }

    // Dipanggil dari script di button (OnMouseEnter)
    public void OnHover(GameObject btnGO)
    {
        SetSpritesColor(Hex(hoverHex));
    }

    // Dipanggil dari script di button (OnMouseExit)
    public void OnHoverExit(GameObject btnGO)
    {
        SetSpritesColor(physicsModeActive ? Hex(physicsOnHex) : Hex(normalHex));
    }

    // Dipanggil dari script di button (OnMouseDown)
    public void OnMouseDownBtn(GameObject btnGO)
    {
        SetSpritesColor(Hex(clickHex));

        DOVirtual.DelayedCall(0.08f, () =>
        {
            SetSpritesColor(Hex(hoverHex));
        });
    }
}