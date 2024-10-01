using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private Slider slider;
    [SerializeField] private float fadeTime = 3f;
    private float currentVolume; // Current volume in settings
    private bool fading;

    private void Start()
    {
        SleepHandler.Instance.OnPlayerWakeUp += PlayMusic;
        slider.onValueChanged.AddListener(UpdateVolume);

        currentVolume = slider.value;
        PlayMusic();
    }

    public void UpdateVolume(float sliderValue)
    {
        currentVolume = sliderValue;
        if (fading) return;
        musicAudioMixer.SetFloat( "MusicVolume",Mathf.Log10(currentVolume) * 20);
    }
    
    public void PlayMusic()
    {
        int rand = Random.Range(0, songs.Length);

        musicAudioSource.clip = songs[rand];
        StartCoroutine(FadeIn(musicAudioSource, fadeTime));
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(musicAudioSource, fadeTime));
    }
    
    public IEnumerator FadeIn(AudioSource source, float time)
    {
        float targetVolume = currentVolume;
        
        
        source.Play();
        musicAudioMixer.SetFloat( "MusicVolume", 0);

        float elapsedTime = 0f;
        fading = true;
        while (elapsedTime < time)
        {
            musicAudioMixer.SetFloat( "MusicVolume", Mathf.Log10(Mathf.Lerp(0.0001f, targetVolume, elapsedTime / time )) * 20);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
 
        musicAudioMixer.SetFloat( "MusicVolume",Mathf.Log10(targetVolume) * 20);
        fading = false;
    }
    
    public IEnumerator FadeOut(AudioSource source, float time)
    {
        float startVolume = currentVolume;
        
        //musicAudioMixer.SetFloat( "MusicVolume",Mathf.Log10(startVolume) * 20);

        float elapsedTime = 0f;

        fading = true;
        while (elapsedTime < time)
        {
            musicAudioMixer.SetFloat( "MusicVolume", Mathf.Log10(Mathf.Lerp(startVolume, 0.0001f, elapsedTime / time)) * 20);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fading = false;
        source.Stop();
        
        // Return to settings volume after stopping music
        musicAudioMixer.SetFloat( "MusicVolume",Mathf.Log10(startVolume) * 20);
    }

    public void PauseFade()
    {
        
    }
    
    private void OnDestroy()
    {
        SleepHandler.Instance.OnPlayerWakeUp -= PlayMusic;
        slider.onValueChanged.RemoveListener(UpdateVolume);
    }
}
