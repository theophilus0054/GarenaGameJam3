using UnityEngine;
using TMPro;
using System.Collections;
using Random = System.Random;

public class TipHandler : MonoBehaviour
{
    TextMeshProUGUI tipText;

    private string[] msg = {
        "Beware of the spice-stealing fairies!",
        "Pull the plug if there’s a short circuit from the socket!",
        "Swipe several times to drive the cat away!",
        "Try to maintain the stove’s temperature!",
        "Keep the bottle from falling!",
        "Spice up your steak to win the game!",
        "Don’t forget to flip your steak!"
    };

    Random rand = new Random();
    //bool tipApplied = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tipText = this.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (tipText.isActiveAndEnabled && !tipApplied)
    //     {
    //         // Fade out over time or implement any other behavior if needed
    //         int value = rand.Next(0, msg.Length); // 0–msg.Length-1
    //         tipText.text = msg[value];
    //         tipApplied = true;
    //     }

    //     if (!tipText.isActiveAndEnabled && tipApplied)
    //     {
    //         tipApplied = false;
    //     }
    // }

    void OnEnable()
    {
        int value = UnityEngine.Random.Range(0, msg.Length);
        tipText.text = msg[value];
    }

}
