using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string volumeParameter = "MasterVolume";
    [SerializeField] private Slider slider;

    private const float MIN_DB = -80f;

    void Start()
    {
        if (!slider)
            slider = GetComponent<Slider>();

        // Load saved volume
        float savedValue = PlayerPrefs.GetFloat(volumeParameter, 1f);
        slider.value = savedValue;

        SetVolume(savedValue);
        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        // Convert linear (0–1) to decibel
        float dB = value > 0
            ? Mathf.Log10(value) * 20f
            : MIN_DB;

        audioMixer.SetFloat(volumeParameter, dB);

        PlayerPrefs.SetFloat(volumeParameter, value);
    }
}
