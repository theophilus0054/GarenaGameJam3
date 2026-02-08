using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundsSO soundsSO;
    [SerializeField] private AudioMixerGroup masterMixer;

    private AudioSource audioSource;

    Dictionary<SoundType, SoundList> soundDict;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        soundDict = new Dictionary<SoundType, SoundList>();
        foreach (var s in soundsSO.sounds)
            soundDict[s.type] = s;
    }


    public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1f)
    {
        if (!Instance.soundDict.TryGetValue(sound, out var data)) return;

        if (data.sounds.Length == 0) return;

        AudioClip clip = data.sounds[UnityEngine.Random.Range(0, data.sounds.Length)];
        float finalVolume = volume * data.volume;

        if (source != null)
            source.PlayOneShot(clip, finalVolume);
        else
            Instance.audioSource.PlayOneShot(clip, finalVolume);
    }

}


[Serializable]
public class SoundList
{
    public SoundType type;
    [Range(0,1)] public float volume = 1f;
    public AudioClip[] sounds;
}


public enum SoundType
{
    Hover,
    Click,
    Burnt,
    Sizzle,
    CatMeow,
    CatHiss,
    BottleWiggle,
    FairyPickup,
    DropItem,
    Seasoning,
    StoveTap,
    LightningSpark,
    Unplug,
    SparklingDone,
    Poof,

    LoseStolen,
    LoseCat,
    LosePlug,
    LoseBurned,
    LoseBottle,
    LoseBland,
    LoseRaw
}
