using System.Collections;
using UnityEngine;

/// <summary>
/// Central audio manager handling BGM, sound effects, engine pitch modulation,
/// collision feedback, and UI sounds. Includes synthetic procedural fallbacks.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource engineSource;
    public AudioSource reverseSource;

    [Header("Audio Clips")]
    public AudioClip buttonClickClip;
    public AudioClip collisionClip;
    public AudioClip levelCompleteClip;
    public AudioClip levelFailedClip;
    public AudioClip parkingSuccessClip;
    public AudioClip countdownTickClip;
    public AudioClip brakeClip;
    public AudioClip engineClip;
    public AudioClip reverseClip;
    public AudioClip bgmClip;

    private bool soundEnabled = true;
    private bool musicEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSources();
            LoadClips();
            ApplySettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = 0.45f;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = 0.8f;
        }

        if (engineSource == null)
        {
            engineSource = gameObject.AddComponent<AudioSource>();
            engineSource.loop = true;
            engineSource.playOnAwake = false;
            engineSource.volume = 0.35f;
            engineSource.pitch = 0.9f;
        }

        if (reverseSource == null)
        {
            reverseSource = gameObject.AddComponent<AudioSource>();
            reverseSource.loop = true;
            reverseSource.playOnAwake = false;
            reverseSource.volume = 0.5f;
        }
    }

    private void LoadClips()
    {
        // Attempt loading from Resources or project path
        buttonClickClip = Resources.Load<AudioClip>("Audio/ButtonClick");
        collisionClip = Resources.Load<AudioClip>("Audio/Collision");
        levelCompleteClip = Resources.Load<AudioClip>("Audio/LevelComplete");
        levelFailedClip = Resources.Load<AudioClip>("Audio/LevelFailed");
        parkingSuccessClip = Resources.Load<AudioClip>("Audio/ParkingSuccess");
        countdownTickClip = Resources.Load<AudioClip>("Audio/CountdownTick");
        brakeClip = Resources.Load<AudioClip>("Audio/Brake");
        engineClip = Resources.Load<AudioClip>("Audio/EngineLoop");
        reverseClip = Resources.Load<AudioClip>("Audio/ReverseBeep");
        bgmClip = Resources.Load<AudioClip>("Audio/BackgroundMusic");

        // Generate synthetic fallbacks if any are null
        if (buttonClickClip == null) buttonClickClip = CreateSyntheticTone(950f, 0.08f);
        if (collisionClip == null) collisionClip = CreateSyntheticNoise(0.5f, 60f);
        if (levelCompleteClip == null) levelCompleteClip = CreateSyntheticChime(new float[] { 523f, 659f, 784f, 1046f }, 1.2f);
        if (levelFailedClip == null) levelFailedClip = CreateSyntheticTone(180f, 0.8f);
        if (parkingSuccessClip == null) parkingSuccessClip = CreateSyntheticChime(new float[] { 880f, 1320f }, 0.8f);
        if (countdownTickClip == null) countdownTickClip = CreateSyntheticTone(1200f, 0.05f);
        if (brakeClip == null) brakeClip = CreateSyntheticNoise(0.4f, 1500f);
        if (engineClip == null) engineClip = CreateSyntheticTone(60f, 1.0f);
        if (reverseClip == null) reverseClip = CreateSyntheticTone(1100f, 0.2f);
        if (bgmClip == null) bgmClip = CreateSyntheticChime(new float[] { 261f, 329f, 392f, 493f }, 3.0f);
    }

    private void Start()
    {
        PlayBGM();
    }

    public void ApplySettings()
    {
        soundEnabled = SaveManager.IsSoundEnabled();
        musicEnabled = SaveManager.IsMusicEnabled();

        if (musicSource != null) musicSource.mute = !musicEnabled;
        if (sfxSource != null) sfxSource.mute = !soundEnabled;
        if (engineSource != null) engineSource.mute = !soundEnabled;
        if (reverseSource != null) reverseSource.mute = !soundEnabled;
    }

    public void PlayBGM()
    {
        if (musicSource != null && bgmClip != null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.clip = bgmClip;
                musicSource.Play();
            }
        }
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlayCollision()
    {
        PlaySFX(collisionClip, 1.0f);
    }

    public void PlayLevelComplete()
    {
        PlaySFX(levelCompleteClip);
    }

    public void PlayLevelFailed()
    {
        PlaySFX(levelFailedClip);
    }

    public void PlayParkingSuccess()
    {
        PlaySFX(parkingSuccessClip);
    }

    public void PlayCountdownTick()
    {
        PlaySFX(countdownTickClip, 0.6f);
    }

    public void PlayBrakeSound()
    {
        PlaySFX(brakeClip, 0.5f);
    }

    public void StartEngineSound()
    {
        if (engineSource != null && engineClip != null && soundEnabled)
        {
            if (!engineSource.isPlaying)
            {
                engineSource.clip = engineClip;
                engineSource.Play();
            }
        }
    }

    public void StopEngineSound()
    {
        if (engineSource != null && engineSource.isPlaying)
        {
            engineSource.Stop();
        }
        SetReverseBeeping(false);
    }

    public void UpdateEnginePitch(float normalizedSpeed)
    {
        if (engineSource != null)
        {
            engineSource.pitch = Mathf.Lerp(0.85f, 1.8f, Mathf.Clamp01(normalizedSpeed));
        }
    }

    public void SetReverseBeeping(bool active)
    {
        if (reverseSource != null && reverseClip != null)
        {
            if (active && soundEnabled && !reverseSource.isPlaying)
            {
                reverseSource.clip = reverseClip;
                reverseSource.Play();
            }
            else if (!active && reverseSource.isPlaying)
            {
                reverseSource.Stop();
            }
        }
    }

    private void PlaySFX(AudioClip clip, float volumeScale = 1.0f)
    {
        if (soundEnabled && sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volumeScale);
        }
    }

    // ==========================================
    // PROCEDURAL AUDIO SYNTHESIZERS (FALLBACK)
    // ==========================================
    private AudioClip CreateSyntheticTone(float frequency, float duration)
    {
        int sampleRate = 22050;
        int sampleCount = Mathf.FloorToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float env = 1f - (t / duration);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * env * 0.5f;
        }
        AudioClip clip = AudioClip.Create("SyntheticTone_" + frequency, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateSyntheticNoise(float duration, float lowFreq)
    {
        int sampleRate = 22050;
        int sampleCount = Mathf.FloorToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float env = Mathf.Exp(-6f * t);
            float low = Mathf.Sin(2f * Mathf.PI * lowFreq * t) * 0.6f;
            float noise = (Random.value * 2f - 1f) * 0.4f;
            samples[i] = (low + noise) * env * 0.7f;
        }
        AudioClip clip = AudioClip.Create("SyntheticNoise", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateSyntheticChime(float[] frequencies, float duration)
    {
        int sampleRate = 22050;
        int sampleCount = Mathf.FloorToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        float stepTime = duration / frequencies.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            int step = Mathf.Min(frequencies.Length - 1, Mathf.FloorToInt(t / stepTime));
            float subT = t % stepTime;
            float env = Mathf.Exp(-4f * subT);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequencies[step] * t) * env * 0.6f;
        }
        AudioClip clip = AudioClip.Create("SyntheticChime", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
