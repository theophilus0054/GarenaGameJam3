using UnityEngine;

public class SpriteButton : MonoBehaviour
{
    public enum ButtonType
    {
        Play,
        Settings,
        Quit,
        Stage,
        CloseStageSelect
    }

    public ButtonType buttonType;
    public int stageNumber = 1;

    void OnMouseDown()
    {
        if (MainMenuManager.Instance == null) return;

        bool physicsActive = MainMenuManager.Instance.physicsModeActive;

        // 🚫 BLOCK everything except Play & CloseStageSelect
        if (physicsActive &&
            buttonType != ButtonType.Play &&
            buttonType != ButtonType.CloseStageSelect)
        {
            return;
        }

        switch (buttonType)
        {
            case ButtonType.Play:
                MainMenuManager.Instance.OpenStageSelect();
                break;

            case ButtonType.Settings:
                MainMenuManager.Instance.ToggleSettings();
                break;

            case ButtonType.Quit:
                MainMenuManager.Instance.QuitGame();
                break;

            case ButtonType.Stage:
                MainMenuManager.Instance.StartGame(stageNumber);
                break;

            case ButtonType.CloseStageSelect:
                MainMenuManager.Instance.CloseStageSelect();
                break;
        }
    }
}
