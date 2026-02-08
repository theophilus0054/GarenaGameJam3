using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject levelUI;
    [SerializeField] private Image blackScreen;
    [SerializeField] private TextMeshProUGUI loseHintText;

    [Header("Camera Win Move")]
    public Camera mainCam;
    public float camMoveHeight = 3f;
    public float camMoveDuration = 0.6f;
    public float camReturnDuration = 0.5f;

    [Header("Lose Fade")]
    public float loseFadeDuration = 1f;

    private Vector3 camStartPos;

    // Lose sound mapping
    Dictionary<LoseCause, SoundType> loseSoundMap;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mainCam == null)
            mainCam = Camera.main;

        camStartPos = mainCam.transform.position;

        // Map LoseCause -> SoundType
        loseSoundMap = new Dictionary<LoseCause, SoundType>()
        {
            { LoseCause.LoseStolen, SoundType.LoseStolen },
            { LoseCause.LoseCat, SoundType.LoseCat },
            { LoseCause.LosePlug, SoundType.LosePlug },
            { LoseCause.LoseBurned, SoundType.LoseBurned },
            { LoseCause.LoseBottle, SoundType.LoseBottle },
            { LoseCause.LoseBland, SoundType.LoseBland },
            { LoseCause.LoseRaw, SoundType.LoseRaw },
        };
    }

    // ================= GAME RESULT =================

    public void Win()
    {
        Time.timeScale = 0f;
        if (levelUI) levelUI.SetActive(false);

        StartCoroutine(WinSequence());
    }

    public void Lose(LoseCause cause = LoseCause.LoseRaw)
    {
        Time.timeScale = 0f;
        if (levelUI) levelUI.SetActive(false);

        SetLoseHint(cause);
        PlayLoseSFX(cause);

        StartCoroutine(LoseSequence());
    }

    // ================= WIN CINEMATIC =================
    IEnumerator WinSequence()
    {
        Vector3 targetPos = camStartPos + Vector3.up * camMoveHeight;

        yield return mainCam.transform.DOMove(targetPos, camMoveDuration)
            .SetUpdate(true)
            .WaitForCompletion();

        ShowWinIndicator();

        yield return mainCam.transform.DOMove(camStartPos, camReturnDuration)
            .SetUpdate(true)
            .WaitForCompletion();

        winUI.GetComponent<WinObjectAnimate>()?.Play();
    }

    // ================= LOSE CINEMATIC =================
    IEnumerator LoseSequence()
    {
        blackScreen.gameObject.SetActive(true);
        blackScreen.color = new Color(0, 0, 0, 0);

        yield return blackScreen.DOFade(1f, loseFadeDuration)
            .SetUpdate(true)
            .WaitForCompletion();

        ShowLoseIndicator();
    }

    // ================= UI CONTROL =================
    private void ShowWinIndicator()
    {
        if (winUI != null)
            winUI.SetActive(true);
    }

    private void ShowLoseIndicator()
    {
        if (loseUI != null)
            loseUI.SetActive(true);
    }

    // ================= LOSE TEXT =================
    void SetLoseHint(LoseCause cause)
    {
        if (!loseHintText) return;

        switch (cause)
        {
            case LoseCause.LoseStolen:
                loseHintText.text = "The fairy stole your spice!\nhint: put em back to its place";
                break;

            case LoseCause.LoseCat:
                loseHintText.text = "Mr. whiskers wants to play!\nhint: swipe several times ";
                break;

            case LoseCause.LosePlug:
                loseHintText.text = "Your house is on fire!\nhint: pull down the plug";
                break;

            case LoseCause.LoseBurned:
                loseHintText.text = "Your Steak is burned!\nhint: click to lower the temprature";
                break;

            case LoseCause.LoseBottle:
                loseHintText.text = "A bottle fell down!\nhint: hold to hold it still";
                break;

            case LoseCause.LoseBland:
                loseHintText.text = "Your steak is bland!\n(unforgivable)\nhint: spice up your steak";
                break;

            case LoseCause.LoseRaw:
            default:
                loseHintText.text = "Your steak is still raw!\nhint: Flip your steak (swipe up)";
                break;
        }
    }

    // ================= LOSE SFX =================
    void PlayLoseSFX(LoseCause cause)
    {
        if (!loseSoundMap.TryGetValue(cause, out var sound))
            sound = SoundType.LoseRaw; // fallback

        SoundManager.PlaySound(sound);
    }
}

// ================= ENUM LOSE CAUSE =================
public enum LoseCause
{
    LoseStolen,
    LoseCat,
    LosePlug,
    LoseBurned,
    LoseBottle,
    LoseBland,
    LoseRaw
}
