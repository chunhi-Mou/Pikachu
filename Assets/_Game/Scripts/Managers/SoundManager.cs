using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.Serialization;

public enum SoundID
{
    BG_1 = 0,
}

public enum FxID
{
    Button, Shuffle, Hint, Victory, Fail,
    TileSelect, MatchFail, MatchSuccess,
    TimeUp, SwipeOn, SwipeOff
}

[System.Serializable]
public struct FxSound
{
    public FxID id;
    public AudioClip clip;
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup fxMixerGroup;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicClips;

    [Header("Sound Effects (FX)")]
    [SerializeField] private List<FxSound> fxList;
    [FormerlySerializedAs("fxPoolSize")] [SerializeField] private int fxSize = 10;

    private List<AudioSource> fxSources;
    private Dictionary<FxID, AudioClip> fxDictionary;
    private int currentFxSourceIndex = 0;

    #region Unity Events

    private void Awake()
    {
        InitMusicSource();
        InitFxDictionary();
        InitFxSources();
    }

    private void Start()
    {
        OnInitSettings();
        PlayMusic(0);
    }

    #endregion

    #region Init

    private void InitMusicSource()
    {
        musicSource.outputAudioMixerGroup = musicMixerGroup;
    }

    private void InitFxDictionary()
    {
        fxDictionary = new Dictionary<FxID, AudioClip>();
        foreach (var fx in fxList)
        {
            if (!fxDictionary.ContainsKey(fx.id))
            {
                fxDictionary.Add(fx.id, fx.clip);
            }
        }
    }

    private void InitFxSources()
    {
        fxSources = new List<AudioSource>();
        for (int i = 0; i < fxSize; i++)
        {
            GameObject fxObj = new GameObject($"FX_{i}");
            fxObj.transform.SetParent(this.transform);

            AudioSource source = fxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = fxMixerGroup;

            fxSources.Add(source);
        }
    }

    private void OnInitSettings()
    {
        bool isMusicOn = DataManager.Instance.GetSoundState(GameCONST.VOLUME_OF_MUSIC);
        ToggleAllMusic(isMusicOn);

        bool isFxOn = DataManager.Instance.GetSoundState(GameCONST.VOLUME_OF_FX);
        ToggleAllFx(isFxOn);
    }

    #endregion

    #region Public 

    public void PlayMusic(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= musicClips.Length) return;

        musicSource.clip = musicClips[clipIndex];
        musicSource.Play();
    }

    public void PlayFx(FxID id)
    {
        if (!fxDictionary.TryGetValue(id, out AudioClip clip))
        {
            Debug.LogWarning($"Missing FxID: {id}");
            return;
        }

        AudioSource source = fxSources[currentFxSourceIndex];
        source.PlayOneShot(clip);
        currentFxSourceIndex = (currentFxSourceIndex + 1) % fxSize;
    }

    public void ToggleAllFx(bool isOn)
    {
        SetVolume(fxMixerGroup.audioMixer, GameCONST.VOLUME_OF_FX, isOn);
        DataManager.Instance.SaveSoundState(GameCONST.VOLUME_OF_FX, isOn);
    }

    public void ToggleAllMusic(bool isOn)
    {
        SetVolume(musicMixerGroup.audioMixer, GameCONST.VOLUME_OF_MUSIC, isOn);
        DataManager.Instance.SaveSoundState(GameCONST.VOLUME_OF_MUSIC, isOn);
    }
    
    public bool GetMusicState()
    {
        return DataManager.Instance.GetSoundState(GameCONST.VOLUME_OF_MUSIC);
    }

    public bool GetFxState()
    {
        return DataManager.Instance.GetSoundState(GameCONST.VOLUME_OF_FX);
    }

    #endregion

    #region Helper

    private void SetVolume(AudioMixer mixer, string volumeParam, bool isOn)
    {
        // Volume 0 dB khi bật, -80 dB khi tắt (mute)
        float volume = isOn ? 0f : -80f;
        mixer.SetFloat(volumeParam, volume);
    }
    #endregion
}
