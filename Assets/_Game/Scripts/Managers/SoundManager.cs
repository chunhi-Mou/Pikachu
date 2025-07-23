using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public enum SoundID
{
    BG_1 = 0,
}

public enum FxID
{
    Button = 0,
    Shuffle = 1,
    Hint = 2,
    Victory = 3,
    Fail = 4,
    TileSelect = 5,
    MatchFail = 6,
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
    [SerializeField] private int fxPoolSize = 10;
    
    private List<AudioSource> fxSources;
    private Dictionary<FxID, AudioClip> fxDictionary;
    private int currentFxSourceIndex = 0;
    
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        musicSource.outputAudioMixerGroup = musicMixerGroup;

        fxDictionary = new Dictionary<FxID, AudioClip>();
        foreach (var fx in fxList)
        {
            if (!fxDictionary.ContainsKey(fx.id))
            {
                fxDictionary.Add(fx.id, fx.clip);
            }
        }
        
        fxSources = new List<AudioSource>();
        for (int i = 0; i < fxPoolSize; i++)
        {
            GameObject sourceObject = new GameObject("FX" + i);
            sourceObject.transform.SetParent(this.transform);
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
   
            source.outputAudioMixerGroup = fxMixerGroup;
            
            fxSources.Add(source);
        }
    }

    private void Start()
    {
        PlayMusic((int)SoundID.BG_1);
    }
    
    public void PlayMusic(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= musicClips.Length) return;

        musicSource.clip = musicClips[clipIndex];
        musicSource.Play();
    }
    
    public void PlayFx(FxID id)
    {
        if (fxDictionary.TryGetValue(id, out AudioClip clip))
        {
            AudioSource source = fxSources[currentFxSourceIndex];
            source.PlayOneShot(clip);
            currentFxSourceIndex = (currentFxSourceIndex + 1) % fxPoolSize;
        }
        else
        {
            Debug.LogWarning("Missing FxID: " + id);
        }
    }
}