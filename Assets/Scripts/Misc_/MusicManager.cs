using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private Slider slider;

    private void Start()
    {
        SleepHandler.Instance.OnPlayerWakeUp += PlayMusic;
    }

    private void Update()
    {
        audioSource.volume = slider.value;
    }

    public void PlayMusic()
    {
        int rand = Random.Range(0, songs.Length);

        audioSource.PlayOneShot(songs[rand]);
    }
}
