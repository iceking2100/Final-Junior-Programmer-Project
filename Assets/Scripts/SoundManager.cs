using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

public void PlaySFX(AudioClip clip)
    {
        
    }

    public void PlayMusic(AudioClip clip)
    {

    }

    public void AdjustSFXVolume(float volume)
    {

    }
}
