using UnityEngine;

public class AudioService
{
    private AudioSource music;
    private AudioSource sfx;
    private AudioClip worldMusic;
    private bool muted = false;

    public bool Muted => muted;

    // INISIALISASI
    public void Init(AudioSource musicSource, AudioSource sfxSource)
    {
        music = musicSource;
        sfx = sfxSource;
        Debug.Log("[AudioService] Inisialisasi selesai");
    }

    // MUSIK DUNIA (Desa/Hutan/Danau/Gua)
    public void PlayWorldMusic(AudioClip clip)
    {
        if (clip == null) return;
        worldMusic = clip;
        PlayMusic(clip);
    }

    // MUSIK BATTLE
    public void PlayBattleMusic(AudioClip clip)
    {
        if (clip == null) return;
        PlayMusic(clip);
    }

    // KEMBALI KE MUSIK DUNIA (setelah battle)
    public void ResumeWorldMusic()
    {
        if (worldMusic != null)
        {
            PlayMusic(worldMusic);
        }
    }

    // PLAY MUSIK
    private void PlayMusic(AudioClip clip)
    {
        if (music == null || clip == null) return;

        music.clip = clip;
        music.Play();
        Debug.Log($"[AudioService] Memutar musik: {clip.name}");
    }

    // SFX DENGAN PITCH JITTER
    public void PlaySfx(AudioClip clip, float pitchJitter = 0.05f)
    {
        if (clip == null || sfx == null) return;

        sfx.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        sfx.PlayOneShot(clip);
    }

    // TOGGLE MUTE (tombol M)
    public bool ToggleMute()
    {
        muted = !muted;
        AudioListener.volume = muted ? 0f : 1f;
        Debug.Log($"[AudioService] Mute: {muted}");
        return muted;
    }
}