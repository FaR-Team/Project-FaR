using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private float fadeTime = 3f;
    

    [SerializeField] private OptionsMenu optionsMenu;
    
    private float currentMusicVolume; 
    private bool fading;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SleepHandler.Instance.OnPlayerWakeUp += PlayMusic;
        
        if (optionsMenu == null)
        {
            optionsMenu = FindObjectOfType<OptionsMenu>();
        }
        
        ApplyMixerVolumes();
        
        currentMusicVolume = optionsMenu.musicVolumeSlider.value;
        
        PlayMusic();
    }
    
    public void ApplyMixerVolumes()
    {
        if (optionsMenu == null) return;
        
        audioMixer.SetFloat("MasterVolume", ConvertToDecibel(optionsMenu.masterVolumeSlider.value));
        audioMixer.SetFloat("MusicVolume", ConvertToDecibel(optionsMenu.musicVolumeSlider.value));
        audioMixer.SetFloat("SFXVolume", ConvertToDecibel(optionsMenu.sfxVolumeSlider.value));
        audioMixer.SetFloat("UIVolume", ConvertToDecibel(optionsMenu.uiVolumeSlider.value));
    }
    
    private float ConvertToDecibel(float sliderValue)
    {
        if (sliderValue <= 0.0001f)
            return -80f;
            
        return Mathf.Log10(sliderValue) * 20f;
    }
    
    public void SetVolume(string parameterName, float value)
    {
        if (fading && parameterName == "MusicVolume") return;
        
        audioMixer.SetFloat(parameterName, ConvertToDecibel(value));
        
        if (parameterName == "MusicVolume")
        {
            currentMusicVolume = value;
        }
    }
    
    public void PlayMusic()
    {
        int rand = UnityEngine.Random.Range(0, songs.Length);
        
        // Only change if we have songs available
        if (songs.Length > 0)
        {
            musicAudioSource.clip = songs[rand];
            StartCoroutine(FadeIn(musicAudioSource, fadeTime));
        }
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1.0f, float pitch = 1.0f)
    {
        if (clip == null || sfxAudioSource == null) return;
        
        sfxAudioSource.pitch = pitch;
        sfxAudioSource.PlayOneShot(clip, volumeScale);
        
        sfxAudioSource.pitch = 1.0f;
    }

    public void PlaySFX(AudioClip clip, float volumeScale, float minPitch, float maxPitch)
    {
        if (clip == null || sfxAudioSource == null) return;
        
        float randomPitch = UnityEngine.Random.Range(minPitch, maxPitch);
        PlaySFX(clip, volumeScale, randomPitch);
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(musicAudioSource, fadeTime));
    }
    
    public IEnumerator FadeIn(AudioSource source, float time)
    {
        float targetVolume = currentMusicVolume;
        
        source.Play();
        audioMixer.SetFloat("MusicVolume", -80f);
        
        float elapsedTime = 0f;
        fading = true;
        while (elapsedTime < time)
        {
            float t = elapsedTime / time;
            float volume = Mathf.Lerp(0.0001f, targetVolume, t);
            audioMixer.SetFloat("MusicVolume", ConvertToDecibel(volume));
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
 
        audioMixer.SetFloat("MusicVolume", ConvertToDecibel(targetVolume));
        fading = false;
    }
    
    public IEnumerator FadeOut(AudioSource source, float time)
    {
        float startVolume = currentMusicVolume;
        
        float elapsedTime = 0f;
        fading = true;
        while (elapsedTime < time)
        {
            float t = elapsedTime / time;
            float volume = Mathf.Lerp(startVolume, 0.0001f, t);
            audioMixer.SetFloat("MusicVolume", ConvertToDecibel(volume));
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        source.Stop();
        
        audioMixer.SetFloat("MusicVolume", ConvertToDecibel(startVolume));
        fading = false;
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        currentMusicVolume = value;
        if (!fading)
        {
            SetVolume("MusicVolume", value);
        }
    }
    
    public float GetCurrentVolume(string parameterName)
    {
        float value;
        audioMixer.GetFloat(parameterName, out value);
        return value;
    }
    
    private void OnDestroy()
    {
        if (SleepHandler.Instance != null)
        {
            SleepHandler.Instance.OnPlayerWakeUp -= PlayMusic;
        }
    }
}
