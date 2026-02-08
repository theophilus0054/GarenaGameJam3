using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameDifficulty
{
    EASY,
    MEDIUM,
    HARD,
    NIGHTMARE
}

public class Difficulty : MonoBehaviour
{
    public static Difficulty Instance;

    public GameDifficulty currentDifficulty = GameDifficulty.EASY;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button startButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        increaseButton.onClick.AddListener(IncreaseDifficulty);
        decreaseButton.onClick.AddListener(DecreaseDifficulty);
        startButton.onClick.AddListener(StartGame);

        UpdateText();
    }

    void UpdateText()
    {
        difficultyText.text = currentDifficulty.ToString();
    }

    void IncreaseDifficulty()
    {
        int max = System.Enum.GetValues(typeof(GameDifficulty)).Length;
        currentDifficulty = (GameDifficulty)(((int)currentDifficulty + 1) % max);

        Debug.Log("Difficulty: " + currentDifficulty);
        UpdateText();
    }

    void DecreaseDifficulty()
    {
        int max = System.Enum.GetValues(typeof(GameDifficulty)).Length;
        currentDifficulty = (GameDifficulty)(((int)currentDifficulty - 1 + max) % max);

        Debug.Log("Difficulty: " + currentDifficulty);
        UpdateText();
    }

    // ================= START GAME BASED ON DIFFICULTY =================
    public void StartGame()
    {
        string sceneName = GetSceneByDifficulty();

        Debug.Log("Loading Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    string GetSceneByDifficulty()
    {
        switch (currentDifficulty)
        {
            case GameDifficulty.EASY:
                return "EasySteakLevel";

            case GameDifficulty.MEDIUM:
                return "MediumSteakLevel";

            case GameDifficulty.HARD:
                return "HardSteakLevel";

            case GameDifficulty.NIGHTMARE:
                return "NightmareSteakLevel";

            default:
                return "EasySteakLevel";
        }
    }
}
