using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameDifficulty
{
    EASY,
    MEDIUM,
    HARD,
    NIGHTMARE
}

public class Difficulty : MonoBehaviour
{
    public GameDifficulty currentDifficulty = GameDifficulty.EASY;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    public void SetDifficulty(string value)
    {
        difficultyText.text = value;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        increaseButton.onClick.AddListener(IncreaseDifficulty);
        decreaseButton.onClick.AddListener(DecreaseDifficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IncreaseDifficulty()
    {
        int max = System.Enum.GetValues(typeof(GameDifficulty)).Length;
        currentDifficulty = (GameDifficulty)(((int)currentDifficulty + 1) % max);

        Debug.Log("Difficulty: " + currentDifficulty);
        SetDifficulty(currentDifficulty.ToString());
    }

    void DecreaseDifficulty()
    {
        int max = System.Enum.GetValues(typeof(GameDifficulty)).Length;
        currentDifficulty = (GameDifficulty)(((int)currentDifficulty - 1 + max) % max);

        Debug.Log("Difficulty: " + currentDifficulty);
        SetDifficulty(currentDifficulty.ToString());
    }
}
