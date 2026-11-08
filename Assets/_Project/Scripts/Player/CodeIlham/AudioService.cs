using UnityEngine;

public class AudioService
{
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioClip worldMusic;
    public bool Muted { get; private set; }

    public AudioService()
    {
        GameObject audioObj = new GameObject("AudioService");
        Object.DontDestroyOnLoad(audioObj);
        
        musicSource = audioObj.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 1f;
        
        sfxSource = audioObj.AddComponent<AudioSource>();
        sfxSource.volume = 1f;
    }

    public void PlayWorldMusic(AudioClip clip)
    {
        worldMusic = clip;
        PlayMusic(clip);
    }

    public void PlayBattleMusic(AudioClip clip)
    {
        PlayMusic(clip);
    }

    public void ResumeWorldMusic()
    {
        if (worldMusic != null) PlayMusic(worldMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip, float pitchJitter = 0.05f)
    {
        if (clip == null) return;
        sfxSource.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        sfxSource.PlayOneShot(clip);
    }

    public bool ToggleMute()
    {
        Muted = !Muted;
        AudioListener.volume = Muted ? 0f : 1f;
        return Muted;
    }
}